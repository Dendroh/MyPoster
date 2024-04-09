using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing.Rendering;
public class NumpadButton : MonoBehaviour
{
	Text number;

	void Start()
	{
		if (this.gameObject.name != ("Image (10)"))
		{
			number = GetComponentInChildren<Text>();
		}
	}
	//on clicked
	public void onDown()
	{
		if (this.gameObject.name != ("Image (10)"))
		{
			number.color = new Color32(174, 0, 0, 255);
		}
	}
	//on clickExited
	public void onExit()
	{
		if (this.gameObject.name != ("Image (10)"))
		{
			number.color = new Color32(255, 255, 255, 255);
		}
	}
}
