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
using UnityEngine;
using UnityEngine.Profiling;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;
namespace Alchera
{
    public class FaceService : MonoBehaviour, IDetectService
    {
        private struct Translator : ITranslator
        {
            internal UInt16 maxCount;
            internal IFit3D fit3d;
            internal UInt16 count;
            internal FaceData[] storage;
            internal ProcParams fitParam;
            internal ProcCache[] cache;
            internal bool need3D;
            internal FaceLib.Context context;
            //internal Face3DLib.Context context3d;
            private AutoBackgroundQuad quad;

            internal unsafe void Init(int _maxCount, bool _need3D, int _levelOf3DProcess)
            {
                context = default(FaceLib.Context);
                context.trackMode = 1;
                context.trackFrameInterval = 10;
                context.maxCount = (UInt32)_maxCount;

                maxCount = (UInt16)_maxCount;
                storage = new FaceData[maxCount];
                cache = new ProcCache[maxCount];
                need3D = _need3D;

                quad = FindObjectOfType<AutoBackgroundQuad>();

                string str = Application.persistentDataPath + "/model";
                fixed (byte* path = context.modelPath.folderPath)
                {
                    for (int i = 0; i < str.Length; i++)
                    {
                        path[i] = (byte)str[i];
                    }
                }

                FaceLib.Init(ref context);

                if (need3D)
                {
                    fit3d = default(Face3DLib.Context);
                    fitParam.Raw[0] = 1;
                    for (var i = 0; i < maxCount; ++i)
                    {
                        cache[i].raw[0] = (UInt16)_levelOf3DProcess; // 0: headpose only, 1: compute params , 2: compute everything
                    }
                    fit3d.Init(ref fitParam);
                }
            }


            unsafe IEnumerable<T> ITranslator.Fetch<T>(IEnumerable<T> result)
            {
                // Only `FaceData` can be fetched
                if (typeof(T) != typeof(FaceData))
                    throw new NotSupportedException($"Available types: `{typeof(FaceData).FullName}`");

                // this code doesn't implement memory reusage
                Profiler.BeginSample("FaceService.Translator.Fetch");
                // apply fitting for each faces
                if (need3D)
                {
                    Face3DLib.Set(ref fitParam, Screen.width, Screen.height, Camera.main.fieldOfView);
                    float[] camMat = new float[6];
                    float v = 1.0f / (float)Math.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad) * Screen.height;
                    camMat[0] = v * 0.5f; camMat[1] = 0.0f; camMat[2] = Screen.width * 0.5f;
                    camMat[3] = 0; camMat[4] = v * 0.5f; camMat[5] = Screen.height * 0.5f;

                    float screenRatio = (float)Screen.width / Screen.height;
                    var height = quad.texture.height < 16 ? 1 : quad.texture.height; //divide by 0
                    float adjustment = quad.transform.localScale.y / height;

                    float centerX = quad.texture.width / 2;
                    float centerY = quad.texture.height / 2;
                    if (ReadWebcam.instance.GetAdjustedVideoRotationAngle() % 180 != 0) // width < height
                    {
                        centerX = quad.texture.height / 2;
                        centerY = quad.texture.width / 2;
                    }
                    ReadWebcam.instance.GetMirrorValue(out int mirrorX, out int mirrorY);
#if UNITY_EDITOR || UNITY_STANDALONE
#elif UNITY_IOS
                    mirrorX *= -1;  //iOS만 카메라가 반대 방향으로 들어온다.
#endif
                    fixed (FaceData* faces = storage)
                    {
                        for (int i = 0; i < count; ++i)
                        {
                            
                            for (int j = 0; j < FaceData.NumLandmark; ++j)
                            {
                                var posX = -mirrorX * (faces[i].Landmark[j].x - centerX) * adjustment;
                                var posY = -mirrorY * (faces[i].Landmark[j].y - centerY) * adjustment;
                                var posZ = quad.transform.localPosition.z;

                                Vector3 pos = new Vector3(posX, posY, posZ);

                                float rx = pos[0] * camMat[0] + pos[1] * camMat[1] + pos[2] * camMat[2];
                                float ry = pos[0] * camMat[3] + pos[1] * camMat[4] + pos[2] * camMat[5];
                                rx /= pos[2];
                                ry /= pos[2];

                                faces[i].Landmark[j].x = rx;
                                faces[i].Landmark[j].y = ry;
                            }
                            fit3d.Process((IntPtr)(faces + i), ref fitParam, ref cache[i]);
                        }
                    }
                }
                Profiler.EndSample();

                // segment of reserved storage
                return result = new ArraySegment<FaceData>(storage, 0, count)
                    as IEnumerable<FaceData>    // double casting. 
                    as IEnumerable<T>;          // usually this is bad 
            }

            void IDisposable.Dispose() { }
        }

        public int maxCount;
        public bool need3D;
        public int levelOf3DProcess;

        TaskCompletionSource<ITranslator> promise;
        Translator translator;

        public void Start()
        {
            try
            {
                translator.Init(maxCount, need3D, levelOf3DProcess);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        unsafe Task<ITranslator> IDetectService.DetectAsync(ref ImageData image)
        {
            Profiler.BeginSample("FaceService.DetectAsync");

            // too small image
            if (image.WebcamWidth < 40)
            {
                Debug.LogWarning("FaceService.DetectAsync: image is too small");
                goto DetectDone;
            }

            if (promise != null)
                promise.TrySetCanceled();
            // the number of detected faces
            fixed (FaceData* facePtr = translator.storage)
            {
                translator.count = (UInt16)FaceLib.Detect(ref translator.context, ref image, facePtr);
            }

        DetectDone:
            // return result in translator form
            promise = new TaskCompletionSource<ITranslator>();
            promise.SetResult(translator);

            Profiler.EndSample();
            return promise.Task;
        }

        void IDisposable.Dispose()
        {
            Debug.LogWarning("FaceService.Dispose");

            FaceLib.Release(ref translator.context);
            if (need3D)
            {
                translator.fit3d.Dispose();
            }
            if (promise == null)
                return;

            promise.TrySetCanceled();
            promise = null;
        }
    }

}
