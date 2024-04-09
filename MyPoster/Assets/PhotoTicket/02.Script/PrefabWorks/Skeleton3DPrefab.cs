using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alchera
{
    public class Skeleton3DPrefab : MonoBehaviour, IHand3D
    {
        Transform[] skeleton;
        public float distFromCamAdjustment = 0;
        public float scalerAdjustment = 0.55f;
        void Start()
        {
            //  wrist   : 0
            //  thumb   : 1~4
            //  1st     : 5~8
            //  2nd     : 9~12
            //  3rd     : 13~16
            //  4th     : 17~20
            //  link fingerpoints       : 21~35
            //  link finger with wrist  : 36~41
            skeleton = new Transform[HandData.NumPoints + 21];
            for (int i = 0; i < skeleton.Length; i++)
                skeleton[i] = transform.GetChild(i);
        }
        public void UseHandData(ref ImageData image, ref HandData hand, int leftOrRight)
        {
            if (skeleton == null)
            {
                return;
            }
            SetPoints(ref hand);
            SetLinks();
        }
        public unsafe void SetPoints(ref HandData hand)
        {
            Vector3* ptr = hand.Points;
            int mirror = ReadWebcam.instance.isCameraFront ? -1 : 1;
            for (var p = 0; p < HandData.NumPoints; ++p)
            {
                var st = skeleton[p];
                var point = ptr[p];
                point.y = mirror * point.y;

                point.z *= scalerAdjustment - distFromCamAdjustment;

                //st.localPosition = Vector3.Lerp(st.localPosition, point, 0.5f);
                st.localPosition = point;
            }
            //currently, 3D fitting only use 20 point. So we need to adjust one point manually.
            skeleton[1].localPosition = Vector3.Lerp(
                skeleton[0].localPosition,
                skeleton[2].localPosition, 0.6f);
        }

        public void SetLinks()
        {
            var scaleFactor = 1.0f;
            SetSkeletonLink(21, 1, 2, scaleFactor);
            SetSkeletonLink(22, 2, 3, scaleFactor);
            SetSkeletonLink(23, 3, 4, scaleFactor);

            SetSkeletonLink(24, 5, 6, scaleFactor);
            SetSkeletonLink(25, 6, 7, scaleFactor);
            SetSkeletonLink(26, 7, 8, scaleFactor);

            SetSkeletonLink(27, 9, 10, scaleFactor);
            SetSkeletonLink(28, 10, 11, scaleFactor);
            SetSkeletonLink(29, 11, 12, scaleFactor);

            SetSkeletonLink(30, 13, 14, scaleFactor);
            SetSkeletonLink(31, 14, 15, scaleFactor);
            SetSkeletonLink(32, 15, 16, scaleFactor);

            SetSkeletonLink(33, 17, 18, scaleFactor);
            SetSkeletonLink(34, 18, 19, scaleFactor);
            SetSkeletonLink(35, 19, 20, scaleFactor);

            SetSkeletonLink(36, 0, 1, scaleFactor);
            SetSkeletonLink(37, 1, 5, scaleFactor);
            SetSkeletonLink(38, 5, 9, scaleFactor);
            SetSkeletonLink(39, 9, 13, scaleFactor);
            SetSkeletonLink(40, 13, 17, scaleFactor);
            SetSkeletonLink(41, 17, 0, scaleFactor);
        }

        unsafe void SetSkeletonLink(int linkIdx, int baseIdx, int targetIdx, float scaleFactor)
        {
            if (skeleton == null)
            {
                return;
            }
            Vector3 basePos = skeleton[baseIdx].localPosition;
            Vector3 targetPos = skeleton[targetIdx].localPosition;
            skeleton[linkIdx].localPosition = (basePos + targetPos) * 0.5f;
            Vector3 diff = targetPos - basePos;

            float dist = diff.magnitude;

            if (dist < 1.0e-8)
            {
                skeleton[linkIdx].localRotation = Quaternion.identity;
            }
            else
            {
                skeleton[linkIdx].localScale = new Vector3(scaleFactor * 0.25f, dist / 2.0f, scaleFactor * 0.25f);

                Quaternion localQuaternion = Quaternion.identity;
                localQuaternion.SetFromToRotation(new Vector3(0, 1, 0), diff / dist);

                skeleton[linkIdx].localRotation = localQuaternion;
            }
        }

        public void UseHandData(ref HandData hand, int leftOrRight, ref Vector2 offset)
        {
            throw new System.NotImplementedException();
        }
    }
}

