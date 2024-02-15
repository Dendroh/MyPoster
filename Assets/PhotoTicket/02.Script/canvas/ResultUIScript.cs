using System.Collections;
using UnityEngine;
using Alchera;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;

public class ResultUIScript : MonoBehaviour, UIScript
{
	ReadWebcamInSequence camReader;
	[SerializeField] MovieDownManager downManager;
	[SerializeField] GameObject cameraErrorPopup;
	[SerializeField] AudioSource sendAudioKr;
	[SerializeField] AudioSource sendAudioEn;
	[SerializeField] AudioSource buttonAudio;

	public static bool photoProgressFinished = false;

	void Start()
	{
		camReader = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ReadWebcamInSequence>();
	}

	void Update()
	{
		if (FlowController.instance.currentCanvas == FlowController.instance.resultCanvas)
		{
			if (Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				SendPoster();
			}

			if (Input.GetKeyDown(KeyCode.KeypadMinus))
			{
				RetakePhoto();
			}

			if (Input.GetKeyDown(KeyCode.KeypadMultiply))
			{
				GoToIntro();
			}
		}
	}

	public void Init()
	{
		StartCoroutine(StopCameraAfterSeconds(1));
		StartCoroutine(DelayedPlayAudio(2));
		if (ComplexSceneBehavior.photoCount < ComplexSceneBehavior.photoCountMax)
		{
			StartCoroutine(RetakePhotoAfterSeconds(1));
		} else
		{
			ComplexSceneBehavior.photoCount = 1;
		}
	}

	private IEnumerator DelayedPlayAudio(float delay)
	{
		yield return new WaitForSeconds(delay);
		StartCoroutine(UtilsScript.playAudio(sendAudioKr, sendAudioEn));
	}

	IEnumerator StopCameraAfterSeconds(float sec)
	{
		yield return new WaitForSeconds(sec);
		camReader.changeCameraState(false);
	}

	IEnumerator RetakePhotoAfterSeconds(float sec)
	{
		yield return new WaitForSeconds(sec);
		photoProgressFinished = false;
		if (isCameraConnected())
		{
			ComplexSceneBehavior.photoCount++;
			FlowController.instance.ChangeFlow(FlowController.instance.photoCanvas);
		} else
		{
			cameraErrorPopup.SetActive(true);
		}
	}

	public void Dispose()
	{
		PlayerPrefs.SetInt("WAS_FREE", 1);
		var detector = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ComplexSceneBehavior>();
		//detector.StopPlayVideo();
		print("DisposeResult");

		// 화면 전환을 위한 오디오 중지
		StartCoroutine(UtilsScript.stopAudio(sendAudioKr));
		StartCoroutine(UtilsScript.stopAudio(sendAudioEn));
	}

	public void SendPoster()
	{
		// 버튼 효과음 출력
		StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

		string paymode = PlayerPrefs.GetString("pay_mode");
		string siteId = PlayerPrefs.GetString("site_id");
		string productId = PlayerPrefs.GetString("productId");

		if (!IsCurrentMovieFree(FlowController.instance.currentMovieNumber))
		{  // 영화가 유료인 경우
			string url = ConstantsScript.OPERATE_URL + "/site/" + siteId + "/product/get_price_list/" + productId;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (Stream stream = response.GetResponseStream())
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						string text = reader.ReadToEnd();

						JObject obj = JObject.Parse(text);

						if (paymode.Equals("p"))
						{  // 결제모드 - p 유료 데이터 전송
							if (obj["default_price"] != null && Convert.ToInt32(Regex.Replace(obj["default_price"].ToString(), @"[^0-9]", "")) > 0)
							{
								GotoPayment();
							} else
							{
								GotoSend();
							}
						} else if (paymode.Equals("r"))
						{    // 결제모드 - r 유료 프린터
							JArray array = new JArray();
							array = JArray.Parse(obj["list"].ToString());
							if (array != null && array.Count > 0)
							{
								GotoPayment();
							} else
							{
								GotoSend();
							}
						} else
						{    // 결제모드 - n, t 무료
							GotoSend();
						}
					}
				}
			}
		} else
		{    // 영화가 무료인 경우
			GotoSend();
		}
	}

	bool IsCurrentMovieFree(int movieNumber)
	{
		if (downManager.jsonData.movieInfo[movieNumber].MovieIsFree)
			return true;
		else
			return false;
	}

	void GotoSend()
	{
		//FlowController.instance.ChangeFlow(FlowController.instance.sendCanvas);
		FlowController.instance.ChangeFlow(FlowController.instance.AgreementCanvas);
	}

	void GotoPayment()
	{
		FlowController.instance.ChangeFlow(FlowController.instance.paymentCanvas);
	}

	public void cancelPopup(GameObject popup)
	{
		popup.SetActive(false);
	}

	public void GoToIntro()
	{
		// 버튼 효과음 출력
		StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

		if (PlayerPrefs.GetString("quiz") == "true")
		{  // 퀴즈 모드인 경우
			FlowController.instance.ChangeFlow(FlowController.instance.introCanvas);
		} else
		{
			FlowController.instance.ChangeFlow(FlowController.instance.selectCanvas);
		}
	}

	public void RetakePhoto()
	{
		// 버튼 효과음 출력
		StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

		if (isCameraConnected())
		{
			FlowController.instance.ChangeFlow(FlowController.instance.photoCanvas);
		} else
		{
			cameraErrorPopup.SetActive(true);
		}
	}

	private bool isCameraConnected()
	{
		WebCamDevice[] devices = WebCamTexture.devices;
		return devices.Length > 0;
	}
}
