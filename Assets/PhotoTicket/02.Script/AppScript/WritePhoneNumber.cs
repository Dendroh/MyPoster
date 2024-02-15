using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WritePhoneNumber : MonoBehaviour
{
	public Text phoneNumberText;
	[SerializeField] GameObject numberPad;
	[SerializeField] Button sendButton;
	[SerializeField] AudioSource clickAudio;
	// SVGImage planeMark;
	Text sendText;
	Button[] numberButtons;

	public string phoneNumber;

	void Awake()
	{
		numberButtons = numberPad.GetComponentsInChildren<Button>();
		// planeMark = sendButton.GetComponentInChildren<SVGImage>();
		sendText = sendButton.GetComponentInChildren<Text>();
	}

	void Start()
	{
		numberButtons[0].onClick.AddListener(() => { WriteNumber(0); });
		numberButtons[1].onClick.AddListener(() => { WriteNumber(1); });
		numberButtons[2].onClick.AddListener(() => { WriteNumber(2); });
		numberButtons[3].onClick.AddListener(() => { WriteNumber(3); });
		numberButtons[4].onClick.AddListener(() => { WriteNumber(4); });
		numberButtons[5].onClick.AddListener(() => { WriteNumber(5); });
		numberButtons[6].onClick.AddListener(() => { WriteNumber(6); });
		numberButtons[7].onClick.AddListener(() => { WriteNumber(7); });
		numberButtons[8].onClick.AddListener(() => { WriteNumber(8); });
		numberButtons[9].onClick.AddListener(() => { WriteNumber(9); });
		numberButtons[10].onClick.AddListener(() => { EraseNumber(); });
	}
	public void Init()
	{
		phoneNumberText.text = "010 - ";
		phoneNumber = "010 - ";

		SetSendButton(false);
	}

	public void WriteNumber(int numberButtonIndex)
	{
		if (UtilsScript.checkConfig() != "")
		{
			clickAudio.Play();    // 버튼 효과음 출력
		}

		if (phoneNumberText.text.Length > 16)
			return;

		SetSendButton(phoneNumberText.text.Length == 16 ? true : false);

		if (phoneNumberText.text.Length == 10)
		{
			phoneNumberText.text += " - ";
			phoneNumber += " - ";
		}

		if (phoneNumberText.text.Length == 6)
		{
			phoneNumberText.text += numberButtonIndex.ToString();
		} else if (phoneNumberText.text.Length == 13)
		{
			phoneNumberText.text = phoneNumberText.text.Remove(phoneNumberText.text.Length - 4) + "* - " + numberButtonIndex.ToString();

		} else
		{
			phoneNumberText.text = phoneNumberText.text.Remove(phoneNumberText.text.Length - 1) + "*" + numberButtonIndex.ToString();
		}

		phoneNumber += numberButtonIndex.ToString();
	}

	public void EraseNumber()
	{
		if (UtilsScript.checkConfig() != "")
		{
			clickAudio.Play();    // 버튼 효과음 출력
		}

		if (phoneNumberText.text.Length > 6)
		{
			phoneNumberText.text = phoneNumberText.text.Remove(phoneNumberText.text.Length - 2) + phoneNumber[phoneNumber.Length - 2];
			phoneNumber = phoneNumber.Remove(phoneNumber.Length - 1);
		}

		if (phoneNumberText.text.Length == 13)
		{
			phoneNumberText.text = phoneNumberText.text.Remove(phoneNumberText.text.Length - 4) + phoneNumber[phoneNumber.Length - 4];
			phoneNumber = phoneNumber.Remove(phoneNumber.Length - 3);
		}

		SetSendButton(false);
	}

	public void SetSendButton(bool flag)
	{
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
}