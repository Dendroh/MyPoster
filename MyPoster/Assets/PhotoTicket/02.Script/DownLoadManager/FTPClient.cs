using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System;
using System.Threading.Tasks;

class FTPClient : MonoBehaviour
{
	private string FTPID = "serapian";
	private string FTPPassword = "se1302";
	private string ServerAddress = "ftp://192.168.1.200/myposter/";
	MovieDownManager movieDownManager;
	public string S3BucketName = null;


	void Awake()
	{
	}


	void Start()
	{
		movieDownManager = GetComponent<MovieDownManager>();
	}


	public void downloadJSON(string assetName)
	{
		string SavePath = Application.persistentDataPath + "/" + assetName;
		string ftpUri = ServerAddress + assetName;
		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ftpUri));

		request.UsePassive = true;
		request.UseBinary = true;
		request.KeepAlive = true;

		if (!string.IsNullOrEmpty(FTPID) && !string.IsNullOrEmpty(FTPPassword))
		{
			request.Credentials = new NetworkCredential(FTPID, FTPPassword);
		}

		request.Method = WebRequestMethods.Ftp.DownloadFile;

		if (!string.IsNullOrEmpty(SavePath))
		{
			downloadAndSave(request.GetResponse(), SavePath);
		}

		movieDownManager.finish();
	}



	IEnumerator DownloadAssetProcessing(string assetName, int type)
	{

		string SavePath = Application.persistentDataPath + "/Assetbundles/" + assetName;
		// 192.168.1.200/mypoter/CGV/파일명
		string ftpUri = ServerAddress + "/" + movieDownManager.folderName + "/" + assetName;
		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ftpUri));
		print("DownloadStickerProcessing!!!!!!!!!! : ");
		request.UsePassive = true;
		request.UseBinary = true;
		request.KeepAlive = true;

		if (!string.IsNullOrEmpty(FTPID) && !string.IsNullOrEmpty(FTPPassword))
		{
			request.Credentials = new NetworkCredential(FTPID, FTPPassword);
		}

		request.Method = WebRequestMethods.Ftp.DownloadFile;

		if (!string.IsNullOrEmpty(SavePath))
		{
			downloadAndSave(request.GetResponse(), SavePath);
			print("download!!!!!!!!!! : ");

			progress(assetName, type);

			//StartCoroutine(progress(assetName,type));

		}


		//yield return new WaitForSeconds(0.5f);
		yield return null;

	}

	async Task<bool> FetchUsers(string assetName)
	{
		string SavePath = Application.persistentDataPath + "/Assetbundles/" + assetName;
		// 192.168.1.200/mypoter/CGV/파일명
		string ftpUri = ServerAddress + "/" + movieDownManager.folderName + "/" + assetName;
		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ftpUri));
		print("DownloadStickerProcessing!!!!!!!!!! : ");
		request.UsePassive = true;
		request.UseBinary = true;
		request.KeepAlive = true;

		if (!string.IsNullOrEmpty(FTPID) && !string.IsNullOrEmpty(FTPPassword))
		{
			request.Credentials = new NetworkCredential(FTPID, FTPPassword);
		}

		request.Method = WebRequestMethods.Ftp.DownloadFile;

		if (!string.IsNullOrEmpty(SavePath))
		{
			WebResponse req = await request.GetResponseAsync();
			Stream reader = req.GetResponseStream();

			//Create Directory if it does not exist
			if (!Directory.Exists(Path.GetDirectoryName(SavePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
			}

			FileStream fileStream = new FileStream(SavePath, FileMode.Create);


			int bytesRead = 0;
			byte[] buffer = new byte[2048];

			while (true)
			{
				bytesRead = reader.Read(buffer, 0, buffer.Length);

				if (bytesRead == 0)
					break;

				fileStream.Write(buffer, 0, bytesRead);
			}
			fileStream.Close();

			return true;
		}

		return false;
	}

	void progress(string assetName, int type)
	{
		// Poster Image
		if (type == 1)
		{
			movieDownManager.InitPoster(assetName);//instantiate 하는것 아님. 단순히 다운로드 완료 확인용
		}
		// Sticke
		else if (type == 2)
		{
			movieDownManager.InitSticker(assetName); //instantiate 하는것 아님. 단순히 다운로드 완료 확인용
		} else if (type == 3)
		{
			movieDownManager.InitImage(assetName);//instantiate 하는것 아님. 단순히 다운로드 완료 확인용
		}

		Debug.Log("progress : " + assetName);
		//yield return new WaitForSeconds(0.1f);  
	}

	void downloadAndSave(WebResponse request, string savePath)
	{
		Stream reader = request.GetResponseStream();

		//Create Directory if it does not exist
		if (!Directory.Exists(Path.GetDirectoryName(savePath)))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(savePath));
		}

		FileStream fileStream = new FileStream(savePath, FileMode.Create);


		int bytesRead = 0;
		byte[] buffer = new byte[2048];

		while (true)
		{
			bytesRead = reader.Read(buffer, 0, buffer.Length);

			if (bytesRead == 0)
				break;

			fileStream.Write(buffer, 0, bytesRead);
		}
		fileStream.Close();
	}
}
