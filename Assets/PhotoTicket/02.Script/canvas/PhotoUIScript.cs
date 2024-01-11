using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alchera;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using UnityEngine.Video;

public class PhotoUIScript : MonoBehaviour, UIScript
{
	[Header("사진 촬영버튼 클릭 전")]
	// 로딩화면 관련
	[SerializeField] Text posterProgressText;
	[SerializeField] Text posterProgressLabel;
	[SerializeField] Slider posterProgressSlider;
	[SerializeField] Text stickerProgressText;
	[SerializeField] Text stickerProgressLabel;
	[SerializeField] Slider stickerProgressSlider;
	[SerializeField] Text posterPrefabProgressText;
	[SerializeField] Text posterPrefabProgressLabel;
	[SerializeField] Slider posterPrefabProgressSlider;
	[SerializeField] Text DownLoadProgressLabel;

	// 일반, 크로마키 촬영화면 설정 관련
	[SerializeField] GameObject chromarkeyWebcam;
	[SerializeField] GameObject normalWebcam;

	[SerializeField] MovieDownManager downManager;
	[SerializeField] GameObject thumbnailPrefab;
	[SerializeField] Scrollbar movieScroll;
	[SerializeField] Transform content;
	[SerializeField] GameObject downArrow;
	[SerializeField] GameObject upArrow;
	[SerializeField] GameObject carousel;
	[SerializeField] GameObject quizClickBlocker;
	[SerializeField] GameObject introButton;
	[Space]
	[Header("사진 촬영버튼 클릭 후")]
	[SerializeField] GameObject redButton;
	[SerializeField] GameObject clickBlocker;
	[SerializeField] GameObject guideText;
	[SerializeField] GameObject FirstBG;
	[SerializeField] Text photoCountText;
	[SerializeField] Animation photoResultAnimation;
	[SerializeField] RawImage JPGResult;
	[SerializeField] RawImage GifResult;
	[SerializeField] GameObject resultLoadingShield;
	[SerializeField] AudioSource photoAudioKr;
	[SerializeField] AudioSource photoAudioEn;
	[SerializeField] AudioSource contentsClickAudio;
	[SerializeField] AudioSource buttonAudio;
	[SerializeField] GameObject DownloadLoadingShield;

	Button[] posterButtons;
	public RectTransform[] moviePosters;
	RectTransform[] posterChecks;
	RectTransform photoCountTextRT;
	ReadWebcamInSequence camReader;
	StickerController sticker;
	ComplexSceneBehavior detector;
	bool isPhotoUIInitialized = false;
	static bool doneSetPoster = false; // 포스터 프리팹 생성 완료 여부, 비동기에서 사용하기에 static
	static bool doneLoading = false;
	bool isPhotoCanvas = true;
	//int posterWidth = 280;
	int posterHeight = 280;
	bool[] isFree;
	public int posterPrepabProgress = 0;
	float sliderValue;
	float percent;

	public void InstantiateThumbnail()
	{
		Debug.Log("InstantiateThumbnail");
		moviePosters = new RectTransform[downManager.jsonData.movieInfo.Count];
		var movieInfo = downManager.jsonData.movieInfo;
		isFree = new bool[moviePosters.Length];

		for (int i = 0; i < moviePosters.Length; i++)
		{
			moviePosters[i] = Instantiate(thumbnailPrefab, Vector3.zero, Quaternion.identity).GetComponent<RectTransform>(); //포스터 갯수만큼 복제
			moviePosters[i].transform.SetParent(content);

			moviePosters[i].GetComponent<RectTransform>().anchoredPosition3D = Vector3.one;
			moviePosters[i].GetComponent<RectTransform>().localScale = new Vector3(3f, 2.8f, 1);

			moviePosters[i].GetComponent<Image>().sprite = downManager.posterSprites[i];
			moviePosters[i].GetComponent<ThumbnailController>().id = i;
			moviePosters[i].GetComponent<ThumbnailController>().init();

			isFree[i] = movieInfo[i].MovieIsFree;
		}

		camReader = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ReadWebcamInSequence>();
		photoCountTextRT = photoCountText.GetComponent<RectTransform>();
		posterButtons = new Button[moviePosters.Length];
		posterChecks = new RectTransform[moviePosters.Length];

		for (int i = 0; i < moviePosters.Length; i++)
		{
			posterButtons[i] = moviePosters[i].GetComponent<Button>();
			posterChecks[i] = moviePosters[i].GetComponentsInChildren<RectTransform>()[3]; //체크박스

			//무료인지 보여주는 이미지 설정
			moviePosters[i].GetComponentsInChildren<RectTransform>()[1].gameObject.SetActive(!isFree[i]);
			// 유료 이미지 설정
			// moviePosters[i].GetComponentsInChildren<RectTransform>()[2].gameObject.SetActive(!isFree[i]);
			moviePosters[i].pivot = new Vector2(-i, 1);
		}
		var mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		sticker = mainCamera.GetComponent<StickerController>();
		detector = mainCamera.GetComponent<ComplexSceneBehavior>();

		if (moviePosters.Length % 5 != 0)
			content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, posterHeight * ((moviePosters.Length / 5) - 1) + (posterHeight / 2.2f) - 130);
		else
			content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, posterHeight * ((moviePosters.Length / 5) - 2) + (posterHeight / 2.2f) - 130);

		isPhotoUIInitialized = true;
	}

	void Update()
	{
		//포스터 프리팹 설정
		if (MovieDownManager.completeDownload)
		{    // 포스터 다운로드가 완료된 경우
			MovieDownManager.completeDownload = false;

			Loading();  // PosterPrefab Loding 활성화

			// 포스터 프리팹 설정
			StartCoroutine(SetPoster());
		}

		posterPrefabProgressText.text = percent + "%";
		posterPrefabProgressSlider.value = sliderValue;

		if (doneLoading)
		{  // 로딩이 완료된 경우
			doneLoading = false;
			posterPrefabProgressText.text = 100 + "%";
			posterPrefabProgressSlider.value = 100;

			DownloadLoadingShield.SetActive(false); // Loding 비활성화
		}

		//instantiate 후 update 호출
		if (isPhotoUIInitialized == false)
			return;
		if (isPhotoCanvas == false)
			return;
		if (moviePosters.Length <= 4)
			return;

		// 풋 스위치 클릭시 촬영
		if (Input.GetKeyDown(KeyCode.F3))
		{
			if (redButton.activeSelf)
			{
				Debug.Log("F3 key was pressed.");
				ClickRedButton();
			}
		}
	}

	public void Init()
	{
		print("IntroPhoto");
		ReadWebcamInSequence.bSendTexture = true;
		camReader.changeCameraState(true);
		downArrow.SetActive(true);
		upArrow.SetActive(true);
		isPhotoCanvas = true;
		JPGResult.texture = null;
		GifResult.texture = null;
		redButton.SetActive(true);
		clickBlocker.SetActive(false);
		guideText.SetActive(true);
		FirstBG.SetActive(true);
		detector.Init();
		movieScroll.value = 1;
		selectPoster(FlowController.instance.currentMovieNumber);
		contentsClickAudio.enabled = true;
		carousel.GetComponent<RectTransform>().sizeDelta = new Vector2(1080, 610);

		if (PlayerPrefs.GetString("quiz") == "true")
		{  // 퀴즈 모드인 경우
			quizClickBlocker.SetActive(true);
			introButton.SetActive(false);
			carousel.GetComponent<RectTransform>().sizeDelta = new Vector2(1080, 480);
		}

		StartCoroutine(UtilsScript.playAudio(photoAudioKr, photoAudioEn));
	}

	public void Dispose()
	{
		print("DisposePhoto");
		// sticker.RemoveAllStickers();
		resultLoadingShield.SetActive(false);
		photoResultAnimation.Play();
		isPhotoCanvas = false;
		contentsClickAudio.enabled = false;
		ReadWebcamInSequence.bUpdateQuad = false;   // 쿼드 설정 여부 초기화
		ReadWebcamInSequence.bSendTexture = false;

		// 화면 전환을 위한 오디오 중지
		StartCoroutine(UtilsScript.stopAudio(photoAudioKr));
		StartCoroutine(UtilsScript.stopAudio(photoAudioEn));
	}

	IEnumerator SetPoster()
	{
		for (int i = 0; i < downManager.jsonData.movieInfo.Count; i++)
		{
			sticker.SetSticker(i);
			posterPrepabProgress++;

			if (i == downManager.jsonData.movieInfo.Count - 1)
			{    // 마지막 반복인 경우
				doneSetPoster = true;
			}

			yield return null;
		}

		yield break;
	}

	void ChangeBackGroundQuadMaterial(int movieNumber, string type)
	{
		GameObject backGroundQuad = GameObject.Find("BackGroundQuad");

		MeshRenderer meshRenderer = backGroundQuad.GetComponent<MeshRenderer>();

		// 이미지 경로
		string filePath = Path.Combine(Application.persistentDataPath, "Assetbundles", downManager.jsonData.movieInfo[movieNumber].chromakeyBackground);

		if (File.Exists(filePath))
		{
			if (type == "0")
			{  // 이미지
				byte[] fileData = File.ReadAllBytes(filePath); // 파일을 바이트 배열로 읽음

				Texture2D loadedTexture = new Texture2D(1080, 1440); // 새로운 Texture2D 생성 (크기 조정 필요)
				loadedTexture.LoadImage(fileData); // 파일 데이터를 Texture2D에 로드

				Sprite ccImage = centerCrop(loadedTexture, 1080, 1440);

				// Renderer의 mainTexture에 할당
				meshRenderer.material.mainTexture = ccImage.texture;

				Texture backGroundTexture = backGroundQuad.GetComponent<Renderer>().material.mainTexture;

				AutoBackgroundQuad.SetQuadSize(backGroundQuad, backGroundTexture);
			} else
			{    // 비디오
				VideoPlayer videoPlayer = backGroundQuad.GetComponent<VideoPlayer>();
				if (videoPlayer == null)
				{
					videoPlayer = backGroundQuad.AddComponent<VideoPlayer>();
					videoPlayer.isLooping = true;
				}

				videoPlayer.url = filePath; // 비디오 파일 할당

				// 음향 출력 모드를 "None"으로 설정
				videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

				// 볼륨을 0으로 설정
				videoPlayer.SetDirectAudioVolume(0, 0);

				backGroundQuad.transform.localScale = new Vector3(-backGroundQuad.transform.localScale.x, backGroundQuad.transform.localScale.y, backGroundQuad.transform.localScale.z); //좌우반전

				// 비디오 크기를 1080x1440으로 조절하고 센터 크롭 적용
				videoPlayer.targetCamera = null;
				videoPlayer.targetTexture = RenderTexture.GetTemporary(1080, 1440);
				videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
				// videoPlayer.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
				videoPlayer.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // 현재 각도
				videoPlayer.transform.Rotate(0f, 0f, -180f); // 추가로 180도 회전
				videoPlayer.Play();
			}
		}
	}

	Sprite centerCrop(Texture2D texture, int imageWidth, int imageHeight)
	{
		float cropWidth;
		float cropHeight;

		float imageRatio = (float)texture.width / (float)texture.height; // 이미지 비율
										 // center crop and resize
		if (imageRatio > 1.0)
		{ // 가로가 긴 경우
			cropHeight = imageHeight;
			cropWidth = cropHeight * imageRatio;
		} else
		{    // 세로가 긴 경우
			cropWidth = imageWidth;
			cropHeight = cropWidth / imageRatio;
		}

		if (cropWidth < imageWidth)
		{
			float widthRatio = (float)imageWidth / (float)cropWidth;
			cropWidth = imageWidth;
			cropHeight = cropHeight * widthRatio;
		}

		if (cropHeight < imageHeight)
		{
			float heightRatio = (float)imageHeight / (float)cropHeight;
			cropHeight = imageHeight;
			cropWidth = cropWidth * heightRatio;
		}

		// 이미지 생성
		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

		return sprite;
	}

	public async void Loading()
	{
		// 포스터 다운로드 Progress UI 비활성화
		posterProgressText.gameObject.SetActive(false);
		posterProgressLabel.gameObject.SetActive(false);
		posterProgressSlider.gameObject.SetActive(false);
		stickerProgressText.gameObject.SetActive(false);
		stickerProgressLabel.gameObject.SetActive(false);
		stickerProgressSlider.gameObject.SetActive(false);

		// 포스터 Set Prefab Progress UI 활성화
		posterPrefabProgressText.gameObject.SetActive(true);
		posterPrefabProgressLabel.gameObject.SetActive(true);
		posterPrefabProgressSlider.gameObject.SetActive(true);
		DownLoadProgressLabel.text = "포스터 스티커 이미지 설정중...";

		await Task.Run(async () =>
		{
			while (!doneSetPoster)
			{    // 완료 안된 경우
				int posterTotalCount = downManager.jsonData.movieInfo.Count;

				// 로딩바,  퍼센티지 반영
				percent = (int)((float)posterPrepabProgress / posterTotalCount * 100);
				sliderValue = (float)posterPrepabProgress / posterTotalCount;

				await Task.Delay(50); // 1초마다 동작
			}

			doneLoading = true;
		});
	}

	public void selectPoster(int movieNumber)
	{
		if (UtilsScript.checkConfig() != "")
		{
			contentsClickAudio.Play();    // 컨텐츠 클릭 효과음 출력
		}

		for (int i = 0; i < posterButtons.Length; i++)
		{
			posterButtons[i].interactable = true;
			posterChecks[i].gameObject.SetActive(false);
		}

		posterButtons[movieNumber].interactable = false;
		posterChecks[movieNumber].gameObject.SetActive(true);

		var movieInfo = downManager.jsonData.movieInfo;
		string productId = movieInfo[movieNumber].ID;

		sticker.setActiveStickers(movieNumber);

		if (downManager.jsonData.movieInfo[movieNumber].isChromakey)
		{  // 크로마키 컨텐츠인 경우
			if (AutoBackgroundQuad.updateQuad == true)
			{    // Quad 설정이 완료된 경우
				chromarkeyWebcam.gameObject.SetActive(true);    // 크로마키 캠 ON
				normalWebcam.gameObject.SetActive(false);   // 일반 캠 OFF
			}

			ChangeBackGroundQuadMaterial(movieNumber, downManager.jsonData.movieInfo[movieNumber].chromakeyType);  // 크로마키 화면 적용
		} else
		{    // 일반 컨텐츠인 경우
			if (AutoBackgroundQuad.updateQuad == true)
			{    // Quad 설정이 완료된 경우
				chromarkeyWebcam.gameObject.SetActive(false);   // 크로마키 캠 OFF
				normalWebcam.gameObject.SetActive(true);    // 일반 캠 ON
			}
		}

		PlayerPrefs.SetString("productId", productId);

		FlowController.instance.currentMovieNumber = movieNumber;
		FlowController.instance.currentMovieId = downManager.jsonData.movieInfo[movieNumber].ID;
	}




	public void SetPhotoCountText(float timer, float expectedTime)
	{
		if (timer == expectedTime)
		{
			photoCountText.text = "Ready";
			photoCountText.fontSize = 130;
			photoCountTextRT.anchoredPosition = new Vector2(0, -100f);

		} else
		{
			photoCountText.text = Mathf.Ceil(timer).ToString();
			photoCountText.fontSize = 200;
			photoCountTextRT.anchoredPosition = new Vector2(0, -120f);

		}
	}

	public void ClickRedButton()
	{
		redButton.SetActive(false);
		clickBlocker.SetActive(true);
		guideText.SetActive(false);
		FirstBG.SetActive(false);
		quizClickBlocker.SetActive(false);
		detector.StartTimer();
		carousel.GetComponent<RectTransform>().sizeDelta = new Vector2(1080, 480);

		// 화면 전환을 위한 오디오 중지
		StartCoroutine(UtilsScript.stopAudio(photoAudioKr));
		StartCoroutine(UtilsScript.stopAudio(photoAudioEn));
	}

	/* 2019-07-12 백상열 추가 */
	/* 처음화면으로 돌아가기 */
	public void GoToIntro()
	{
		if (UtilsScript.checkConfig() != "")
		{
			buttonAudio.Play();    // 버튼 효과음 출력
		}

		FlowController.instance.ChangeFlow(FlowController.instance.selectCanvas);
	}
}
