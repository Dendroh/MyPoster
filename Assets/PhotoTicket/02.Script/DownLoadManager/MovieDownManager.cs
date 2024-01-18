using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using AWSSDK.Examples;
using System.Net;
using System.Threading.Tasks;

public class MovieDownManager : MonoBehaviour
{
	public Text versionText;
	public Text posterProgressText;
	public Slider posterProgressSlider;
	public Text stickerProgressText;
	public Slider stickerProgressSlider;

	[HideInInspector] public MovieJsonData jsonData;

	[HideInInspector] public Sprite[] posterSprites;
	[HideInInspector] public Sprite[] idlePosterSprites;
	[HideInInspector] public Dictionary<string, Sprite> imageSprite;
	[HideInInspector] public bool isDevelopment;
	[HideInInspector] public string folderName;

	[SerializeField] GameObject DownloadLoadingShield;
	[SerializeField] Text DownLoadProgressLabel;
	[SerializeField] string basicFolderName = "Develop";
	S3Example s3;

	[SerializeField] int posterTotalCount;
	public int posterProgress;
	[SerializeField] int stickerTotalCount;
	public int stickerProgress;
	public int posterPrefabProgress;
	int imageTotalCount;
	int idlePosterProgress;
	public static bool completeDownload = false;
	// bool[] thumbnailFinish;

	DownloadImageProcess downloadImageProcess;

	string url;
	string siteId;
	string kioskId;
	string payMode;
	string passCode;

	void Start()
	{
		s3 = GetComponent<S3Example>();
		jsonData = GetComponent<MovieJsonData>();

		DownloadLoadingShield.SetActive(true);
		DownLoadProgressLabel.text = "데이터 읽어오기 시작...";

		if (GameObject.Find("Configuration") == null)
		{
			folderName = basicFolderName;
		} else
		{
			folderName = GameObject.Find("Configuration").GetComponent<PhotoTicketConfig>().param5;

			downloadImageProcess = GetComponent<DownloadImageProcess>();

			url = ConstantsScript.OPERATE_URL + "/update/assetbundles/";

			// 사용자 로컬 저장소에서 정보 가져오기
			siteId = PlayerPrefs.GetString("site_id");
			kioskId = PlayerPrefs.GetString("kiosk_id");
			payMode = PlayerPrefs.GetString("pay_mode");
			passCode = PlayerPrefs.GetString("passcode");

			new WebClient().DownloadFile(url + siteId, Application.persistentDataPath + "/" + passCode + ".txt");
		}

		finish();
	}

	public void finish()//pulldata -> OnSuccess 이후에 호출됨
	{
		jsonData.Read(folderName + ".txt");
		versionText.text = "Version: " + jsonData.Version;
		print($"영화정보 다운로드완료. 에셋 다운로드 중... 전체영화 갯수 = {jsonData.movieInfo.Count}");
		DownLoadProgressLabel.text = "데이터 읽기 완료! 다운로드 중입니다.";

		foreach (var movie in jsonData.movieInfo)
		{
			stickerTotalCount += GetStickerCount(movie.FaceCenters)
						      + GetStickerCount(movie.HandCenters)
						      + GetStickerCount(movie.Foregrounds);
		}

		posterSprites = new Sprite[jsonData.movieInfo.Count];
		idlePosterSprites = new Sprite[jsonData.IdlePoster.Length];
		posterTotalCount = jsonData.movieInfo.Count + jsonData.IdlePoster.Length;
		imageSprite = new Dictionary<string, Sprite>();

		var currentVersion = jsonData.Version;
		var localVersion = PlayerPrefs.GetString("version", "none");
		var latestSiteId = PlayerPrefs.GetString("latestSiteId", "none");   // 최근 실행 사이트
		siteId = PlayerPrefs.GetString("site_id");

		//기존 기기 버전과 검사
		if (currentVersion == localVersion && latestSiteId == siteId)
		{ // 기존 버전과 같음. 따로 에셋 다운로드 필요 없이 바로 instantiate
			print($"Version Matches. {localVersion} == {currentVersion} 따로 에셋 다운로드 필요 없이 바로 instantiate");
			CreateFolderIfNeed();
			jsonData.initEverything();
		} else
		{
			StartCoroutine(DownloadImage());
		}

		PlayerPrefs.SetString("latestSiteId", siteId);

		StartCoroutine(CheckProcess());
	}

	void CreateFolderIfNeed()
	{
		if (!Directory.Exists(Application.persistentDataPath + "/Assetbundles"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/Assetbundles");
		}
	}

	IEnumerator CheckProcess()
	{
		while (true)
		{
			var count = 0;
			yield return new WaitForSeconds(1.0f);

			if (posterProgress < posterTotalCount ||
			    stickerProgress < stickerTotalCount)
			{
				count++;
			}

			print("다운로드 완료 검사중 : " + (count > 0 ? "다운중.." : "다운완료!"));
			posterProgressText.text = (int)((float)posterProgress / posterTotalCount * 100) + "%";
			stickerProgressText.text = (int)((float)stickerProgress / stickerTotalCount * 100) + "%";

			posterProgressSlider.value = (float)posterProgress / posterTotalCount;
			stickerProgressSlider.value = (float)stickerProgress / stickerTotalCount;

			if (count > 0)
				continue;
			else
				break;
		}

		FinishEverything();
	}
	void FinishEverything()
	{
		PlayerPrefs.SetString("version", jsonData.Version);

		print("모든 다운로드 완료됨, Instantiate 시작");

		GameObject.Find("0.Intro Canvas").GetComponent<IntroUIScript>().InstantiatePoster();
		GameObject.Find("1.Select Canvas").GetComponent<SelectUIScript>().InstantiatePoster();
		GameObject.Find("2.Photo Canvas").GetComponent<PhotoUIScript>().InstantiateThumbnail();

		completeDownload = true;
	}

	int GetStickerCount(string[] stickerNames)
	{
		if (stickerNames == null)
			return 0;
		return stickerNames.Length;
	}

	public void InitSticker(string assetName)
	{
		stickerProgress++;
	}

	public void InitPoster(string assetName)
	{
		string path = Application.persistentDataPath + "/Assetbundles/" + assetName;
		var MF = jsonData.movieInfo;
		for (int i = 0; i < MF.Count; i++)
		{
			if (MF[i].MoviePoster == assetName)
			{
				posterSprites[i] = IMG2Sprite.instance.LoadNewSprite(path);
				posterProgress++;
				return;
			}
		}
		var MP = jsonData.IdlePoster;
		for (int i = 0; i < MP.Length; i++)
		{
			if (MP[i] == assetName)
			{
				idlePosterSprites[i] = IMG2Sprite.instance.LoadNewSprite(path);
				posterProgress++;
				return;
			}
		}
	}

	public void InitImage(string assetName)
	{
		//assetname, sprite
		string path = Application.persistentDataPath + "/Assetbundles/" + assetName;
		imageSprite[assetName] = IMG2Sprite.instance.LoadNewSprite(path);
	}

	IEnumerator DownloadImage()
	{
		float waitingTime = 0.05f;
		string savePath = Application.persistentDataPath + "/Assetbundles/";

		for (int i = 0; i < jsonData.IdlePoster.Length; i++)
		{
			yield return new WaitForSeconds(waitingTime);
			downloadImageProcess.DownloadPosterImage(siteId, jsonData.IdlePoster[i], jsonData.IdlePosterSize[i]);
		}

		foreach (var tempMovieInfo in jsonData.movieInfo)
		{
			yield return new WaitForSeconds(waitingTime);
			downloadImageProcess.DownloadPosterImage(siteId, tempMovieInfo.MoviePoster, tempMovieInfo.MoviePosterSize);

			for (int i = 0; i < tempMovieInfo.FaceCenters.Length; i++)
			{
				yield return new WaitForSeconds(waitingTime);
				downloadImageProcess.DownloadStickerImage(siteId, tempMovieInfo.FaceCenters[i], tempMovieInfo.FaceCentersSize[i]);
			}

			for (int i = 0; i < tempMovieInfo.HandCenters.Length; i++)
			{
				yield return new WaitForSeconds(waitingTime);
				downloadImageProcess.DownloadStickerImage(siteId, tempMovieInfo.HandCenters[i], tempMovieInfo.HandCentersSize[i]);
			}

			for (int i = 0; i < tempMovieInfo.Foregrounds.Length; i++)
			{
				yield return new WaitForSeconds(waitingTime);
				downloadImageProcess.DownloadStickerImage(siteId, tempMovieInfo.Foregrounds[i], tempMovieInfo.ForegroundsSize[i]);
			}

			if (tempMovieInfo.chromakeyBackground != null && tempMovieInfo.isChromakey == true)
			{
				yield return new WaitForSeconds(waitingTime);
				downloadImageProcess.DownloadStickerImage(siteId, tempMovieInfo.chromakeyBackground, tempMovieInfo.chromakeySize);
			}
		}

		InitPosterImage();
	}

	public void InitPosterImage()
	{
		string savePath = Application.persistentDataPath + "/Assetbundles/";

		for (int i = 0; i < jsonData.IdlePoster.Length; i++)
		{
			idlePosterSprites[i] = IMG2Sprite.instance.LoadNewSprite(savePath + jsonData.IdlePoster[i]);
		}

		int movieInfoCount = 0;

		foreach (var tempMovieInfo in jsonData.movieInfo)
		{
			posterSprites[movieInfoCount] = IMG2Sprite.instance.LoadNewSprite(savePath + tempMovieInfo.MoviePoster);

			movieInfoCount++;
		}
	}
}