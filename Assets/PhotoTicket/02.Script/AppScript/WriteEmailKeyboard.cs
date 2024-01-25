using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class WriteEmailKeyboard : MonoBehaviour
{
	public Text emailAddressText;
	[SerializeField] GameObject keyboardPad;
	[SerializeField] Button sendButton;
	[SerializeField] AudioSource clickAudio;
	Unity.VectorGraphics.SVGImage planeMark;
	Text sendText;
	Button[] keyboardButtons;
	bool bshift;
	bool bspecial;

	void Awake()
	{
		keyboardButtons = keyboardPad.GetComponentsInChildren<Button>();
		// planeMark = sendButton.GetComponentInChildren<SVGImage>();
		sendText = sendButton.GetComponentInChildren<Text>();

		Init();
	}

	void Start()
	{
		// 숫자
		for (int i = 0; i < 10; i++)
		{
			int asciiNum = i;

			keyboardButtons[i].onClick.RemoveAllListeners();
			keyboardButtons[i].onClick.AddListener(() => { WriteNumber((char)asciiNum); });
		}

		keyboardButtons[10].onClick.RemoveAllListeners();
		keyboardButtons[10].onClick.AddListener(() => { erase(); });

		// 영문자
		for (int i = 11; i < 37; i++)
		{
			int asciiNum = 97 + i - 11;

			keyboardButtons[i].onClick.AddListener(() => { WriteChar((char)asciiNum); });
		}

		keyboardButtons[37].onClick.AddListener(() => { special(); });  // 특수문자변환
		keyboardButtons[38].onClick.AddListener(() => { shift(); });    // Shift

		// 특수문자 and .com
		for (int i = 39; i < 42; i++)
		{
			int index = i;  // 루프 참조에 의해 i값을 직접 사용 시, 루프 종료 조건 값이 할당됨
			keyboardButtons[index].onClick.AddListener(() => { WriteSpecial(keyboardButtons[index].GetComponentInChildren<Text>().text); });
		}

		// Space Bar
		keyboardButtons[42].onClick.AddListener(() => { WriteSpecial(" "); });
	}

	public void Init()
	{
		emailAddressText.text = "";
		SetSendButton(false);
	}

	void WriteNumber(int numberButtonIndex)
	{
		if (UtilsScript.checkConfig() != "")
		{
			clickAudio.Play();    // 버튼 효과음 출력
		}

		emailAddressText.text += numberButtonIndex.ToString();

		if (IsValidEmail(emailAddressText.text))
		{
			SetSendButton(true);
		} else
		{
			SetSendButton(false);
		}
	}

	void WriteChar(char key)
	{
		if (UtilsScript.checkConfig() != "")
		{
			clickAudio.Play();    // 버튼 효과음 출력
		}

		emailAddressText.text += key.ToString();

		if (IsValidEmail(emailAddressText.text))
		{
			SetSendButton(true);
		} else
		{
			SetSendButton(false);
		}
	}

	void WriteSpecial(string special)
	{
		if (UtilsScript.checkConfig() != "")
		{
			clickAudio.Play();    // 버튼 효과음 출력
		}

		emailAddressText.text += special;

		if (IsValidEmail(emailAddressText.text))
		{
			SetSendButton(true);
		} else
		{
			SetSendButton(false);
		}
	}

	void erase()
	{
		if (UtilsScript.checkConfig() != "")
		{
			clickAudio.Play();    // 버튼 효과음 출력
		}

		if (emailAddressText.text.Length > 0)
		{
			emailAddressText.text = emailAddressText.text.Remove(emailAddressText.text.Length - 1);
		}

		if (IsValidEmail(emailAddressText.text))
		{
			SetSendButton(true);
		} else
		{
			SetSendButton(false);
		}
	}

	void shift()
	{
		if (UtilsScript.checkConfig() != "")
		{
			clickAudio.Play();    // 버튼 효과음 출력
		}

		if (!bshift)
		{
			for (int i = 11; i < 37; i++)
			{
				int asciiNum = 65 + i - 11;

				keyboardButtons[i].onClick.RemoveAllListeners();
				keyboardButtons[i].onClick.AddListener(() => { WriteChar((char)(asciiNum)); });
				keyboardButtons[i].gameObject.transform.GetChild(0).GetComponent<Text>().text = ((char)(asciiNum)).ToString();
			}

			bshift = true;
		} else
		{
			for (int i = 11; i < 37; i++)
			{
				int asciiNum = 97 + i - 11;

				keyboardButtons[i].onClick.RemoveAllListeners();
				keyboardButtons[i].onClick.AddListener(() => { WriteChar((char)(asciiNum)); });
				keyboardButtons[i].gameObject.transform.GetChild(0).GetComponent<Text>().text = ((char)(asciiNum)).ToString();
			}

			bshift = false;
		}
	}

	void special()
	{
		if (UtilsScript.checkConfig() != "")
		{
			clickAudio.Play();    // 버튼 효과음 출력
		}

		if (!bspecial)
		{
			for (int i = 0; i < 10; i++)
			{
				keyboardButtons[i].onClick.RemoveAllListeners();
			}

			keyboardButtons[0].onClick.AddListener(() => { WriteSpecial("!"); });
			keyboardButtons[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = "!";
			keyboardButtons[1].onClick.AddListener(() => { WriteSpecial("@"); });
			keyboardButtons[1].gameObject.transform.GetChild(0).GetComponent<Text>().text = "@";
			keyboardButtons[2].onClick.AddListener(() => { WriteSpecial("#"); });
			keyboardButtons[2].gameObject.transform.GetChild(0).GetComponent<Text>().text = "#";
			keyboardButtons[3].onClick.AddListener(() => { WriteSpecial("$"); });
			keyboardButtons[3].gameObject.transform.GetChild(0).GetComponent<Text>().text = "$";
			keyboardButtons[4].onClick.AddListener(() => { WriteSpecial("%"); });
			keyboardButtons[4].gameObject.transform.GetChild(0).GetComponent<Text>().text = "%";
			keyboardButtons[5].onClick.AddListener(() => { WriteSpecial("^"); });
			keyboardButtons[5].gameObject.transform.GetChild(0).GetComponent<Text>().text = "^";
			keyboardButtons[6].onClick.AddListener(() => { WriteSpecial("&"); });
			keyboardButtons[6].gameObject.transform.GetChild(0).GetComponent<Text>().text = "&";
			keyboardButtons[7].onClick.AddListener(() => { WriteSpecial("."); });
			keyboardButtons[7].gameObject.transform.GetChild(0).GetComponent<Text>().text = ".";
			keyboardButtons[8].onClick.AddListener(() => { WriteSpecial("-"); });
			keyboardButtons[8].gameObject.transform.GetChild(0).GetComponent<Text>().text = "-";
			keyboardButtons[9].onClick.AddListener(() => { WriteSpecial("_"); });
			keyboardButtons[9].gameObject.transform.GetChild(0).GetComponent<Text>().text = "_";

			keyboardButtons[37].gameObject.transform.GetChild(0).GetComponent<Text>().text = "숫";

			bspecial = true;
		} else
		{
			for (int i = 0; i < 10; i++)
			{
				int asciiNum = i;

				keyboardButtons[i].onClick.RemoveAllListeners();
				keyboardButtons[i].onClick.AddListener(() => { WriteNumber((char)asciiNum); });
				keyboardButtons[i].gameObject.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
			}

			keyboardButtons[37].gameObject.transform.GetChild(0).GetComponent<Text>().text = "특";

			bspecial = false;
		}
	}

	public void SetSendButton(bool flag)
	{
		sendText = sendButton.GetComponentInChildren<Text>();
		planeMark = sendButton.GetComponentInChildren<Unity.VectorGraphics.SVGImage>();
		if (flag)
		{
			sendButton.interactable = true;
			// planeMark.color = Color.white;
			sendText.color = Color.white;
		} else
		{
			sendButton.interactable = false;
			// planeMark.color = new Color(0.4f, 0.4f, 0.4f, 0.5f);
			sendText.color = new Color(0.4f, 0.4f, 0.4f, 0.5f);
		}
	}

	public bool IsValidEmail(string email)
	{
		bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
		return valid;
	}
}
