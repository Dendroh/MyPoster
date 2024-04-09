using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AWSSDK.Examples;
using System.Net;

public class MovieDownManager : MonoBehaviour
{
	public Text versionText;

	[HideInInspector] public MovieJsonData jsonData;

	[HideInInspector] public Sprite[] posterSprites;
	[HideInInspector] public Sprite[] idlePosterSprites;
	[HideInInspector] public Dictionary<string, Sprite> imageSprite;
	[HideInInspector] public bool isDevelopment;
	[HideInInspector] public string folderName;

	[SerializeField] GameObject DownloadLoadingShield;
	[SerializeField] Text DownLoadProgressLabel;
	[SerializeField] string basicFolderName = "Develop";

	public int posterProgress;
	public int stickerProgress;
	public int posterPrefabProgress;
	public static bool initialRun = false;
	public static bool completeDownload = false;

	DownloadImageProcess downloadImageProcess;

	string url, siteId, kioskId, payMode, passCode;

	void Start()
	{
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

		posterSprites = new Sprite[jsonData.movieInfo.Count];
		idlePosterSprites = new Sprite[jsonData.IdlePoster.Length];
		imageSprite = new Dictionary<string, Sprite>();

		var currentVersion = jsonData.Version;
		var localVersion = PlayerPrefs.GetString("version", "none");
		var latestSiteId = PlayerPrefs.GetString("latestSiteId", "none");   // 최근 실행 사이트
		siteId = PlayerPrefs.GetString("site_id");

		//기존 기기 버전과 검사
		if (currentVersion == localVersion && latestSiteId == siteId)
		{ 
			// 기존 버전과 같음. 따로 에셋 다운로드 필요 없이 바로 instantiate
			CreateFolderIfNeed();
			jsonData.initEverything();
		} else
		{
			StartCoroutine(DownloadImage());
			initialRun = true;
		}

		if (initialRun == false)
		{
			print("모든 다운로드 완료됨, Instantiate 시작");

			GameObject.Find("0.Intro Canvas").GetComponent<IntroUIScript>().InstantiatePoster();
			GameObject.Find("1.Select Canvas").GetComponent<SelectUIScript>().InstantiatePoster();
			GameObject.Find("2.Photo Canvas").GetComponent<PhotoUIScript>().InstantiateThumbnail();

			completeDownload = true;
		} else
		{
			print("최초 사이트 접속중..");
		}

		PlayerPrefs.SetString("latestSiteId", siteId);
		PlayerPrefs.SetString("version", jsonData.Version);
	}

	void CreateFolderIfNeed()
	{
		if (!Directory.Exists(Application.persistentDataPath + "/Assetbundles"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/Assetbundles");
		}
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
		string path = Application.persistentDataPath + "/Assetbundles/" + assetName;
		imageSprite[assetName] = IMG2Sprite.instance.LoadNewSprite(path);
	}

	public void InitSticker(string assetName)
	{
		stickerProgress++;
	}

	IEnumerator DownloadImage()
	{
		float waitingTime = 0.05f;

		for (int i = 0; i < jsonData.IdlePoster.Length; i++)
		{
			yield return new WaitForSeconds(waitingTime);
			yield return StartCoroutine(downloadImageProcess.DownloadPosterImageCoroutine(siteId, jsonData.IdlePoster[i], jsonData.IdlePosterSize[i]));
		}

		foreach (var tempMovieInfo in jsonData.movieInfo)
		{
			yield return new WaitForSeconds(waitingTime);
			yield return StartCoroutine(downloadImageProcess.DownloadPosterImageCoroutine(siteId, tempMovieInfo.MoviePoster, tempMovieInfo.MoviePosterSize));

			for (int i = 0; i < tempMovieInfo.FaceCenters.Length; i++)
			{
				yield return new WaitForSeconds(waitingTime);
				yield return StartCoroutine(downloadImageProcess.DownloadStickerImageCoroutine(siteId, tempMovieInfo.FaceCenters[i], tempMovieInfo.FaceCentersSize[i]));
			}

			for (int i = 0; i < tempMovieInfo.HandCenters.Length; i++)
			{
				yield return new WaitForSeconds(waitingTime);
				yield return StartCoroutine(downloadImageProcess.DownloadStickerImageCoroutine(siteId, tempMovieInfo.HandCenters[i], tempMovieInfo.HandCentersSize[i]));
			}

			for (int i = 0; i < tempMovieInfo.Foregrounds.Length; i++)
			{
				yield return new WaitForSeconds(waitingTime);
				yield return StartCoroutine(downloadImageProcess.DownloadStickerImageCoroutine(siteId, tempMovieInfo.Foregrounds[i], tempMovieInfo.ForegroundsSize[i]));
			}

			if (tempMovieInfo.chromakeyBackground != null && tempMovieInfo.isChromakey == true)
			{
				yield return new WaitForSeconds(waitingTime);
				yield return StartCoroutine(downloadImageProcess.DownloadStickerImageCoroutine(siteId, tempMovieInfo.chromakeyBackground, tempMovieInfo.chromakeySize));
			}
		}

		Debug.LogWarning("모든 이미지 다운로드 완료");
		Application.Quit();
	}
}