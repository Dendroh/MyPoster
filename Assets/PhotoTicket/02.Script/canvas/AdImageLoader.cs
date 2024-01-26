using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class AdsInfo
{
	public string type;
	public string position;
	public string picture;
	public string startd;
	public string endd;
	public string startt;
	public string endt;
}


public class AdImageLoader : MonoBehaviour
{
	public string adInfoURL;
	public RawImage AdImage;
	public static bool adComplete = false;

	void Start()
	{
		string siteId = PlayerPrefs.GetString("site_id");

		if (siteId.Length > 0)
		{
			adInfoURL = ConstantsScript.OPERATE_URL + "/site/" + siteId + "/ad/app_list";
			Debug.Log("Ad Image URL" + adInfoURL);
			// 1초후에 1시간 간격으로 광고 이미지 변경 루틴 수행
			InvokeRepeating("AdImageDownload", 1, 1 * 60 * 60);
		}
	}

	void AdImageDownload()
	{
		StartCoroutine(GetRequest(adInfoURL));
	}

	IEnumerator DownloadImage(string MediaUrl)
	{
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
			Debug.Log(request.error);
		else
		{
			Debug.Log("Ad Image Download Success");

			AdImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
		}
	}

	IEnumerator GetRequest(string uri)
	{
		using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
		{
			// Request and wait for the desired page.
			yield return webRequest.SendWebRequest();

			if (webRequest.result == UnityWebRequest.Result.ConnectionError)
			{
				Debug.Log("Error: " + webRequest.error);
			} else
			{
				Debug.Log("Ad Information API Call Success");

				Debug.Log("Received: " + webRequest.downloadHandler.text);

				string jsonString = webRequest.downloadHandler.text;

				if (jsonString != null && jsonString.Length > 0)
				{
					AdsInfo[] data = JsonHelper.FromJson<AdsInfo>(jsonString);

					if (data != null && data.Length > 0)
					{
						string imgURL = "";
						foreach (var ad in data)
						{
							Debug.Log(ad.picture);
							imgURL = ad.picture;
						}

						StartCoroutine(DownloadImage(imgURL));
					}
				}
			}

			adComplete = true;
		}
	}
}
