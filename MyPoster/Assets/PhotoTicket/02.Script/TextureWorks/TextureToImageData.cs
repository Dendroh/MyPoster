// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
//  Author      sg.ju@alcherainc.com
//
// ---------------------------------------------------------------------------
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Alchera
{
    /// <summary>
    /// 텍스쳐를 Alchera_Plugin에 맞는 ImageData 형식으로 변경해주는 클래스 
    /// </summary>
    public class TextureToImageData : MonoBehaviour, ITextureConverter
    {
        // This will hold the reference. 
        // So that buffer won't get collected by GC
        Color32[] pixelBuffer;
        unsafe UInt64 GetBufferStart(Color32[] pixels)
        {
            fixed (Color32* ptr = &pixels[0])
                return (UInt64)ptr;
        }
        /**
        텍스쳐를 가져와 ImageData를 생성합니다.
        Task 형식으로 비동기 처리를 하며, 즉 매 프레임마다 이미지 데이터가 들어온다는 보장이 없습니다.
        
        @param texture 웹캠으로부터 받아온 텍스쳐
        @returns ImageData 가 세팅 완료되면 Task를 반환
         */
        Task<ImageData> ITextureConverter.ConvertAsync(Texture texture)
        {
            var source = new TaskCompletionSource<ImageData>();

            if (texture is WebCamTexture)
                StartCoroutine(ConvertWebcamInMainThread(texture as WebCamTexture, source));
            else if (texture is Texture2D)
                throw new ArgumentException(
                    "Cropped Texture2D process not Implemented"
                );
            else
                throw new ArgumentException(
                    "Can't convert given texture type. Available: WebCamTexture, Texture2D"
                );
            return source.Task;
        }

        IEnumerator ConvertWebcamInMainThread(WebCamTexture webcam, TaskCompletionSource<ImageData> promise) // will be deprecated
        {
            if (pixelBuffer == null || pixelBuffer.Length != webcam.width * webcam.height)
            {
                pixelBuffer = new Color32[webcam.width * webcam.height];
                print($"WebcamTexture: {webcam.width} {webcam.height}");
            }
            webcam.GetPixels32(pixelBuffer);

            ImageData image = default(ImageData);
            image.Data = GetBufferStart(pixelBuffer);
            image.WebcamWidth = (UInt16)webcam.width;
            image.WebcamHeight = (UInt16)webcam.height;
            image.DetectionWidth = (UInt16)webcam.width;
            image.DetectionHeight = (UInt16)webcam.height;
            image.OffsetX = 0;
            image.OffsetY = 0;

            image.Degree = (UInt16)(ReadWebcam.instance.GetAdjustedVideoRotationAngle()); // 방향에 따라 내부에서 이미지를 다르게 처리한다. LandscapeLeft:0 Protrait:90 LandscapeRight,Editor:180 

            promise.SetResult(image);
            yield break;
        }
     }
}