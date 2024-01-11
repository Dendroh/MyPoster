// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
//  Author      sg.ju@alcherainc.com
//
// ---------------------------------------------------------------------------
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace Alchera {
    public class AutoBackgroundQuad : MonoBehaviour {
        [SerializeField] Material chromarkeyWebcamMaterial = null;
        [SerializeField] Material normalWebcamMaterial = null;
        [SerializeField] Texture blackTexture = null;
        [HideInInspector] public Texture texture;
        WaitForSeconds ws = new WaitForSeconds(0.5f);

        public static bool bChromarkey = false;
        public static bool updateQuad = false;

        void Start() {
            chromarkeyWebcamMaterial.mainTexture = blackTexture;
            normalWebcamMaterial.mainTexture = blackTexture;
            texture = new Texture2D(16, 16);
        }

        public static void SetQuadSize(GameObject quad, Texture texture) {
            var zScale = Mathf.Tan(Mathf.Deg2Rad * (Camera.main.fieldOfView / 2.0f)) * quad.transform.localPosition.z * 2.0f;

            float w = texture.width;
            float h = texture.height;
            float screenRatio = (float)Screen.width / Screen.height;
            if (screenRatio > 1) {
                if (screenRatio > (w / h)) {
                    float scaleRatio = screenRatio * h / w;
                    quad.transform.localScale = new Vector3(zScale * (w / h) * scaleRatio, zScale * scaleRatio, 1);
                } else
                    quad.transform.localScale = new Vector3(zScale * (w / h), zScale, 1);
            } else {
                if (screenRatio > (h / w)) {
                    float scaleRatio = screenRatio * w / h;
                    quad.transform.localScale = new Vector3(zScale * scaleRatio, zScale * scaleRatio * (h / w), 1);
                } else
                    quad.transform.localScale = new Vector3(zScale, zScale * (h / w), 1);
            }

            quad.transform.localRotation = Quaternion.Euler(0, 0, -90f); // 이미지를 좌로 90도 회전
            quad.transform.localScale = new Vector3(-quad.transform.localScale.x, quad.transform.localScale.y, quad.transform.localScale.z); //좌우반전
        }

        public void SetQuadMaterial(bool flag) {
            chromarkeyWebcamMaterial.mainTexture = flag ? texture : blackTexture;
            normalWebcamMaterial.mainTexture = flag ? texture : blackTexture;
        }

        public IEnumerator UpdateQuad() {
            texture = new Texture2D(16, 16);

            while (texture.width < 20) {
                var request = (ReadWebcam.instance as ITextureSequence).CaptureAsync();
                yield return request;
                texture = request.Result;
                yield return ws;
            }
            SetQuadMaterial(true);
            updateQuad = true;
        }

        void Update() {
            if (texture.width < 20)
                return;

            SetQuadSize();
            SetQuadMirror();
            SetQuadRotation();

        }
        void SetQuadSize() {
            var zScale = Mathf.Tan(Mathf.Deg2Rad * (Camera.main.fieldOfView / 2.0f)) * transform.localPosition.z * 2.0f;

            float w = texture.width;
            float h = texture.height;
            float screenRatio = (float)Screen.width / Screen.height;
            if (screenRatio > 1) // width > height
            {
                if (screenRatio > (w / h)) {
                    float scaleRatio = screenRatio * h / w;
                    transform.localScale = new Vector3(zScale * (w / h) * scaleRatio, zScale * scaleRatio, 1);
                } else
                    transform.localScale = new Vector3(zScale * (w / h), zScale, 1);
            } else {
                if (screenRatio > (h / w)) {
                    float scaleRatio = screenRatio * w / h;
                    transform.localScale = new Vector3(zScale * scaleRatio, zScale * scaleRatio * (h / w), 1);
                } else
                    transform.localScale = new Vector3(zScale, zScale * (h / w), 1);
            }

        }
        void SetQuadMirror() {
            ReadWebcam.instance.GetMirrorValue(out int mirrorX, out int mirrorY);
            transform.localScale = new Vector3(mirrorX * transform.localScale.x, mirrorY * transform.localScale.y, 1);
        }

        void SetQuadRotation() {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, ReadWebcam.instance.GetAdjustedVideoRotationAngle()));
        }

        public IEnumerator UpdateFromTextureSequence() {
            yield return null;
            var ws = new WaitForSeconds(0.2f);
            texture = new Texture2D(16, 16);

            while (texture.width < 20) {
                var request = (ReadWebcam.instance as ITextureSequence).CaptureAsync();
                yield return request;
                texture = request.Result;

                yield return ws;
            }

            SetQuadMaterial(true);
        }
    }

}