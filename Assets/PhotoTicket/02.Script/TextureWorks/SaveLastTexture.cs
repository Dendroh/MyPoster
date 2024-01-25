// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
//  Author      dh.park@alcherainc.com
//
// ---------------------------------------------------------------------------
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Alchera
{
    /// <summary>
    /// 사진 촬영 시 raw_data jpg를 취득하는 클래스
    /// 
    /// 1280*720 데이터를 저장하며, 스티커가 포함된 jpg와 gif 는 Recorder.cs 에서 관리합니다. 
    /// </summary>
    public class SaveLastTexture : MonoBehaviour
    {
        Texture texture;
        
        string GenerateFilename()
        {
            var now = System.DateTime.Now;
            return String.Format("{0}.png",          // use time for png name
                         (UInt64)(now.TimeOfDay.TotalSeconds * 1000));
        }
        /**
       카메라상태를 변경합니다. flag를 이용해 카메라를 켜거나 끌 수 있습니다.
       @param tex 저장할 이미지의 Texture입니다. WebcamTexture를 받아옵니다.
       @param filename photo/${filename}.jpg 로 저장됩니다.
        */
        public Task SaveTexture(Texture tex,String filename)
        {
            texture = tex;
#if UNITY_EDITOR
            var SaveFolder = Application.dataPath; // Defaults to the asset folder in the editor for faster access to the gif file
#else
				//SaveFolder = Application.persistentDataPath;
                var SaveFolder = "photo";
#endif
            var path = SaveFolder + "/" + filename + ".jpg";
            if (!Directory.Exists(SaveFolder))
            {
                Directory.CreateDirectory(SaveFolder);
            }

            if (texture is Texture2D)
            {
                StartCoroutine(
                    WriteWithTexture2D(path, texture as Texture2D)
                );
                return Task.CompletedTask;
            }
            if (texture is WebCamTexture)
            {
                StartCoroutine(
                    WriteWithWebCamTexture(path, texture as WebCamTexture)
                );
                return Task.CompletedTask;
            }
            throw new NotSupportedException(
                "Given texture type is not supported");

        }

        IEnumerator WriteWithTexture2D(string path, Texture2D target)
        {
            // prepare image file
            var jpg = target.EncodeToJPG(75);
            //path = Path.Combine(path, GenerateFilename());
            // write to file
            var stream = File.OpenWrite(path);

            yield return stream.WriteAsync(jpg, 0, jpg.Length);
            stream.Close();

            // complete !
            Debug.Log(path);
        }

        IEnumerator WriteWithWebCamTexture(string path, WebCamTexture webcam)
        {
            // schedule in next frame
            yield return null;
            // this might be heavy for foreground thread, 
            //   but WebCamTexture require this
            var target = Texture2D.blackTexture;
            target.Reinitialize(webcam.width, webcam.height);
            target.SetPixels32(webcam.GetPixels32());

            // To provide responsive experience, 
            //   we can delegate the following work to thread pool.
            // But for code's simplicity, just suspend the work here
            //   and resume it in next frame.
            yield return null;
            target.Apply();

            StartCoroutine(
                WriteWithTexture2D(path, target)
            );
        }
        
    }

}
