using Alchera;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelectUIScript : MonoBehaviour, UIScript
{
	[SerializeField] MovieDownManager downManager;
	[SerializeField] Scrollbar movieScroll;
	[SerializeField] Transform posterParent;
	[SerializeField] GameObject moviePosterPrefab;
	[SerializeField] GameObject printErrorPopup;
	[SerializeField] GameObject cameraErrorPopup;
	[SerializeField] AudioSource audioKr;
	[SerializeField] AudioSource audioEn;
	[SerializeField] AudioSource counselAudioKr;
	[SerializeField] AudioSource counselAudioEn;
	[SerializeField] AudioSource buttonAudio;
	[SerializeField] SendUIScript sendUIScript;
	[SerializeField] AgreementUIScript agreementUIScript;
	[SerializeField] EndUIScript endUIScript;
	[SerializeField] PaymentUIScript paymentUIScript;
	[SerializeField] GameObject netClientErrorCanvas;
	[SerializeField] GameObject isReconnectPopup;
	[SerializeField] GameObject failReconnectPopup;
	[SerializeField] GameObject reconnectPopup;
	[SerializeField] GameObject printProgressPopup;

	bool bPhotoable = true; // 촬영 가능 여부
	RectTransform[] moviePosters;
	public string filePath; // 가져올 파일 경로
	bool[] isFree;
	bool isSelectUIInitialized = false;
	bool isSelectCanvas = false;
	Button[] posterButtons;
	float[] thumbnailValues;
	float interval;
	float posterPosition;
	int posterWidth = 574;
	int maxLoopCount = 10;  // 루프 최대 반복 횟수
	int intervalCount;
	int scrollIndex = 0;
	Vector2 beforePos;

	ReadWebcamInSequence camReader;

	public static NetClient _netClient; // 네트워크 설정
	public string printStatus = "";   // 프린터 상태
	public string checkPrintResult = ""; // 프린터 상태 체크 결과
	public bool paymentProcess;
	public string paymentResult = "";
	public string canvas = "";
	public string sendType = "";
	private string _approvalNum = "";
	private string _approvalDate = "";  // 서버 전달 용 날짜
	private string _paymentDate = "";   // 결제 취소 용 날짜
	private string _price = "";

	private void Awake()
	{
		// 네트워크 통신 설정
		_netClient = new NetClient();
		_netClient.OnReceiveMessage += ReceiveMessage;
		_netClient.Start();

		Debug.Log("NetClient: Start");
	}

	void Start()
	{
		camReader = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ReadWebcamInSequence>();
		camReader.changeCameraState(true);
	}


	void Update()
	{
		if (isSelectUIInitialized == false)
			return;
		if (isSelectCanvas == false)
			return;

		if (Input.GetMouseButtonDown(0))
		{
			beforePos = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (beforePos.x > Input.mousePosition.x + 40)
			{
				moveRight();
			} else if (beforePos.x + 40 < Input.mousePosition.x)
			{
				moveLeft();
			}
		}
		for (int i = 0; i < thumbnailValues.Length; i++)
		{
			moviePosters[i].localScale = Vector3.one * Mathf.Max((1 - 2f * Mathf.Abs(movieScroll.value - thumbnailValues[i])), 0.646f);
			if (moviePosters[i].localScale.x > 0.85f)
				posterButtons[i].interactable = true;
			else
				posterButtons[i].interactable = false;
		}
	}

	public void InstantiatePoster()
	{
		print("instantiate Poster");
		var movieInfo = downManager.jsonData.movieInfo;
		moviePosters = new RectTransform[movieInfo.Count];
		isFree = new bool[movieInfo.Count];

		for (int i = 0; i < movieInfo.Count; i++)
		{
			moviePosters[i] = Instantiate(moviePosterPrefab, Vector3.zero, Quaternion.identity).GetComponent<RectTransform>();
			moviePosters[i].transform.SetParent(posterParent);
			moviePosters[i].GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

			moviePosters[i].GetComponent<Image>().sprite = downManager.posterSprites[i];
			moviePosters[i].GetComponent<PosterController>().id = i;
			moviePosters[i].GetComponent<PosterController>().init();
			isFree[i] = movieInfo[i].MovieIsFree;
		}

		intervalCount = (moviePosters.Length) * 2;
		interval = 1f / intervalCount;
		posterPosition = 1f / ((moviePosters.Length + 2) * 2);
		thumbnailValues = new float[moviePosters.Length];
		posterButtons = new Button[moviePosters.Length];
		for (int i = 0; i < moviePosters.Length; i++)
		{
			var ratio = interval * (2 * i + 1);
			thumbnailValues[i] = ratio;
			moviePosters[i].anchorMin = new Vector2(posterPosition * (2 * i + 3), 0);
			moviePosters[i].anchorMax = new Vector2(posterPosition * (2 * i + 3), 1);
			moviePosters[i].GetComponentsInChildren<Image>()[1].gameObject.SetActive(!isFree[i]);
			posterButtons[i] = moviePosters[i].GetComponent<Button>();
		}

		posterParent.GetComponent<RectTransform>().sizeDelta = new Vector3(posterWidth * 0.95131f * (moviePosters
		.Length + 2), 0);

		movieScroll.value = thumbnailValues[0]; //movie 1으로 초기화
		scrollIndex = 0;
		posterButtons[0].interactable = true;
		for (int i = 1; i < posterButtons.Length; i++)
		{
			posterButtons[i].interactable = false;
		}
		isSelectUIInitialized = true;
	}

	void moveRight()
	{
		if (scrollIndex < moviePosters.Length - 1)
			scrollIndex++;

		StartCoroutine(moveScroll(thumbnailValues[scrollIndex]));
	}

	void moveLeft()
	{
		if (scrollIndex > 0)
			scrollIndex--;

		StartCoroutine(moveScroll(thumbnailValues[scrollIndex]));
	}

	IEnumerator moveScroll(float value)
	{
		yield return null;
		var changed = movieScroll.value > value;
		while (movieScroll.value > value)
		{
			movieScroll.value -= 0.05f;
			if (movieScroll.value < value)
			{
				movieScroll.value = value;
				yield break;
			}
			yield return null;
		}
		while (movieScroll.value < value)
		{
			movieScroll.value += 0.05f;
			if (movieScroll.value > value)
			{
				movieScroll.value = value;
				yield break;
			}
			yield return null;
		}
	}

	public void Init()
	{
		if (PlayerPrefs.GetString("pay_mode").Equals("t") || PlayerPrefs.GetString("pay_mode").Equals("r"))
		{
			bPhotoable = false;
			checkPrint();
		}

		FlowController.instance.currentMovieNumber = -1;
		isSelectCanvas = true;
		movieScroll.value = thumbnailValues[0]; //movie 1으로 초기화
		scrollIndex = 0;

		StartCoroutine(UtilsScript.playAudio(audioKr, audioEn));
		posterButtons[0].interactable = true;
	}

	public void checkPrint()
	{
		if (_netClient.status == true)
		{
			StartCoroutine(CheckQueue());

			sendType = "check_print";
			canvas = "select";

			AgentSendData sendData = new AgentSendData();
			sendData.Command = "print_status";
			sendData.PrintCount = "1";

			string checkMessage = JsonUtility.ToJson(sendData);

			Debug.Log("sendmessage:" + checkMessage);
			_netClient.SendMessage(checkMessage);
		} else if (_netClient.status == false && _netClient.isRetry == true)
		{  // 재연결 시도중
			reconnectPopup.SetActive(true);
		} else if (_netClient.status == false && _netClient.isRetry == false)
		{  // 재연결 실패
			netClientErrorCanvas.SetActive(true);
			isReconnectPopup.SetActive(false);
			failReconnectPopup.SetActive(true);
		}
	}

	// 큐를 주기적으로 탐색
	public IEnumerator CheckQueue()
	{
		// 1초 주기로 탐색
		WaitForSeconds waitSec = new WaitForSeconds(1);

		int count = 0;

		while (count < maxLoopCount)
		{
			count++;
			if (_netClient.status == true)
			{    // 정상 연결
				if (netClientErrorCanvas.activeSelf == true)
				{  // NetClientError 화면이 활성화된 경우
					netClientErrorCanvas.SetActive(false);
					isReconnectPopup.SetActive(false);
					failReconnectPopup.SetActive(false);
					yield break;
				}

				if (checkPrintResult.Length > 0)
				{  // 프린터 체크 데이터가 있는 경우
				   // 스크립트 별 동작 구성
					if (printStatus.Equals("disable"))
					{     // 프린터 상태 에러
						if (canvas.Equals("select"))
						{
							failCheckPrint();
						} else if (canvas.Equals("send"))
						{
							sendUIScript.FailCheckPrint();
						} else if (canvas.Equals("payment"))
						{
							paymentUIScript.FailCheckPrint();
						} else if (canvas.Equals("end"))
						{
							endUIScript.FailCheckPrint();
						}
					} else if (printStatus.Equals("proceed"))
					{     // 진행중
						if (canvas.Equals("end"))
						{
							endUIScript.PrintInProgress();
						} else if (canvas.Equals("select"))
						{
							PrintInProgress();
						}
					} else
					{     // 정상
						if (canvas.Equals("select"))
						{
							bPhotoable = true;
						} else if (canvas.Equals("send"))
						{
							sendUIScript.SuccessCheckPrint();
						} else if (canvas.Equals("payment"))
						{
							paymentUIScript.SuccessCheckPrint();
						} else if (canvas.Equals("end"))
						{
							endUIScript.SuccessCheckPrint();
						}
					}

					canvas = "";
					printStatus = "";
					checkPrintResult = "";
					yield break;
				} else if (paymentResult.Length > 0)
				{  // 결제데이터가 있는 경우
				   // 스크립트 별 동작 구성
					if (paymentProcess)
					{     // 성공, 진행중
						if (canvas.Equals("payment"))
						{
							paymentUIScript.Success();
						} else if (canvas.Equals("agreement"))
						{
							agreementUIScript.SuccessCancel();
						}
					} else
					{     // 실패
						if (canvas.Equals("payment"))
						{
							paymentUIScript.Fail();
						} else if (canvas.Equals("agreement"))
						{
							agreementUIScript.FailCancel();
						}
					}

					canvas = "";
					paymentResult = "";
					yield break;
				}
			} else if (_netClient.status == false && _netClient.isRetry == true)
			{  // 재연결 시도중
				if (canvas.Equals("payment"))
				{ // 결제 화면인 경우
				  // UI 재구성
					paymentUIScript.SetLoadingProgress(false);
					paymentUIScript.SetPayGuide(0);
					paymentUIScript.SetLoadingGuide(4);
				} else if (canvas.Equals("agreement"))
				{
					// UI 재구성
					agreementUIScript.SetLoadingProgress(false);
					agreementUIScript.SetLoadingGuide(3);
				}

				netClientErrorCanvas.SetActive(true);
				isReconnectPopup.SetActive(true);
				failReconnectPopup.SetActive(false);
			} else if (_netClient.status == false && _netClient.isRetry == false)
			{ // 재연결 실패
				if (canvas.Equals("payment"))
				{ // 결제 화면인 경우
				  // UI 재구성
					paymentUIScript.SetLoadingProgress(false);
					paymentUIScript.SetPayGuide(0);
					paymentUIScript.SetLoadingGuide(4);
				} else if (canvas.Equals("agreement"))
				{    // 약관 동의 화면인 경우
				     // UI 재구성
					agreementUIScript.SetLoadingProgress(false);
					agreementUIScript.SetLoadingGuide(3);
				}

				netClientErrorCanvas.SetActive(true);
				isReconnectPopup.SetActive(false);
				failReconnectPopup.SetActive(true);
			}

			yield return waitSec;
		}
	}

	private void ReceiveMessage(string message)
	{
		Debug.Log("ReceiveMessage:" + message);

		PaymentResponse response = JsonUtility.FromJson<PaymentResponse>(message);
		PrintStatusResponse printResponse = JsonUtility.FromJson<PrintStatusResponse>(message);

		if (sendType.Equals("check_print"))
		{   // 프린터 상태 체크
			checkPrintResult = printResponse.Result;
			if (checkPrintResult.Equals("0") || checkPrintResult.Equals("1"))
			{ // 사용 가능
				printStatus = "able";
			} else if (checkPrintResult.Equals("2"))
			{  // 진행중
				printStatus = "proceed";
			} else
			{    // 불가능
				printStatus = "disable";
			}
		} else
		{    // 결제 관련
			paymentResult = response.Result;
			if (response.Result.Equals("True"))
			{
				System.DateTime CurrentTime = System.DateTime.Now;

				paymentProcess = true;
				_approvalNum = response.ApprovalNum;
				_approvalDate = CurrentTime.ToString("yyyy-MM-dd HH:mm:ss");
				_price = response.Price;
				_paymentDate = response.ApprovalDate;

				Debug.Log("approval_num:" + response.Result);
				Debug.Log("approval_num:" + response.ApprovalNum);
				Debug.Log("approval_time:" + response.ApprovalDate);
				Debug.Log("price:" + response.Price);
			} else
			{
				paymentProcess = false;
				_approvalNum = "";
				_approvalDate = "";
				_price = "";
			}
		}
	}

	public void failCheckPrint()
	{
		Debug.Log("fail check select canvas");

		// 화면 전환을 위한 오디오 중지
		StartCoroutine(UtilsScript.stopAudio(audioKr));
		StartCoroutine(UtilsScript.stopAudio(audioEn));

		// 관리자 문의 안내 멘트 출력
		StartCoroutine(UtilsScript.playAudio(counselAudioKr, counselAudioEn));

		// 에러 팝업 활성화
		printErrorPopup.SetActive(true);
	}

	public void PrintInProgress()
	{
		Debug.Log("print in progress select canvas");

		// 에러 팝업 활성화
		printProgressPopup.SetActive(true);
	}

	public void Dispose()
	{
		print("DisposeSelect");
		isSelectCanvas = false;

		// 화면 전환을 위한 오디오 중지
		StartCoroutine(UtilsScript.stopAudio(audioKr));
		StartCoroutine(UtilsScript.stopAudio(audioEn));
		StartCoroutine(UtilsScript.stopAudio(counselAudioKr));
		StartCoroutine(UtilsScript.stopAudio(counselAudioEn));
	}

	public void StartPhoto(int movieNumber)
	{
		if (bPhotoable)
		{   // 촬영 가능한 경우
			if (isCameraConnected() == true)
			{
				FlowController.instance.currentMovieNumber = movieNumber;
				FlowController.instance.currentMovieId = downManager.jsonData.movieInfo[movieNumber].ID;
				FlowController.instance.ChangeFlow(FlowController.instance.photoCanvas);
			} else
			{
				cameraErrorPopup.SetActive(true);
			}
		} else
		{
			checkPrint();
		}
	}

	private bool isCameraConnected()
	{
		WebCamDevice[] devices = WebCamTexture.devices;
		return devices.Length > 0;
	}

	// 프린트 에러 버튼 클릭 이벤트 (참조가 없지만 버튼에 연결되어 있음)
	public void cancelPopup (GameObject popup)
	{
		StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));
		popup.SetActive(false);
		FlowController.instance.ChangeFlow(FlowController.instance.introCanvas);
	}
}

