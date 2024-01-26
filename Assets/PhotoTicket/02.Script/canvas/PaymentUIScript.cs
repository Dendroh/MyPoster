using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Unity.VectorGraphics;

public class PaymentUIScript : MonoBehaviour, UIScript
{
	[SerializeField] MovieDownManager downManager;
	[SerializeField] Text popupPriceText;
	[SerializeField] Text resultText;
	[SerializeField] Scrollbar productScroll;
	[SerializeField] Transform content;
	[SerializeField] SVGImage check;
	[SerializeField] SVGImage unCheck;
	[SerializeField] Image payGuide;
	[SerializeField] Image loadingGuide;
	[SerializeField] GameObject confirmPopup;
	[SerializeField] GameObject printErrorPopup;
	[SerializeField] GameObject chooseProductPopup;
	[SerializeField] GameObject loadingProgress;
	[SerializeField] Sprite[] payGuideSprites;
	[SerializeField] AudioSource guideAudioKr;
	[SerializeField] AudioSource guideAudioEn;
	[SerializeField] AudioSource selectProductAudioKr;
	[SerializeField] AudioSource selectProductAudioEn;
	[SerializeField] AudioSource checkAudioKr;
	[SerializeField] AudioSource checkAudioEn;
	[SerializeField] AudioSource paymentAudioKr;
	[SerializeField] AudioSource paymentAudioEn;
	[SerializeField] AudioSource counselAudioKr;
	[SerializeField] AudioSource counselAudioEn;
	[SerializeField] AudioSource buttonAudio;
	[SerializeField] GameObject priceObject;
	[SerializeField] SelectUIScript selectUIScript;

	RectTransform[] productList;
	RectTransform[] checkList;
	RectTransform defaultProduct;

	// 결제 종류 선택
	int PaymentType = 0;

	static int price = 0;
	string payment_name;
	public static bool bChooseProduct = false;
	public static bool bChoose = false;

	public static string printStatus;
	public static string checkPrintResult = "";
	public static string sendType = "";
	public static string canvas = "";
	public static bool paymentProcess;
	public static string paymentResult = "";
	private string _approvalNum = "";
	private string _approvalDate = "";  // 서버 전달 용 날짜
	private string _paymentDate = "";   // 결제 취소 용 날짜
	private string _price = "";

	//private string reqPayment = "{\"Command\":\"payment\",\"Amount\": {0}\"}";

	void Start()
	{

	}

	void Update()
	{
		if (bChoose)
		{
			bChoose = false;

			// 버튼 효과음 출력
			StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

			string priceStr = string.Format("{0:#,##0}", Int64.Parse(price.ToString()));
			resultText.text = priceStr + "원";
		}
	}

	public void setPriceProduct()
	{
		string siteId = PlayerPrefs.GetString("site_id");
		string productId = PlayerPrefs.GetString("productId");
		string url = ConstantsScript.OPERATE_URL + "/site/" + siteId + "/product/get_price_list/" + productId;

		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		HttpWebResponse response = (HttpWebResponse)request.GetResponse();

		Stream stream = response.GetResponseStream();
		StreamReader reader = new StreamReader(stream);

		string text = reader.ReadToEnd();

		JObject obj = JObject.Parse(text);

		JArray array = new JArray();

		string paymode = PlayerPrefs.GetString("pay_mode");
		if (paymode.Equals("r"))
		{
			array = JArray.Parse(obj["list"].ToString());   // 결제모드 r인 경우에만 상품 목록 사용
		} else
		{
			array.Add(obj["default_price"]);    // Default 상품 사용
		}

		productList = new RectTransform[array.Count];
		checkList = new RectTransform[array.Count];

		for (int i = 0; i < array.Count; i++)
		{
			// 상품 prefabs 설정
			productList[i] = Instantiate(priceObject, Vector3.zero, Quaternion.identity).GetComponent<RectTransform>();
			productList[i].transform.SetParent(content);

			productList[i].GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
			productList[i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1);

			if (paymode.Equals("r"))
			{
				productList[i].GetComponent<ProductController>().count = (int)array[i]["count"];
				productList[i].Find("Text").GetComponent<Text>().text = array[i]["name"].ToString();
				productList[i].Find("Price").GetComponent<Text>().text = array[i]["price"].ToString() + " 원";
			} else
			{
				productList[i].GetComponent<ProductController>().count = 0;
				productList[i].Find("Text").GetComponent<Text>().text = "마이포스터 다운로드";
				productList[i].Find("Price").GetComponent<Text>().text = obj["default_price"].ToString() + " 원";
			}

			// 버튼 이벤트 설정
			productList[i].GetComponent<ProductController>().id = i;
			productList[i].GetComponent<ProductController>().init();

			checkList[i] = productList[i].GetComponentsInChildren<RectTransform>()[3]; // 체크박스
			checkList[i].gameObject.SetActive(false);
		}

		// Scroll View Size 설정
		content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 170 * productList.Length);
	}

	public void confirm()
	{
		// 버튼 효과음 출력
		StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

		if (!bChooseProduct)
		{
			chooseProductPopup.SetActive(true);
			return;
		}

		// 화면 전환을 위한 오디오 중지
		StartCoroutine(UtilsScript.stopAudio(guideAudioKr));
		StartCoroutine(UtilsScript.stopAudio(guideAudioEn));
		StartCoroutine(UtilsScript.stopAudio(selectProductAudioKr));
		StartCoroutine(UtilsScript.stopAudio(selectProductAudioEn));

		if (PlayerPrefs.GetString("pay_mode").Equals("p") || price == 0)
		{
			SuccessCheckPrint();
		} else
		{
			selectUIScript.sendType = "check_print";
			selectUIScript.canvas = "payment";

			//큐 탐색 시작
			StartCoroutine(selectUIScript.CheckQueue());


			AgentSendData sendData = new AgentSendData();
			sendData.Command = "print_status";
			sendData.PrintCount = PlayerPrefs.GetString("count");

			string checkMessage = JsonUtility.ToJson(sendData);

			Debug.Log("SENDMESSAGE:" + checkMessage);
			SelectUIScript._netClient.SendMessage(checkMessage);
		}
	}

	public void cancelPopup(GameObject popup)
	{
		// 버튼 효과음 출력
		StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

		popup.SetActive(false);
	}

	public void clickProduct(int i, int count)
	{
		payment_name = productList[i].transform.Find("Text").GetComponent<Text>().text.ToString();
		price = Convert.ToInt32(Regex.Replace(productList[i].transform.Find("Price").GetComponent<Text>().text.ToString(), @"[^0-9]", ""));

		for (int j = 0; j < productList.Length; j++)
		{
			checkList[j].gameObject.SetActive(false);
		}

		// 체크 이미지 설정
		checkList[i].gameObject.SetActive(true);

		PlayerPrefs.SetString("name", payment_name);
		PlayerPrefs.SetString("price", price.ToString());
		PlayerPrefs.SetString("count", count.ToString());

		bChoose = true;
		bChooseProduct = true;
	}

	public void Init()
	{
		setPriceProduct();
		SetPayGuide(0);
		SetLoadingGuide(4);
		loadingProgress.SetActive(false);
		productScroll.value = 1;

		paymentResult = "";
		paymentProcess = false;
		print("IntroPayment");

		// 결제 정보 안내 멘트 출력
		if (PlayerPrefs.GetString("pay_mode").Equals("p"))
		{
			StartCoroutine(UtilsScript.playAudio(guideAudioKr, guideAudioEn));
		} else
		{
			StartCoroutine(UtilsScript.playAudio(selectProductAudioKr, selectProductAudioEn));
		}
	}

	public void Dispose()
	{
		SetLoadingGuide(4);
		PlayerPrefs.SetInt("WAS_FREE", 0);
		loadingProgress.SetActive(false);
		confirmPopup.SetActive(false);
		bChooseProduct = false;
		resultText.text = "0원";
		paymentProcess = false;

		for (int i = 0; i < productList.Length; i++)
		{  // prefab 중복 처리를 위한 오브젝트 제거
			Destroy(productList[i].gameObject);
		}

		print("DisposePayment");
	}

	public void GoToResult()
	{
		// 버튼 효과음 출력
		StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));
		FlowController.instance.ChangeFlow(FlowController.instance.resultCanvas);
	}

	public void PayMoney()
	{
		// 버튼 효과음 출력
		StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

		// 화면 전환을 위한 오디오 중지
		StartCoroutine(UtilsScript.stopAudio(checkAudioKr));
		StartCoroutine(UtilsScript.stopAudio(checkAudioEn));

		//결제 요청 시작
		if (price > 0)
		{
			Debug.Log("결제 진입 ! 결제할 금액 : " + price);

			// 카드 결제 안내 멘트 출력
			StartCoroutine(UtilsScript.playAudio(paymentAudioKr, paymentAudioEn));
			FlowController.instance.Loading(true);
			loadingProgress.SetActive(true);
			SetPayGuide(4);
			SetLoadingGuide(1);

			selectUIScript.sendType = "payment";
			selectUIScript.canvas = "payment";

			//큐 탐색 시작
			StartCoroutine(selectUIScript.CheckQueue());

			AgentSendData sendData = new AgentSendData();
			sendData.Command = "payment";
			sendData.Amount = price.ToString();

			string payMessage = JsonUtility.ToJson(sendData);

			Debug.Log("SENDMESSAGE:" + payMessage);
			SelectUIScript._netClient.SendMessage(payMessage);
		} else
		{
			FlowController.instance.ChangeFlow(FlowController.instance.AgreementCanvas);
		}
	}

	public void SetLoadingProgress(bool active)
	{
		loadingProgress.SetActive(active);
	}

	public void SetPayGuide(int i)
	{
		payGuide.sprite = payGuideSprites[i];
	}

	public void SetLoadingGuide(int i)
	{
		loadingGuide.sprite = payGuideSprites[i];
	}

	public void Success()
	{
		SetLoadingGuide(2);
		StartCoroutine(SuccessCoroutine());
	}

	public void SuccessCheckPrint()
	{
		Debug.Log("success check");
		StartCoroutine(UtilsScript.playAudio(checkAudioKr, checkAudioEn));
		printErrorPopup.SetActive(false);
		confirmPopup.SetActive(true);
		string priceStr = string.Format("{0:#,##0}", Int64.Parse(PlayerPrefs.GetString("price")));
		popupPriceText.text = "결제 금액\r\n" + PlayerPrefs.GetString("name") + " " + priceStr + "원";
	}

	public void Fail()
	{
		SetLoadingGuide(3);
		StartCoroutine(FailCoroutine());
	}

	public void FailCheckPrint()
	{
		Debug.Log("fail check");

		// 관리자 문의 안내 멘트 출력
		StartCoroutine(UtilsScript.playAudio(counselAudioKr, counselAudioEn));
		printErrorPopup.SetActive(true);
	}

	IEnumerator SuccessCoroutine()
	{
		//로딩바숨김
		Debug.Log("SuccessCoroutine:");

		PlayerPrefs.SetString("approval_num", _approvalNum);
		PlayerPrefs.SetString("approval_time", _approvalDate);
		// PlayerPrefs.SetString("price", _price);
		PlayerPrefs.SetString("price", price.ToString());
		PlayerPrefs.SetString("payment_time", _paymentDate);
		PlayerPrefs.SetInt("payment_type", PaymentType);

		Debug.Log("SuccessCoroutine : approval_num:" + _approvalNum);
		Debug.Log("SuccessCoroutine : approval_time:" + _approvalDate);
		Debug.Log("SuccessCoroutine : price:" + _price);

		loadingProgress.SetActive(false);
		yield return new WaitForSeconds(1.0f);

		PlayerPrefs.SetString("b_payment", "true");
		FlowController.instance.ChangeFlow(FlowController.instance.AgreementCanvas);
		yield break;
	}

	IEnumerator FailCoroutine()
	{
		//로딩바숨김
		Debug.Log("FailCoroutine:");
		loadingProgress.SetActive(false);
		yield return new WaitForSeconds(1.0f);
		FlowController.instance.Loading(false);
		SetPayGuide(0);
		SetLoadingGuide(4);
		yield break;
	}
}
