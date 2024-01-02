using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

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

    public async void DownloadPosterImage(string siteId, string assetName, string assetSize) {
        string downloadPath = url + siteId + "/assetbundles/";

        FileInfo fileInfo = new FileInfo(savePath + assetName);

        try {
            // using (var fileStream = fileInfo.OpenRead()) {  // 파일 접근 권한 부여
                await Task.Run(() => {
                    // 파일 다운로드
                    if (!fileInfo.Exists || fileInfo.Length != (assetSize != null  && assetSize != "" ? long.Parse(assetSize) : 0)) {
                        FileDownLoad(downloadPath + assetName, savePath + assetName);
                    }
                    
                    movieDownManager.posterProgress++;
                });
            // }

            // using이 종료 후 파일 접근 권한 해제
        } catch (Exception e) { // 오류가 발생한 경우 오류 화면 전환
            FlowController.instance.ChangeFlow(FlowController.instance.networkCheckCanvas);
        }
    }

    public async void DownloadStickerImage(String siteId, string assetName, string assetSize) {
        string downloadPath = url + siteId + "/assetbundles/";

        FileInfo fileInfo = new FileInfo(savePath + assetName);

        try {
            // using (var fileStream = fileInfo.OpenRead()) {  // 파일 접근 권한 부여
                await Task.Run(() => {
                    if (!fileInfo.Exists || fileInfo.Length != (assetSize != null && assetSize != "" ? long.Parse(assetSize) : 0)) {
                        // 파일 다운로드
                        FileDownLoad(downloadPath + assetName, savePath + assetName);
                    }

                    movieDownManager.stickerProgress++;
                });
            // }

            // using이 종료 후 파일 접근 권한 해제
        } catch (Exception e) { // 오류가 발생한 경우 오류 화면 전환
            FlowController.instance.ChangeFlow(FlowController.instance.networkCheckCanvas);
        }
    }

    public void FileDownLoad(string url, string savePath)
    {
        WebClient webClient = new WebClient();

        webClient.DownloadFileAsync(new Uri(url), savePath);
    }
}
