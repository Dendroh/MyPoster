using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Alchera
{
    public class FaceBoxTrackablePrefab : MonoBehaviour
    {
        Transform DebugDrawingAnchor;
        Transform[] debugPrefabs;
        //Vector3[] initScale;

        int debugCounter = 0;
        void Start()
        {
            DebugDrawingAnchor = transform.GetChild(0);
            //  point  : 0~3
            //  link     : 4~7
            debugPrefabs = new Transform[4 + 4];

            for (int i = 0; i < debugPrefabs.Length; i++)
            {
                debugPrefabs[i] = DebugDrawingAnchor.GetChild(i);
            }
        }
        public void SetPoints(Box box)
        {
            float[] vertexPosX = { box.min.x ,
                                                box.max.x,
                                                box.max.x,
                                                box.min.x };
            float[] vertexPosY = { box.max.y,
                                                box.max.y,
                                                box.min.y,
                                                box.min.y};
            for (var p = 0; p < 4; ++p)
            {
                var pointX = vertexPosX[p];
                var pointY = vertexPosY[p];
                //0.44는 손의 크기와 일치시키기 위한 임의의 값.
                //손보다 크기를 키우거나 줄이는 경우가 필요할 거라 생각하여 변수로 놔둠
                var posX = -1 * (pointX - 640+400 ) * 0.68f;
                var posY = (pointY - 360 +80) * 0.68f;
                debugPrefabs[p].localPosition = new Vector3(-posX, posY, 300);
            }
            SetLinks();
        }

        public void SetLinks()
        {
            var scaleFactor = 8;
            SetSkeletonLink(4, 0, 1, scaleFactor);
            SetSkeletonLink(5, 1, 2, scaleFactor);
            SetSkeletonLink(6, 2, 3, scaleFactor);
            SetSkeletonLink(7, 3, 0, scaleFactor);
        }

        unsafe void SetSkeletonLink(int linkIdx, int baseIdx, int targetIdx, float scaleFactor)
        {
            if (debugPrefabs == null)
            {
                return;
            }
            Vector3 basePos = debugPrefabs[baseIdx].localPosition;
            Vector3 targetPos = debugPrefabs[targetIdx].localPosition;
            debugPrefabs[linkIdx].localPosition = (basePos + targetPos) * 0.5f;
            Vector3 diff = targetPos - basePos;

            float dist = diff.magnitude;

            if (dist < 1.0e-8)
            {
                debugPrefabs[linkIdx].localRotation = Quaternion.identity;
            }
            else
            {
                debugPrefabs[linkIdx].localScale = new Vector3(scaleFactor * 0.25f, dist / 2.0f, scaleFactor * 0.25f);

                Quaternion localQuaternion = Quaternion.identity;
                localQuaternion.SetFromToRotation(new Vector3(0, 1, 0), diff / dist);

                debugPrefabs[linkIdx].localRotation = localQuaternion;
            }
        }
    }

}