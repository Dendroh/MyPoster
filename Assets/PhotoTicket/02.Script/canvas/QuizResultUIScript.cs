using Alchera;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class QuizResultUIScript : MonoBehaviour, UIScript
{
	// [SerializeField] Text resultText;
	[SerializeField] Text evaluationText;
	[SerializeField] Text countText;
	[SerializeField] Text timerText;
	[SerializeField] Text contentsText;
	[SerializeField] AudioSource buttonAudio;
	[SerializeField] GameObject evaluate100Image;
	[SerializeField] GameObject evaluate80Image;
	[SerializeField] GameObject evaluate60Image;
	[SerializeField] GameObject evaluate40Image;
	[SerializeField] PhotoUIScript photoUIScript;
	[SerializeField] GameObject advCanvas;
	[SerializeField] MovieDownManager downManager;

	// 프로모션 관련
	[SerializeField] Image resultImage;
	[SerializeField] Text resultText;

	ReadWebcamInSequence camReader;

	float timer = 6f;
	bool isOnTimer = false;
	bool result;
	int resultImageSpace = 50;

	void Start()
	{
		if (PlayerPrefs.GetString("quiz") == "true")
		{
			StartCoroutine(setResultImage());   // 결과 이미지 설정
		}

		camReader = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ReadWebcamInSequence>();
	}

	public void Init()
	{
		// 사용자 저장 정보 가져오기
		float count = PlayerPrefs.GetInt("correctCount");
		float quizcount = PlayerPrefs.GetInt("quizCount");
		int criteria = PlayerPrefs.GetInt("criteria");

		timer = 6f;    // 타이머 시간 설정, 화면 구성 시간을 고려해 0.2초 추가

		// 텍스트 UI 구성
		countText.text = count.ToString();
		timerText.text = ((int)timer).ToString();
		resultText.text = PlayerPrefs.GetString("doneDesc");

		if (count / quizcount * 100 < criteria)
		{   // 정답 비율이 기준점보다 낮은 경우
			evaluationText.text = "힘내요..";
			evaluate40Image.SetActive(true);
			contentsText.text = "아쉽지만 포토카드 촬영을 하러 가볼까요?";
			result = false;
		} else
		{
			evaluationText.text = "훌륭해요!";
			evaluate100Image.SetActive(true);
			contentsText.text = "잘했어요! 그럼 포토카드 촬영을 하러 가볼까요?";
			result = true;
		}

		isOnTimer = true;   // 타이머 실행

		//if (count / quizcount < 0.6) {  // 정답률 60% 미만 - 40% 점수
		//    evaluationText.text = "힘내요..";
		//    evaluate40Image.SetActive(true);
		//} else if (count / quizcount < 0.8) {   // 정답률 80% 미만 - 60% 점수
		//    evaluationText.text = "좋아요!";
		//    evaluate60Image.SetActive(true);
		//} else if (count / quizcount < 1) { // 정답률 100% 미만 - 80% 점수
		//    evaluationText.text = "훌륭해요!";
		//    evaluate80Image.SetActive(true);
		//} else {    // 정답률 100%
		//    evaluationText.text = "완벽해요!";
		//    evaluate100Image.SetActive(true);
		//}
	}

	void Update()
	{
		if (isOnTimer)
		{
			if (timer > 0)
			{
				timer -= Time.deltaTime;  //3,2,1숫자 셀 카운터
				timerText.text = ((int)timer).ToString();
			} else
			{
				takeResultPicture();    // 촬영 화면 전환
				isOnTimer = false;  // 타이머 종료
			}
		}
	}

	public void Dispose()
	{
		print("DisposeQuizResult");

		// UI 구성
		evaluate40Image.SetActive(false);
		evaluate60Image.SetActive(false);
		evaluate80Image.SetActive(false);
		evaluate100Image.SetActive(false);
		advCanvas.gameObject.SetActive(true);  // 광고 화면 활성화

		// 운영 정보 초기화
		PlayerPrefs.SetInt("correctCount", 0);
		PlayerPrefs.SetInt("quizCount", 0);
		PlayerPrefs.SetString("donePic", "");
		PlayerPrefs.SetString("quizFilePath", "");
	}

	/**
	 * 촬영 포스터 정보 가져오기
	 * @param id
	 * @return int
	 */
	public int getPosterNumber(string id)
	{
		var movieInfoList = downManager.jsonData.movieInfo;
		for (int i = 0; i < movieInfoList.Count; i++)
		{
			if (movieInfoList[i].ID == id)
			{
				return i;
			}
		}

		return 0;
	}

	/**
	 * 결과 촬영하기
	 * @param number
	 */
	public void takeResultPicture()
	{
		int number = 0;
		ReadWebcamInSequence.bUpdateQuad = false;
		camReader.changeCameraState(true);
		if (result)
		{   // 퀴즈 과반수 이상 맞추기 성공한 경우
			number = getPosterNumber(PlayerPrefs.GetString("successContent"));
			photoUIScript.selectPoster(number);
		} else
		{    // 실패한 경우
			number = getPosterNumber(PlayerPrefs.GetString("failureContent"));
			photoUIScript.selectPoster(number);
		}

		FlowController.instance.ChangeFlow(FlowController.instance.photoCanvas);
	}

	/**
	 * 결과 이미지 구성하기
	 */
	IEnumerator setResultImage()
	{
		// 설정 이미지 구성
		string imageFilePath = ConstantsScript.OPERATE_URL + PlayerPrefs.GetString("quizFilePath") + "/" + PlayerPrefs.GetString("donePic");
		RectTransform rectTransform = resultImage.GetComponent<RectTransform>();
		List<string> fileList = new List<string>();
		fileList.Add(imageFilePath);
		StartCoroutine(addImages(fileList, resultImage.GetComponent<RectTransform>()));

		string test = PlayerPrefs.GetString("doneDesc");

		if (PlayerPrefs.GetString("doneDesc").Length > 18)
		{    // 글자수가 2라인 이상인 경우, 아래로 이동
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - resultImageSpace);
		}

		yield break;
	}

	/**
	 * 게임 오브젝트 종료
	 * @param gameObject
	 */
	public void close(GameObject gameObject)
	{
		gameObject.SetActive(false);
	}

	/**
	 * 서버로부터 이미지 정보 가져와 이미지 생성하기
	 * @param filePathList
	 * @param rectTransform  이미지 영역 크기
	 */
	IEnumerator addImages(List<string> filePathList, RectTransform rectTransform)
	{
		// 이미지 크기 설정
		float imageWidth = rectTransform.rect.width;
		float imageHeight = rectTransform.rect.height;

		for (int i = 0; i < filePathList.Count(); i++)
		{
			using (WWW www = new WWW(filePathList[i]))
			{    // 서버로부터 이미지 정보 가져오기
				yield return www;
				if (string.IsNullOrEmpty(www.error))
				{  // 성공
					Texture2D texture = www.texture;

					float cropWidth;
					float cropHeight;

					float imageRatio = (float)texture.width / (float)texture.height;
					// 이미지 비율
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

					// 이미지 영역에 추가
					GameObject imageObject = new GameObject("Image");
					imageObject.transform.SetParent(rectTransform.transform, false);
					Image image = imageObject.AddComponent<Image>();
					image.sprite = sprite;
					image.rectTransform.sizeDelta = new Vector2(cropWidth, cropHeight);
				} else
				{
					Debug.LogError("Failed to load image from server: " + www.error);
				}
			}
		}
	}

	IEnumerator loadWebPage(GameObject gameObject, string url)
	{
		UnityWebRequest request = UnityWebRequest.Get(url);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
		{
			Debug.LogError("Failed to load web page: " + request.error);
		} else
		{
			string html = request.downloadHandler.text;
			gameObject.GetComponentInChildren<Text>().text = html;
		}
	}

	/*
	 * 팝업 닫기
	 * @parma popup
	 */
	public void cancelPopup(GameObject popup)
	{
		// 버튼 효과음 출력
		StartCoroutine(playEffectAudio(buttonAudio));

		popup.SetActive(false);
	}

	/**
	 * 운영 모드에 따른 효과음 출력
	 * @param effect
	 * @return IEnumerator
	 */
	IEnumerator playEffectAudio(AudioSource effect)
	{
		if (UtilsScript.checkConfig() != null && UtilsScript.checkConfig() != "")
		{
			effect.Play();
		}

		yield return null;
	}
}
