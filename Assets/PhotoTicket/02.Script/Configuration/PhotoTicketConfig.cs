using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Runtime.InteropServices;
using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

public class PhotoTicketConfig : MonoBehaviour
{
	//public string param1 = "여기에 저장된 값은 프로그램 내내 유지됩니다.";
	public string param2 = "여기에 저장된 값은 프로그램 내내 유지됩니다.";
	public string param3 = "여기에 저장된 값은 프로그램 내내 유지됩니다.";
	public string param4 = "여기에 저장된 값은 프로그램 내내 유지됩니다.";
	public string param5 = "여기에 저장된 값은 프로그램 내내 유지됩니다.";
	public string param6 = "여기에 저장된 값은 프로그램 내내 유지됩니다.";
	public string param7 = "true";  // autoReturn
	public string param8 = "true";  // 포토카드 재출력
	public string param9 = "kr";    // 언어선택
	public string param10 = "true"; // 상시 음성 안내
	public string param11 = "true"; // 운영 음성 안내
	public string param12 = "true"; // QR Code 사용 여부
	public string param13 = "true"; // Marketing 동의 사용 여부
	public string param14 = "true"; // 퀴즈 모드 설정
	public string param15 = "true"; // 프로모션 사용 여부
	public string param16 = "trre"; // Mirror 사용 여부

	//InputField param1input;
	InputField param2input;
	InputField param3input;
	InputField param4input;
	InputField param5input;
	InputField param6input;

	[SerializeField] Toggle trueAutoReturn;
	[SerializeField] Toggle falseAutoReturn;
	[SerializeField] Toggle trueRePrint;
	[SerializeField] Toggle falseRePrint;
	[SerializeField] Toggle langKr;
	[SerializeField] Toggle langEn;
	[SerializeField] Toggle trueIntroAudio;
	[SerializeField] Toggle falseIntroAudio;
	[SerializeField] Toggle trueOperateAudio;
	[SerializeField] Toggle falseOperateAudio;
	[SerializeField] Toggle trueQRCode;
	[SerializeField] Toggle falseQRCode;
	[SerializeField] Toggle trueMarketing;
	[SerializeField] Toggle falseMarketing;
	[SerializeField] Toggle trueQuiz;
	[SerializeField] Toggle falseQuiz;
	[SerializeField] Toggle truePromotion;
	[SerializeField] Toggle falsePromotion;
	[SerializeField] Toggle trueMirror;
	[SerializeField] Toggle falseMirror;

	[SerializeField] GameObject waitPopup;

	private bool isClicked = false;
	private bool isStarted = false;

	// Start is called before the first frame update
	void Start()
	{
		DontDestroyOnLoad(this);
		param2input = GameObject.Find("Input2").GetComponent<InputField>();
		param3input = GameObject.Find("Input3").GetComponent<InputField>();
		param4input = GameObject.Find("Input4").GetComponent<InputField>();
		param5input = GameObject.Find("Input5").GetComponent<InputField>();

		/*
		param1input.onValueChange.AddListener((s) =>
		{
		    param1 = s;
		});
		*/
		param2input.onValueChanged.AddListener((s) =>
		{
			param2 = s;
		});
		param3input.onValueChanged.AddListener((s) =>
		{
			param3 = s;
		});
		param4input.onValueChanged.AddListener((s) =>
		{
			param4 = s;
		});
		param5input.onValueChanged.AddListener((s) =>
		{
			param5 = s;
		});


		string configKIOSK = PlayerPrefs.GetString("kiosk_id");
		string configPaymode = PlayerPrefs.GetString("pay_mode");
		string configSite = PlayerPrefs.GetString("site_id");
		string configAutoReturn = PlayerPrefs.GetString("autoReturn");
		string configRePrint = PlayerPrefs.GetString("rePrint");
		string configLang = PlayerPrefs.GetString("lang");
		string configIntroAudio = PlayerPrefs.GetString("introAudio");
		string configOperateAudio = PlayerPrefs.GetString("operateAudio");
		string configQRCode = PlayerPrefs.GetString("qrCode");
		string configMarketingAgree = PlayerPrefs.GetString("marketingAgree");
		string configQuiz = PlayerPrefs.GetString("quiz");
		string configPromotion = PlayerPrefs.GetString("promotion");
		string configMirror = PlayerPrefs.GetString("mirror");
		string configPasscode = PlayerPrefs.GetString("passcode");

		// ini 값 저장 저장
		param7 = configAutoReturn;
		param8 = configRePrint;
		param9 = configLang;
		param10 = configIntroAudio;
		param11 = configOperateAudio;
		param12 = configQRCode;
		param13 = configMarketingAgree;
		param14 = configQuiz;
		param15 = configPromotion;
		param16 = configMirror;
		param5input.text = configPasscode;

		if (configSite != null && !configSite.Equals(""))
		{   // Site
			param2input.text = Regex.Replace(configSite, @"\s", "");
		} else
		{
			param2input.text = "11";
		}

		if (configKIOSK != null && !configKIOSK.Equals(""))
		{ // Kiosk
			param3input.text = Regex.Replace(configKIOSK, @"\s", "");
		} else
		{
			param3input.text = "1";
		}

		if (configPaymode != null && !configPaymode.Equals(""))
		{ // Paymode
			param4input.text = Regex.Replace(configPaymode, @"\s", "");
		} else
		{
			param4input.text = "n";
		}

		if (configAutoReturn != null && !configAutoReturn.Equals(""))
		{   // Auto Return
			param7 = Regex.Replace(configAutoReturn, @"\s", "");
		} else
		{
			param7 = "true";
		}

		if (configRePrint != null && !configRePrint.Equals(""))
		{  // Reprint
			param8 = Regex.Replace(configRePrint, @"\s", "");
		} else
		{
			param8 = "true";
		}

		if (configLang != null && !configLang.Equals(""))
		{   // Lang
			param9 = Regex.Replace(configLang, @"\s", "");
		} else
		{
			param9 = "kr";
		}

		if (configIntroAudio != null && !configIntroAudio.Equals(""))
		{   // IntroAudio
			param10 = Regex.Replace(configIntroAudio, @"\s", "");
		} else
		{
			param10 = "true";
		}

		if (configOperateAudio != null && !configOperateAudio.Equals(""))
		{   // OperateAudio
			param11 = Regex.Replace(configOperateAudio, @"\s", "");
		} else
		{
			param11 = "true";
		}

		if (configQRCode != null && !configQRCode.Equals(""))
		{   // QRCode
			param12 = Regex.Replace(configQRCode, @"\s", "");
		} else
		{
			param12 = "true";
		}

		if (configMarketingAgree != null && !configMarketingAgree.Equals(""))
		{   // Marketing Agree
			param13 = Regex.Replace(configMarketingAgree, @"\s", "");
		} else
		{
			param13 = "true";
		}

		if (configQuiz != null && !configQuiz.Equals(""))
		{   // Quiz
			param14 = Regex.Replace(configQuiz, @"\s", "");
		} else
		{
			param14 = "true";
		}

		if (configPromotion != null && !configPromotion.Equals(""))
		{   // QRCode
			param15 = Regex.Replace(configPromotion, @"\s", "");
		} else
		{
			param15 = "true";
		}

		if (configMirror != null && !configMirror.Equals(""))
		{
			param16 = Regex.Replace(configMirror, @"\s", "");
		} else
		{
			param16 = "true";
		}

		setToggle(configAutoReturn, trueAutoReturn, falseAutoReturn); // Auto Return
		setToggle(configRePrint, trueRePrint, falseRePrint);  // 포토카드 재출력
		setToggle(configIntroAudio, trueIntroAudio, falseIntroAudio); // 상시 음성
		setToggle(configOperateAudio, trueOperateAudio, falseOperateAudio);   // 운영 음성
		setToggle(configQRCode, trueQRCode, falseQRCode); // QR Code 사용 여부
		setToggle(configMarketingAgree, trueMarketing, falseMarketing); // Marketing Agree 사용 여부
		setToggle(configQuiz, trueQuiz, falseQuiz); // Quiz 사용 여부
		setToggle(configPromotion, truePromotion, falsePromotion); // Promotion 사용 여부
		setToggle(configMirror, trueMirror, falseMirror); // Mirror 사용 여부

		// 언어 선택
		if (configLang != null && !configLang.Equals(""))
		{
			if (configLang.Equals("kr"))
			{
				langKr.isOn = true;
			} else if (configLang.Equals("en"))
			{
				langEn.isOn = true;
			}
		} else
		{
			configLang = "kr";
		}


		trueAutoReturn.onValueChanged.AddListener(delegate
		{
			toggleChange(trueAutoReturn, ref param7);
		});
		trueRePrint.onValueChanged.AddListener(delegate
		{
			toggleChange(trueRePrint, ref param8);
		});
		langKr.onValueChanged.AddListener(delegate
		{
			langChange(langKr, ref param9);
		});
		trueIntroAudio.onValueChanged.AddListener(delegate
		{
			toggleChange(trueIntroAudio, ref param10);
		});
		trueOperateAudio.onValueChanged.AddListener(delegate
		{
			toggleChange(trueOperateAudio, ref param11);
		});
		trueQRCode.onValueChanged.AddListener(delegate
		{
			toggleChange(trueQRCode, ref param12);
		});
		trueMarketing.onValueChanged.AddListener(delegate
		{
			toggleChange(trueMarketing, ref param13);
		});
		trueQuiz.onValueChanged.AddListener(delegate
		{
			toggleChange(trueQuiz, ref param14);
		});
		truePromotion.onValueChanged.AddListener(delegate
		{
			toggleChange(truePromotion, ref param15);
		});
		trueMirror.onValueChanged.AddListener(delegate
		{
			toggleChange(trueMirror, ref param16);
		});
		StartCoroutine(CheckUserInput());
	}
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			isClicked = true;
		}

		if(Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			StartPhotoTicket();
		}
	}

	private IEnumerator CheckUserInput()
	{
		while (true)
		{
			if (isClicked)
			{
				isClicked = false;
			} else
			{
				yield return new WaitForSeconds(30);
				if (isStarted == false)
				{
					StartPhotoTicket();
				}
			}
		}
	}

	public void setToggle(string toggleName, Toggle trueToggle, Toggle falseToggle)
	{
		if (toggleName != null && !toggleName.Equals(""))
		{
			if (toggleName.Equals("true"))
			{
				trueToggle.isOn = true;
			} else if (toggleName.Equals("false"))
			{
				falseToggle.isOn = true;
			}
		} else
		{
			toggleName = "true";
		}
	}

	public void toggleChange(Toggle toggle, ref string param)
	{
		if (toggle.isOn)
		{
			param = "true";
			Debug.Log("true");
		} else
		{
			param = "false";
			Debug.Log("false");
		}
	}

	public void langChange(Toggle toggle, ref string param)
	{
		if (toggle.isOn)
		{
			param = "kr";
		} else
		{
			param = "en";
		}
	}

	bool IsAllInputFilled()
	{
		//if (param1input.text == "") return false;
		if (param2input.text == "") return false;
		if (param3input.text == "") return false;
		if (param4input.text == "") return false;
		if (param5input.text == "") return false;
		// if (param6input.text == "") return false;
		if (param7 == "") return false;
		if (param8 == "") return false;
		if (param9 == "") return false;
		if (param10 == "") return false;
		if (param11 == "") return false;
		if (param12 == "") return false;
		if (param13 == "") return false;
		if (param14 == "") return false;
		if (param15 == "") return false;
		if (param16 == "") return false;
		return true;
	}

	public void StartPhotoTicket()
	{
		isStarted = true;

		if (IsAllInputFilled() == false) //입력창이 공란으로 남아있을 경우
		{
			Debug.LogError("모든 값을 입력하세요");
			//에러 창 띄우기
			ErrorManager.instance.PopUpError("모든 값을 입력하세요!", false);
			return;
		}

		//string comport = param1input.text;
		string siteId = Regex.Replace(param2input.text, @"\s", "");
		string kioskId = Regex.Replace(param3input.text, @"\s", "");
		string paymode = Regex.Replace(param4input.text, @"\s", "");
		string autoReturn = param7;
		string rePrint = param8;
		string lang = param9;
		string introAudio = param10;
		string operateAudio = param11;
		string qrCode = param12;
		string marketingAgree = param13;
		string quiz = param14;
		string promotion = param15;
		string mirror = param16;
		string passcode = Regex.Replace(param5input.text, @"\s", "");


		PlayerPrefs.SetString("site_id", siteId);
		PlayerPrefs.SetString("kiosk_id", kioskId);
		PlayerPrefs.SetString("pay_mode", paymode);
		PlayerPrefs.SetString("passcode", passcode);
		PlayerPrefs.SetString("autoReturn", autoReturn);
		PlayerPrefs.SetString("rePrint", rePrint);
		PlayerPrefs.SetString("lang", lang);
		PlayerPrefs.SetString("introAudio", introAudio);
		PlayerPrefs.SetString("operateAudio", operateAudio);
		PlayerPrefs.SetString("qrCode", qrCode);
		PlayerPrefs.SetString("marketingAgree", marketingAgree);
		PlayerPrefs.SetString("quiz", quiz);
		PlayerPrefs.SetString("promotion", promotion);
		PlayerPrefs.SetString("mirror", mirror);

		// 대기팝업 실행
		waitPopup.SetActive(true);

		SceneManager.LoadScene(1);
	}
}
