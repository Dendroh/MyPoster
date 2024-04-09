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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Alchera
{
    public class ReadImageFromDirectory :
        MonoBehaviour,
        ITextureSequence
    {
        IEnumerable<String> files;

        void Awake()
        {
            var rootPath = Application.persistentDataPath;
            var imageDirPath = Path.Combine(rootPath, "Images");
            Debug.Log(imageDirPath);

            files = Directory.EnumerateFiles(imageDirPath);
        }

        void IDisposable.Dispose()
        {
            files = null;
        }

        Task<Texture> ITextureSequence.CaptureAsync()
        {
            throw new NotSupportedException();
        }

        IEnumerable<Task<Texture>> ITextureSequence.RepeatAsync()
        {
            Texture2D texture = Texture2D.whiteTexture;

            foreach (string path in files)
            {
                if (path.Contains("jpg") == false)
                    continue;
                Debug.Log(path);

                var bytes = File.ReadAllBytes(path);
                if (bytes == null)
                {
                    Debug.LogError("File.ReadAllBytes: null");
                    yield break;
                }
                if (texture.LoadImage(bytes) == false)
                {
                    Debug.LogError("texture.LoadImage: false");
                    yield break;
                }

                var promise = new TaskCompletionSource<Texture>();
                promise.SetResult(texture);
                yield return promise.Task;
            }
        }
    }

}