// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
// Author
//      Park DongHa     | dh.park@alcherainc.com
//
// ---------------------------------------------------------------------------
using System;
using UnityEngine;
using UnityEngine.UI;
namespace Alchera
{
    public class Glove3DPrefab : MonoBehaviour, IHand3D
    {

        GameObject handMesh;
        GameObject thumbMesh;
        GameObject indexMesh;
        GameObject middleMesh;
        GameObject ringMesh;
        GameObject pinkyMesh;

        public bool applyFilter;
        public float compensateRotate = 0.3f;
        public float clipMin = 0.5f;
        public float clipMax = 15.0f;
        //private Quaternion[] backupRotation = new Quaternion[14]; //2 + 3 * 4
        private Vector3[] FingerBases = new Vector3[5];

        int[] idxMap = { 4, 3, 2, 8, 7, 6, 5, 12, 11, 10, 9, 16, 15, 14, 13, 20, 19, 18, 17, 0 };
        int[] mapToOld = { 4, 3, 2, 8, 7, 6, 5, 12, 11, 10, 9, 16, 15, 14, 13, 20, 19, 18, 17, 0, 1 };
        int[] mapToNew = { 19, 20, 2, 1, 0, 6, 5, 4, 3, 10, 9, 8, 7, 14, 13, 12, 11, 18, 17, 16, 15 };
        private float localScale = 1.0f;
        private bool doNotUpdate = false;

        private Vector3 translate = new Vector3(0, 0, 0);
        private Quaternion rotation = Quaternion.identity;

        public float debug1 = 1;
        public float debug2 = 1;
        public float debug3 = 1;

        // from c++ fitting (inital model)
        private static readonly float[] initVec = {  0.311560561f, -0.378139969f,
        0.26285096f, -0.298559744f,
        0.202835937f, -0.191007633f,
        0.163441216f, -0.6539055f,
        0.157488798f, -0.572807644f,
        0.145406333f, -0.482005321f,
        0.114892361f, -0.334146422f,
        0.02794111f, -0.699347396f,
        0.03271877f, -0.61333311f,
        0.03462118f, -0.512970228f,
        0.02918697f, -0.337827247f,
        -0.07053763f, -0.658724774f,
        -0.067496951f, -0.577720795f,
        -0.061413552f, -0.48427292f,
        -0.04498599f, -0.326130163f,
        -0.18933333f, -0.536878493f,
        -0.177468236f, -0.47422291f,
        -0.158254187f, -0.40935126f,
        -0.119784053f, -0.296871547f,
        0, 0};

        void Start()
        {

            SetHandMesh();
            /*
            SaveLocalRotation(thumbMesh, 2, 0);
            SaveLocalRotation(indexMesh, 3, 2);
            SaveLocalRotation(middleMesh, 3, 5);
            SaveLocalRotation(ringMesh, 3, 8);
            SaveLocalRotation(pinkyMesh, 3, 11);
            */
        }
        /*
        void SaveLocalRotation(GameObject obj, int numLink, int offset)
        {
            Transform cur = obj.transform;
            for (int i = 0; i < numLink; ++i)
            {
                backupRotation[offset + i] = cur.localRotation;

                cur = cur.GetChild(0);
            }
        }
        */

        public void SetHandMesh()
        {
            handMesh = this.transform.Find("RightHand").gameObject;
            thumbMesh = handMesh.transform.Find("RightHandThumb1").gameObject;
            indexMesh = handMesh.transform.Find("RightHandIndex1").gameObject;
            middleMesh = handMesh.transform.Find("RightHandMiddle1").gameObject;
            ringMesh = handMesh.transform.Find("RightHandRing1").gameObject;
            pinkyMesh = handMesh.transform.Find("RightHandPinky1").gameObject;

            FingerBases[0] = thumbMesh.transform.localPosition;
            FingerBases[1] = indexMesh.transform.localPosition;
            FingerBases[2] = middleMesh.transform.localPosition;
            FingerBases[3] = ringMesh.transform.localPosition;
            FingerBases[4] = pinkyMesh.transform.localPosition;
        }

        float GetModelLength(int i)
        {
            return Mathf.Sqrt(initVec[i * 2] * initVec[i * 2] + initVec[i * 2 + 1] * initVec[i * 2 + 1]);
        }

        void SetScale()
        {
            float[] ratio = new float[5];
            ratio[0] = GetModelLength(2) / FingerBases[0].magnitude;
            ratio[1] = GetModelLength(6) / FingerBases[1].magnitude;
            ratio[2] = GetModelLength(10) / FingerBases[2].magnitude;
            ratio[3] = GetModelLength(14) / FingerBases[3].magnitude;
            ratio[4] = GetModelLength(18) / FingerBases[4].magnitude;

            //use 4.0 instead of 5.0 (Magic Number!)
            localScale = (ratio[0] + ratio[1] + ratio[2] + ratio[3] + ratio[4]) / 3.0f;
        }

        public static void JacobiEigenValVec(double[,] a, int maxsize, int n,
        double epsilon, out double[,] eigenval, out double[,] eigenvec)
        {
            int i, j, p, q, flag;
            double[,] d = new double[maxsize, maxsize];
            double[,] s = new double[maxsize, maxsize];
            double[,] s1 = new double[maxsize, maxsize];
            double[,] s1t = new double[maxsize, maxsize];
            double[,] temp = new double[maxsize, maxsize];
            double theta, max;
            //Initialization of matrix d and s
            for (i = 1; i <= n; i++)
            {
                for (j = 1; j <= n; j++)
                {
                    d[i, j] = a[i, j];
                    s[i, j] = 0.0;
                }
            }
            for (i = 1; i <= n; i++) s[i, i] = 1.0;
            do
            {
                flag = 0;
                //Find largest off-diagonal element
                i = 1;
                j = 2;
                max = Math.Abs(d[1, 2]);
                for (p = 1; p <= n; p++)
                {
                    for (q = 1; q <= n; q++)
                    {
                        if (p != q) //off diagonal element
                        {
                            if (max < Math.Abs(d[p, q]))
                            {
                                max = Math.Abs(d[p, q]);
                                i = p;
                                j = q;
                            }
                        }
                    }
                }
                if (d[i, i] == d[j, j])
                {
                    if (d[i, j] > 0) theta = Math.PI / 4.0;
                    else theta = -Math.PI / 4.0;
                }
                else
                {
                    theta = 0.5 * Math.Atan(2.0 * d[i, j] / (d[i, i] - d[j, j]));
                }
                //Construction of the matrix s1 and s1t
                for (p = 1; p <= n; p++)
                {
                    for (q = 1; q <= n; q++)
                    {
                        s1[p, q] = 0.0;
                        s1t[p, q] = 0.0;
                    }
                }
                for (p = 1; p <= n; p++)
                {
                    s1[p, p] = 1.0;
                    s1t[p, p] = 1.0;
                }
                s1[i, i] = Math.Cos(theta); s1[j, j] = s1[i, i];
                s1[j, i] = Math.Sin(theta); s1[i, j] = -s1[j, i];
                s1t[i, i] = s1[i, i]; s1t[j, j] = s1[j, j];
                s1t[i, j] = s1[j, i]; s1t[j, i] = s1[i, j];
                //Product of s1t and d
                for (i = 1; i <= n; i++)
                {
                    for (j = 1; j <= n; j++)
                    {
                        temp[i, j] = 0.0;
                        for (p = 1; p <= n; p++)
                            temp[i, j] += s1t[i, p] * d[p, j];
                    }
                }
                //Product of temp and s1: d = s1t * d * s1
                for (i = 1; i <= n; i++)
                {
                    for (j = 1; j <= n; j++)
                    {
                        d[i, j] = 0.0;
                        for (p = 1; p <= n; p++)
                            d[i, j] += temp[i, p] * s1[p, j];
                    }
                }
                //Product of s and s1: s = s*s1
                for (i = 1; i <= n; i++)
                {
                    for (j = 1; j <= n; j++)
                    {
                        temp[i, j] = 0.0;
                        for (p = 1; p <= n; p++)
                            temp[i, j] += s[i, p] * s1[p, j];
                    }
                }
                for (i = 1; i <= n; i++)
                {
                    for (j = 1; j <= n; j++)
                    {
                        s[i, j] = temp[i, j];
                    }
                }
                //check to see if d is a diagonal matrix
                for (i = 1; i <= n; i++)
                {
                    for (j = 1; j <= n; j++)
                    {
                        if (i != j)
                            if (Math.Abs(d[i, j]) > epsilon)
                                flag = 1;
                    }
                }

            } while (flag == 1);
            //copy results to output matrices
            eigenval = d;
            eigenvec = s;
        }

        void GetRotationTranslation(Vector3[] vecX, Vector3[] vecP)
        {
            Vector3 avgX = new Vector3(0, 0, 0);
            Vector3 avgP = new Vector3(0, 0, 0);

            for (int i = 0; i < vecX.Length; ++i)
            {
                avgX += vecX[i];
                avgP += vecP[i];
            }
            avgX /= vecX.Length;
            avgP /= vecP.Length;

            double[,] px = new double[3, 3];

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    px[i, j] = 0.0;
                }
            }

            for (int i = 0; i < vecX.Length; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        px[j, k] += vecP[i][j] * vecX[i][k];
                    }
                }
            }

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    px[i, j] = px[i, j] / vecX.Length - avgP[i] * avgX[j];
                }
            }

            double tr = px[0, 0] + px[1, 1] + px[2, 2];

            //since jacobieignevalvec is based on numerical recipe in C, which based on pascal code, we provide a matrix start from index 1
            double[,] Q = new double[5, 5];
            double[,] eigenValues = new double[5, 5];
            double[,] eigenVectors = new double[5, 5];

            //setup Q
            Q[1, 1] = tr;
            Q[1, 2] = Q[2, 1] = (px[1, 2] - px[2, 1]);  //A23
            Q[1, 3] = Q[3, 1] = (px[2, 0] - px[0, 2]);  //A31
            Q[1, 4] = Q[4, 1] = (px[0, 1] - px[1, 0]);  //A12

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    Q[i + 2, j + 2] = px[i, j] + px[j, i];
                    if (i == j)
                    {
                        Q[i + 2, j + 2] -= tr;
                    }
                }
            }

            JacobiEigenValVec(Q, 5, 4, 1.0e-5, out eigenValues, out eigenVectors);

            int maxIdx = 1;
            for (int i = 2; i <= 4; ++i)
            {
                if (eigenValues[i, i] > eigenValues[maxIdx, maxIdx])
                {
                    maxIdx = i;
                }
            }

            Vector3 computedTranslate = avgX - rotation * avgP;
            if (computedTranslate.z < clipMin || computedTranslate.z > clipMax)
            {
                //doNotUpdate = true;
                //return;
            }

            doNotUpdate = false;

            Vector4 nq = new Vector4((float)eigenVectors[1, maxIdx], (float)eigenVectors[2, maxIdx], (float)eigenVectors[3, maxIdx], (float)eigenVectors[4, maxIdx]);
            nq.Normalize();

            // float a = float.Parse(input1.text);
            // float b = float.Parse(input2.text);
            // float c = float.Parse(input5.text);
            // float d = float.Parse(input6.text);
            // if (a < 0)
            //     return;
            rotation = Quaternion.Lerp(rotation, new Quaternion(nq[1], nq[2], nq[3], nq[0]), 0.4f);
            translate = Vector3.Lerp(translate, avgX - rotation * avgP, 0.5f);
        }

        unsafe void GetHandTransform(Vector3* fittedPts, GameObject transformObj)
        {
            Vector3 basePt = fittedPts[19];

            Vector3[] vecX = new Vector3[6];
            Vector3[] vecP = new Vector3[6];

            //setup vector for computation
            vecX[0] = fittedPts[6];
            vecX[1] = fittedPts[10];
            vecX[2] = fittedPts[14];
            vecX[3] = fittedPts[18];
            vecX[4] = fittedPts[2];
            vecX[5] = fittedPts[19];

            vecP[0] = indexMesh.transform.localPosition;
            vecP[1] = middleMesh.transform.localPosition;
            vecP[2] = ringMesh.transform.localPosition;
            vecP[3] = pinkyMesh.transform.localPosition;
            vecP[4] = thumbMesh.transform.localPosition;
            vecP[5] = new Vector3(0, 0, 0);

            for (int i = 0; i < 6; ++i)
            {
                vecP[i] *= localScale;
                vecP[i].z = -vecP[i].z;
            }

            GetRotationTranslation(vecX, vecP);
            transformObj.transform.localScale = new Vector3(1, 1, 1) * localScale * debug3;
            transformObj.transform.localPosition = new Vector3(translate.x * debug1, translate.y * debug2, translate.z);
            transformObj.transform.localRotation = Quaternion.Lerp(transformObj.transform.localRotation, rotation, 0.5f);
        }

        void UpdateLink(Quaternion baseRot, ref Transform cur, ref Transform child, Vector3 curP, Vector3 childP, bool isBase)
        {
            //Find Rcu s.t. normalize(Mp curLocalRotate childLP) == normalize(childP - curP)
            Vector3 dirV = childP - curP;
            //dirV.z = -dirV.z;
            dirV.Normalize();

            Vector3 childLP = child.localPosition;

            Matrix4x4 curMat = cur.localToWorldMatrix;  //Mp curLocalRotate
            Matrix4x4 curLocalRotate = Matrix4x4.Rotate(cur.localRotation);
            Matrix4x4 Mp = curMat * curLocalRotate.inverse;
            Vector3 TargetV = Mp.inverse.MultiplyVector(dirV);   //Mp^-1 * dirV

            Quaternion localRotation = Quaternion.FromToRotation(childLP, TargetV);

            if (applyFilter)
            {
                if (!isBase)
                {
                    childLP.x = 0;
                    TargetV.x = 0;

                    localRotation = Quaternion.FromToRotation(childLP, TargetV);
                    float angle;
                    Vector3 axis;
                    localRotation.ToAngleAxis(out angle, out axis);

                    if (axis.x < -0.8)
                    {
                        axis = -axis;
                        angle = -angle;
                    }
                    if (angle < 0) angle = 0;
                    if (angle > 90) angle = 90;
                    localRotation = Quaternion.AngleAxis(angle, axis);
                }
            }


            //Vector3 eulerResult = localRotation.eulerAngles;
            //FilterEulerAngle(ref eulerResult, isBase);

            //cur.localRotation = Quaternion.Euler(eulerResult.x, eulerResult.y, eulerResult.z);
            cur.localRotation = Quaternion.Lerp(cur.localRotation, localRotation, 0.4f);
            //cur.localRotation =localRotation;
        }

        void UpdateFinger(GameObject fingerBase, int numLink, Vector3[] pts)
        {
            Transform baseTransform = handMesh.transform;
            //Matrix4x4 baseMat = baseTransform.localToWorldMatrix;
            Quaternion baseRot = baseTransform.localRotation;
            Transform cur = fingerBase.transform;
            for (int i = 0; i < numLink; ++i)
            {
                Transform child = cur.GetChild(0);
                UpdateLink(baseRot, ref cur, ref child, pts[i], pts[i + 1], i == 0);
                baseRot = baseRot * cur.localRotation;

                Vector3 diff1 = pts[i + 1] - pts[i];
                cur = child;
            }
        }

        unsafe void SetPts(Vector3[] pts, Vector3* fittedPts, int i1, int i2, int i3, int i4 = -1)
        {
            pts[0] = fittedPts[i1];
            pts[1] = fittedPts[i2];
            pts[2] = fittedPts[i3];
            if (i4 != -1)
                pts[3] = fittedPts[i4];
        }

        unsafe void UpdateHandFinger(Vector3* fittedPts)
        {
            Vector3[] pts = new Vector3[4];

            SetPts(pts, fittedPts, 2, 1, 0);
            UpdateFinger(thumbMesh, 2, pts);

            SetPts(pts, fittedPts, 6, 5, 4, 3);
            UpdateFinger(indexMesh, 3, pts);

            SetPts(pts, fittedPts, 10, 9, 8, 7);
            UpdateFinger(middleMesh, 3, pts);

            SetPts(pts, fittedPts, 14, 13, 12, 11);
            UpdateFinger(ringMesh, 3, pts);

            SetPts(pts, fittedPts, 18, 17, 16, 15);
            UpdateFinger(pinkyMesh, 3, pts);
        }

        void CompensateRotate()
        {
            float angle;
            float max_angle;
            Vector3 axis = new Vector3();

            //get max angle
            indexMesh.transform.localRotation.ToAngleAxis(out max_angle, out axis);
            max_angle = Mathf.Abs(max_angle);

            middleMesh.transform.localRotation.ToAngleAxis(out angle, out axis);
            angle = Mathf.Abs(angle);
            if (max_angle < angle) max_angle = angle;

            ringMesh.transform.localRotation.ToAngleAxis(out angle, out axis);
            angle = Mathf.Abs(angle);
            if (max_angle < angle) max_angle = angle;

            pinkyMesh.transform.localRotation.ToAngleAxis(out angle, out axis);
            angle = Mathf.Abs(angle);
            if (max_angle < angle) max_angle = angle;

            //compensate rotate
            Quaternion quat1 = Quaternion.AngleAxis(-max_angle * compensateRotate, new Vector3(1.0f, 0.0f, 0.0f)) * handMesh.transform.localRotation;
            //Quaternion quat2 = handMesh.transform.localRotation * Quaternion.AngleAxis(max_angle * compensateRotate, new Vector3(1.0f, 0.0f, 0.0f));
            //Debug.LogFormat("{0},{1},{2},{3} // {4},{5},{6},{7}", quat1.w, quat1.x, quat1.y, quat1.z, quat2.w, quat2.x, quat2.y, quat2.z);
            handMesh.transform.localRotation = quat1;
        }

        unsafe void Detect(Vector3* points)
        {
            if (points == null)
            {
                handMesh.SetActive(false);
                return;
            }
            if (handMesh == null)
                return;

            //Debug.LogFormat("{0}", count);
            SetScale();

            // Confidence for the hand. It's from scan operation
            //if (hand.Confidence < 0.660f)
            //return;

            // Notice that this is image coordinate....            
            //Debug.LogFormat("{0}, {1}, {2}", points[19].x, points[19].y, points[19].z);

            GetHandTransform(points, handMesh);

            if (!doNotUpdate)
            {
                UpdateHandFinger(points);
                CompensateRotate();
            }
        }

        public unsafe void UseHandData(ref ImageData image, ref HandData hand, int leftOrRight)
        {
            Vector3* ptr = hand.Points;

            Vector3* FingerAnchor = stackalloc Vector3[20];

            if (ReadWebcam.instance.isCameraFront)
            {
                for (int i = 0; i < 21; i++)
                {
                    ptr[i].y = -ptr[i].y;
                }
            }
            for (int i = 0; i < 21; i++)
            {
                FingerAnchor[i] = ptr[mapToOld[i]];
            }

            Detect(FingerAnchor);

        }
    }
}
