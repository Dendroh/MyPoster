using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;

namespace AWSSDK.Examples
{
    public class S3Example : MonoBehaviour
    {
        MovieDownManager movieDownManager;
        //ap-northeast-2:90ca5488-902b-42fe-bd54-22fb43aa3018 thes
        //thes-myposter
        //ap-northeast-2:88de1787-9bfb-4cb4-96aa-e1655cf7ebe8 deprecated
        //phototicket-test
        public string IdentityPoolId = "";
        public string CognitoIdentityRegion = RegionEndpoint.USEast1.SystemName;
        private RegionEndpoint _CognitoIdentityRegion
        {
            get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
        }
        public string S3Region = RegionEndpoint.USEast1.SystemName;
        private RegionEndpoint _S3Region
        {
            get { return RegionEndpoint.GetBySystemName(S3Region); }
        }
        public string S3BucketName = null;
      
        void Awake()
        {
            UnityInitializer.AttachToGameObject(this.gameObject);
            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        }

        void Start()
        {
            movieDownManager = GetComponent<MovieDownManager>();
        }

        #region private members
        private IAmazonS3 _s3Client;
        private AWSCredentials _credentials;
        private AWSCredentials Credentials
        {
            get
            {
                if (_credentials == null)
                    _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
                return _credentials;
            }
        }
        private IAmazonS3 Client
        {
            get
            {
                if (_s3Client == null)
                {
                    _s3Client = new AmazonS3Client(Credentials, _S3Region);
                }
                //test comment
                return _s3Client;
            }
        }
        #endregion
        public void DownLoadJsonData(string fileName) //aaa.txt
        {
            Client.GetObjectAsync(S3BucketName, fileName, (token) =>
            {
                MakeFile(token, fileName);
                movieDownManager.finish();
            });
        }
        public void DownloadSticker(string assetName)
        {
            if (assetName == null)
                return;
            var assetPath = movieDownManager.folderName + "/" + assetName; //sheetName으로 폴더 구성되어있다고 가정
            Client.GetObjectAsync(S3BucketName, assetPath, (token) =>
            {
                MakeFile(token, "/Assetbundles/" + assetName);
                movieDownManager.InitSticker(assetName); //instantiate 하는것 아님. 단순히 다운로드 완료 확인용
            });
        }

        public void DownloadPoster(string assetName)
        {
            if (assetName == null)
                return;
            var assetPath = movieDownManager.folderName + "/" + assetName; //sheetName으로 폴더 구성되어있다고 가정
            Client.GetObjectAsync(S3BucketName, assetPath, (token) =>
            {
                MakeFile(token, "/Assetbundles/" + assetName);
                movieDownManager.InitPoster(assetName);//instantiate 하는것 아님. 단순히 다운로드 완료 확인용
            });
        }

        public void DownloadImage(string assetName)
        {
            if (assetName == null)
                return;

            var assetPath = movieDownManager.folderName + "/" + assetName; //sheetName으로 폴더 구성되어있다고 가정
            Client.GetObjectAsync(S3BucketName, assetPath, (token) =>
            {
                MakeFile(token, "/Assetbundles/" + assetName);
                movieDownManager.InitImage(assetName);//instantiate 하는것 아님. 단순히 다운로드 완료 확인용
            });
        }
        void MakeFile(AmazonServiceResult<GetObjectRequest,GetObjectResponse> token , string assetName)
        {
            if (token.Exception != null)
            {
                Debug.LogError("s3 token has an error " + token.Request.Key.Split('.')[0]);
                ErrorManager.instance.PopUpError(token.Exception.ToString().Split('-')[0] +"\n\n"+ token.Request.Key.Split('.')[0], true);
                return;
            }
            var downloadStream = token.Response.ResponseStream;
            var fs = File.Create(Application.persistentDataPath + "/" + assetName);
            byte[] buffer = new byte[4096 * 10];
            int count;
            while ((count = downloadStream.Read(buffer, 0, buffer.Length)) != 0)
                fs.Write(buffer, 0, count);
            fs.Flush();
            fs.Close();
        }
    }
}
