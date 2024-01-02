// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
// Author
//       Ju Sugang       | sg.ju @alcherainc.com
//
// ---------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Alchera
{
    /// <summary>
    /// Example script for Animoji features
    /// Normally, It is in the Face model Prefab
    /// </summary>
    public class AnimojiPrefab : MonoBehaviour, IFace3D
    {
        SkinnedMeshRenderer smr;
        Transform leftEye;
        Transform rightEye;
        AutoBackgroundQuad quad;
        Dictionary<string, int> blendIndices;

        public Pose HeadPose
        {
            set
            {
                var lerpRatio = 0.5f;   // smoother with moving average

                transform.localPosition
                     = Vector3.Lerp(transform.localPosition,
                                    value.position,
                                    lerpRatio);
                transform.localRotation
                     = Quaternion.Slerp(transform.localRotation,
                                        value.rotation,
                                        lerpRatio);
            }
        }

        string[] exprIndices = { "eyeBlinkRight",
                    "eyeBlinkLeft",
                    "jawOpen",
                    "mouthClose",
                    "mouthFunnel",
                    "mouthPucker",
                    "mouthLeft",
                    "mouthRight",
                    "mouthFrownRight",
                    "mouthShrugLower",
                    "mouthShrugUpper",
                    "mouthUpperUpLeft",
                    "mouthUpperUpRight",
                    "browDownLeft",
                    "browDownRight",
                    "browInnerUp"};

        void Start()
        {
            leftEye = transform.Find("LeftEye");
            rightEye = transform.Find("RightEye");

            //model.localScale = new Vector3(1, 1, -1) * 0.55f;

            //transform.localScale = new Vector3(1, -1, -1) * 0.55f;

            // Acquire renderer for blendshape
            smr = this.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr == null)
                throw new UnityException(string.Format(
                    "SkinnedMeshRenderer is missing: {0}",
                    this.gameObject.name));

            blendIndices = new Dictionary<string, int>();
            for (int i = 0; i < smr.sharedMesh.blendShapeCount; ++i)
            {
                var name = smr.sharedMesh.GetBlendShapeName(i);
                blendIndices.Add(name.ToLower(), i);
            }
        }

        public void SetEyeRotation(ref Quaternion left, ref Quaternion right)
        {
            if (leftEye == null || rightEye == null)
                return;
            leftEye.localRotation = left;
            rightEye.localRotation = right;
        }

        // Since weight index might be different for each fbx model
        // be cautious when implementing new AnimojiModel
        public unsafe void SetAnimation(float* weights)
        {
            if (blendIndices == null)
                return;
            for (int i = 0; i < exprIndices.Length; ++i)
            {
                string key = "PandaBlendshape." + exprIndices[i];
                float value = weights[i];

                //reproject eye weight
                float eye_shut_ratio = 3.0f;
                float eye_shut_start = 20.0f;
                if (i == 0 || i == 1)
                {
                    if (value > eye_shut_start)
                    {
                        value = (value - eye_shut_start) * eye_shut_ratio + eye_shut_start;
                    }
                }
                if (value < 0.0f) value = 0.0f;
                if (value > 100.0f) value = 100.0f;

                if (key.Length > 1)
                {
                    if (blendIndices.ContainsKey(key.ToLower()))
                    {
                        int idx = blendIndices[key.ToLower()];
                        smr.SetBlendShapeWeight(idx, value);
                    }
                    else
                    {
                        Debug.Log(key + " is not founded..");
                    }
                }
            }
        }

        public unsafe void UseFaceData(ref ImageData image, ref FaceData face)
        {
            if (smr == null) return;
            if (quad == null)
            {
                Debug.LogWarning("Finding WebCamQuad...");
                quad = FindObjectOfType<AutoBackgroundQuad>();
                return;
            }

            var pose = face.HeadPose;

            var height = quad.texture.height < 16 ? 1 : quad.texture.height; //divide by zero 방지 
            float adjustment = quad.transform.localScale.y / height;

            ReadWebcam.instance.GetMirrorValue(out int mirrorX, out int mirrorY);

#if UNITY_EDITOR || UNITY_STANDALONE
#elif UNITY_IOS
            mirrorY *= -1;  //iOS만 카메라가 반대 방향으로 들어온다.
#endif  
            var X = 1;
            var Y = -1;
            var Z = 1;
            var W = -1;
            mirrorX = 1;
            mirrorY = -1;

            pose.position = new Vector3(mirrorX * pose.position.x, mirrorY * pose.position.y, pose.position.z);
            pose.rotation = new Quaternion(X * pose.rotation.x, Y * pose.rotation.y, Z * pose.rotation.z, W * pose.rotation.w);
            HeadPose = pose;

            var tX = 1;
            var tY = -1;
            var tZ = -1;
            transform.localScale = new Vector3(tX, tY, tZ) * 0.55f;
            //face.GetEyeRotation(out left, out right);
            //SetEyeRotation(ref left, ref right);
            
            float* weights = stackalloc float[FaceData.NumAnimationWeights];
            face.GetAnimation(weights);
            SetAnimation(weights);
        }
    }

}
