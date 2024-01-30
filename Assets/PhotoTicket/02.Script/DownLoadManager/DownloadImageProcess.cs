using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DownloadImageProcess : MonoBehaviour
{
	MovieDownManager movieDownManager;

	string url;
	string savePath;

	void Start()
	{
		movieDownManager = GetComponent<MovieDownManager>();

		url = ConstantsScript.OPERATE_URL + "/files/";
		savePath = Application.persistentDataPath + "/Assetbundles/";

		if (!Directory.Exists(savePath))
			Directory.CreateDirectory(savePath);
	}

	public IEnumerator DownloadPosterImageCoroutine(string siteId, string assetName, string assetSize)
	{
		string downloadPath = url + siteId + "/assetbundles/";

		FileInfo fileInfo = new FileInfo(savePath + assetName);

		if (!fileInfo.Exists || fileInfo.Length != (assetSize != null && assetSize != "" ? long.Parse(assetSize) : 0))
		{
			UnityWebRequest webRequest = UnityWebRequest.Get(downloadPath + assetName);
			webRequest.downloadHandler = new DownloadHandlerFile(savePath + assetName);
			yield return webRequest.SendWebRequest();

			if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
			{
				Debug.LogError($"Error downloading {assetName}: {webRequest.error}");
				// 오류 처리
			} else
			{
				// 다운로드 완료 처리
				movieDownManager.posterProgress++;
			}
		}
	}


	public IEnumerator DownloadStickerImageCoroutine(string siteId, string assetName, string assetSize)
	{
		string downloadPath = url + siteId + "/assetbundles/";

		FileInfo fileInfo = new FileInfo(savePath + assetName);

		if (!fileInfo.Exists || fileInfo.Length != (assetSize != null && assetSize != "" ? long.Parse(assetSize) : 0))
		{
			UnityWebRequest webRequest = UnityWebRequest.Get(downloadPath + assetName);
			webRequest.downloadHandler = new DownloadHandlerFile(savePath + assetName);
			yield return webRequest.SendWebRequest();

			if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
			{
				Debug.LogError($"Error downloading {assetName}: {webRequest.error}");
				FlowController.instance.ChangeFlow(FlowController.instance.networkCheckCanvas);
				// 오류 처리
			} else
			{
				// 다운로드 완료 처리
				movieDownManager.posterProgress++;
			}
		}
	}

	public IEnumerator DownloadStickerImage(String siteId, string assetName, string assetSize)
	{
		string downloadPath = url + siteId + "/assetbundles/";

		FileInfo fileInfo = new FileInfo(savePath + assetName);

		if (!fileInfo.Exists || fileInfo.Length != (assetSize != null && assetSize != "" ? long.Parse(assetSize) : 0))
		{
			UnityWebRequest webRequest = UnityWebRequest.Get(downloadPath + assetName);
			webRequest.downloadHandler = new DownloadHandlerFile(savePath + assetName);
			yield return webRequest.SendWebRequest();

			if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
			{
				Debug.LogError($"Error downloading {assetName}: {webRequest.error}");
				FlowController.instance.ChangeFlow(FlowController.instance.networkCheckCanvas);
				// 오류 처리
			} else
			{
				// 다운로드 완료 처리
				movieDownManager.stickerProgress++;
			}
		}
	}

	public void FileDownLoad(string url, string savePath)
	{
		WebClient webClient = new WebClient();

		webClient.DownloadFileAsync(new Uri(url), savePath);
	}
}
