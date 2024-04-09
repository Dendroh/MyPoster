using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Alchera;
using ZXing;
using ZXing.QrCode;

public class EndUIScript : MonoBehaviour, UIScript
{
	ReadWebcamInSequence camReader;
	[SerializeField] Text rePrintText;
	[SerializeField] RawImage fromJpgResult;
	[SerializeField] RawImage toJpgResult;
	[SerializeField] GameObject RePrint;
	[SerializeField] GameObject guide;
	[SerializeField] GameObject rePrintGuide;
	[SerializeField] GameObject printMessage;
	[SerializeField] GameObject receiveMessage;
	[SerializeField] GameObject contactMessage;
	[SerializeField] GameObject FirstBG;
	[SerializeField] GameObject NFirstBG;
	[SerializeField] GameObject printErrorPopup;
	[SerializeField] GameObject printInProgessPopup;
	[SerializeField] AudioSource dataAudioKr;
	[SerializeField] AudioSource dataAudioEn;
	[SerializeField] AudioSource printAudioKr;
	[SerializeField] AudioSource printAudioEn;
	[SerializeField] AudioSource rePrintAudioKr;
	[SerializeField] AudioSource rePrintAudioEn;
	[SerializeField] AudioSource buttonAudio;
	[SerializeField] AudioSource counselAudioKr;
	[SerializeField] AudioSource counselAudioEn;
	[SerializeField] int maxPrintCount;
	[SerializeField] RawImage qrCode;
	[SerializeField] SelectUIScript selectUIScript;

	int printCount = 0;

	void Start()
	{
		camReader = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ReadWebcamInSequence>();
	}

	public void Init()
	{
		print("InitEnd");
		FlowController.instance.Loading(false);
		toJpgResult.texture = fromJpgResult.texture;
		camReader.changeCameraState(false);
		printCount = 0;
		printErrorPopup.SetActive(false);
		printInProgessPopup.SetActive(false);

		PlayerPrefs.SetString("count", "0");
		string paymode = PlayerPrefs.GetString("pay_mode");
		string rePrint = PlayerPrefs.GetString("rePrint");
		string downUrl = PlayerPrefs.GetString("down_url");

		// QR Code 생성
		if (PlayerPrefs.GetString("sendType").Equals("qr"))
		{
			if (downUrl != null && !downUrl.Equals(""))
			{
				qrCode.gameObject.SetActive(true);

				Texture2D qr = addQR(downUrl);

				qrCode.texture = qr;
			} else
			{
				qrCode.gameObject.SetActive(false);
			}
		} else
		{
			qrCode.gameObject.SetActive(false);
		}

		if (paymode == "n" || paymode == "p")
		{
			// paymode - n, p 데이터 전송 모드(무/유료)
			StartCoroutine(UtilsScript.playAudio(dataAudioKr, dataAudioEn));
		} else if (paymode == "t" && rePrint == "true")
		{    // 카드 출력 모드(무료) 재출력 기능 사용
		     // UI 재구성
			NFirstBG.SetActive(false);
			FirstBG.SetActive(true);
			RePrint.SetActive(true);
			rePrintText.gameObject.SetActive(true);
			rePrintGuide.SetActive(true);
			printMessage.SetActive(true);
			receiveMessage.SetActive(false);
			contactMessage.SetActive(false);


			StartCoroutine(UtilsScript.playAudio(rePrintAudioKr, rePrintAudioEn));
		} else
		{    // 카드 출력 모드(무/유료) - 무료인 경우 재출력 기능 사용 X
			StartCoroutine(UtilsScript.playAudio(printAudioKr, printAudioEn));
		}
	}

	public void Dispose()
	{
		// 화면 전환을 위한 오디오 중지
		StartCoroutine(UtilsScript.stopAudio(dataAudioKr));
		StartCoroutine(UtilsScript.stopAudio(dataAudioEn));
		StartCoroutine(UtilsScript.stopAudio(printAudioKr));
		StartCoroutine(UtilsScript.stopAudio(printAudioEn));
		StartCoroutine(UtilsScript.stopAudio(rePrintAudioKr));
		StartCoroutine(UtilsScript.stopAudio(rePrintAudioEn));

		rePrintText.text = "";
	}

	public void RePrintPoster()
	{
		string filepath = "";

		if (Application.isEditor)
		{
			if (PlayerPrefs.GetString("printType") == "card")
			{
				filepath = Application.dataPath + "/0.jpg";
			} else
			{
				filepath = Application.dataPath + "/0_merged.jpg";
			}
		} else
		{
			if (PlayerPrefs.GetString("printType") == "card")
			{
				filepath = Path.GetFullPath(".") + "/photo/0.jpg";
			} else
			{
				filepath = Path.GetFullPath(".") + "/photo/0_merged.jpg";
			}
		}
		// 버튼 효과음 출력
		StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

		selectUIScript.sendType = "check_print";
		selectUIScript.canvas = "end";

		//큐 탐색 시작
		StartCoroutine(selectUIScript.CheckQueue());

		AgentSendData sendData = new AgentSendData();

		sendData.Command = "print";
		sendData.FilePath = filepath;
		sendData.PrintType = PlayerPrefs.GetString("printType");
		sendData.PrintCount = PlayerPrefs.GetString("count");
		if (PlayerPrefs.GetString("pay_mode").Equals("t"))
		{
			sendData.PrintCount = "1";
		}

		PlayerPrefs.SetString("count", "0");

		string payMessage = JsonUtility.ToJson(sendData);

		Debug.Log("SENDMESSAGE:" + payMessage);

		//출력 명령 전송
		SelectUIScript._netClient.SendMessage(payMessage);
	}

	private static Color32[] Encode(string textForEncoding, int width, int height)
	{
		// 인코딩 작업
		var writer = new BarcodeWriter
		{
			Format = BarcodeFormat.QR_CODE,
			Options = new QrCodeEncodingOptions
			{
				Height = height,
				Width = width
			}
		};

		// QRcode는 적외선 센서를 통하여 인식을 하게 되는데,
		// 인식을 하게 되면 해당 QRcode에 저장된 텍스트를 실행 시켜 그 내용을 확인 하는 것이다.
		// 이에 따라 QR 코드를 만들 때에는 QRcode 안에 저장할 텍스트와 함께 생성시킨다.

		return writer.Write(textForEncoding);
	}

	public static Texture2D addQR(string text)
	{
		// 인코딩 작업을 위한 Encode 함수 호출
		var encoded = new Texture2D(256, 256, TextureFormat.RGBA32, false);
		encoded.filterMode = FilterMode.Point;

		var color32 = Encode(text, encoded.width, encoded.height);

		encoded.SetPixels32(color32);
		encoded.Apply();

		return encoded;
	}

	public void cancelPopup(GameObject popup)
	{
		// 버튼 효과음 출력
		StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

		popup.SetActive(false);
	}

	public void GoToIntro()
	{
		// 화면 전환을 위한 오디오 중지
		StartCoroutine(UtilsScript.stopAudio(dataAudioKr));
		StartCoroutine(UtilsScript.stopAudio(dataAudioEn));
		StartCoroutine(UtilsScript.stopAudio(printAudioKr));
		StartCoroutine(UtilsScript.stopAudio(printAudioEn));
		StartCoroutine(UtilsScript.stopAudio(rePrintAudioKr));
		StartCoroutine(UtilsScript.stopAudio(rePrintAudioEn));

		// 버튼 효과음 출력
		StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

		// QR 코드 Url 초기화
		PlayerPrefs.SetString("down_url", "");

		FlowController.instance.ChangeFlow(FlowController.instance.introCanvas);
	}

	public void SuccessCheckPrint()
	{
		Debug.Log("success check");
		printErrorPopup.SetActive(false);

		printCount++;   // 재출력 할 때 카운트 증가
		if (printCount > maxPrintCount)
		{   // 재출력 횟수 제한
			rePrintText.text = "재출력 가능 매수 소진";  // 23.02.20 임시 텍스트 - 수정 필요
			return;
		}

		rePrintText.text = "추가 출력 매수 " + printCount + "장";

		string filepath = Application.dataPath + "/1.jpg";

		if (Application.isEditor)
		{
			filepath = Application.dataPath + "/1.jpg";
		} else
		{
			filepath = Path.GetFullPath(".") + "/photo/1.jpg";
		}

		AgentSendData sendData = new AgentSendData();

		sendData.Command = "print";
		sendData.FilePath = filepath;
		sendData.PrintType = PlayerPrefs.GetString("printType");
		sendData.PrintCount = "1";

		string payMessage = JsonUtility.ToJson(sendData);

		Debug.Log("SENDMESSAGE:" + payMessage);

		//출력 명령 전송
		SelectUIScript._netClient.SendMessage(payMessage);
	}

	public void FailCheckPrint()
	{
		Debug.Log("fail check");

		// 화면 전환을 위한 오디오 중지
		StartCoroutine(UtilsScript.stopAudio(rePrintAudioKr));
		StartCoroutine(UtilsScript.stopAudio(rePrintAudioEn));

		// 관리자 문의 안내 멘트 출력
		StartCoroutine(UtilsScript.playAudio(counselAudioKr, counselAudioEn));

		printErrorPopup.SetActive(true);
	}

	public void PrintInProgress()
	{
		Debug.Log("print in progress");

		// 화면 전환을 위한 오디오 중지
		StartCoroutine(UtilsScript.stopAudio(rePrintAudioKr));
		StartCoroutine(UtilsScript.stopAudio(rePrintAudioEn));

		printInProgessPopup.SetActive(true);
	}

	public void ReSendLink()
	{
		FlowController.instance.ChangeFlow(FlowController.instance.introCanvas);
	}
}