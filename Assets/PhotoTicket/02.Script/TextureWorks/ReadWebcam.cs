﻿// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
//  Author      sg.ju@alcherainc.com
//
// ---------------------------------------------------------------------------
using UnityEngine;

namespace Alchera
{
    public class ReadWebcam : MonoBehaviour
    {
        public static ReadWebcam instance = null;
        public static bool bWebcam = false;

        protected WebCamDevice[] devices;
        [HideInInspector] public WebCamTexture current, front, rear;
        [HideInInspector] public bool isCameraFront;
        bool[] CamerasFR;
        int FRIndex = 0;
        [SerializeField] protected AutoBackgroundQuad chromarkeyQuad;
        [SerializeField] protected AutoBackgroundQuad normalQuad;
        void Awake()
        {
            instance = this;
        }
        void Start()
        {
            loadCamera();
        }

        public void loadCamera() {
            var RequestWidth = 1280; // 1920
            var RequestHeight = 720; // 1080
            var RequestedFPS = 300; //as high as the webcam hardware can support;
            devices = WebCamTexture.devices;

            CamerasFR = new bool[devices.Length];
            int i = 0;
            foreach (var device in devices)
            {
                print($"{device.name} {device.isFrontFacing}");
                if (front != null && rear != null) break; // 멀티카메라인 경우 일반 카메라가 첫번째 인덱스에 있다고 생각. 차후 수정필요

#if UNITY_EDITOR || UNITY_STANDALONE
                if (device.name == "Logitech HD Pro Webcam C920" || front == null)
                { //에디터 카메라는 front 이다.
                    front = new WebCamTexture(device.name, RequestWidth, RequestHeight, RequestedFPS);
                    CamerasFR[i] = true;
                    FRIndex = i;
                    isCameraFront = CamerasFR[i];
                    bWebcam = true;
                    current = front;
                    Debug.Log(front);
                    Debug.Log(current);
                    Debug.Log("현재 카메라 이름 : " + current.deviceName);
                }
                else
                {
                    rear = new WebCamTexture(device.name, RequestWidth, RequestHeight, RequestedFPS);
                    CamerasFR[i] = true;
                    bWebcam = false;
                }
#else
                if (device.isFrontFacing){
                    front = new WebCamTexture(device.name, RequestWidth, RequestHeight, RequestedFPS);
                    CamerasFR[i]= true;
                    FRIndex = i;
                    isCameraFront = CamerasFR[i];
                }
                else{
                    rear = new WebCamTexture(device.name,RequestWidth, RequestHeight, RequestedFPS);
                    CamerasFR[i] = false;
                }
#endif
                i++;
            }
            //current = (rear == null) ? front : rear;
        }

        protected bool GetNextCameraFR()
        {
            return CamerasFR[(++FRIndex) % 2];
        }
        public void GetMirrorValue(out int mirrorX, out int mirrorY)
        {
            mirrorX = 1;
            mirrorY = 1;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID
            mirrorY *= -1;  //iOS만 카메라가 반대 방향으로 들어온다.
#endif
            if (!isCameraFront)
            {
                if (GetAdjustedVideoRotationAngle() % 180 == 0)
                    mirrorX *= -1;
                else
                    mirrorY *= -1;
            }
        }
        public int GetAdjustedVideoRotationAngle()
        {
            return 90;
            //return (current.videoRotationAngle + 180) % 360;
        }
        protected void OnDestroy()
        {
            Debug.LogWarning("ReadWebcam.OnDestroy");
            current.Stop();
            if (front != null && front.isPlaying)
                front.Stop();
            if (rear != null && rear.isPlaying)
                rear.Stop();
        }
    }
}
