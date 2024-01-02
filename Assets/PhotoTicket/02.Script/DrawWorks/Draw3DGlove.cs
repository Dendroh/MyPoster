using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

//Actually it is almost same as Draw3DSkeleton :)
namespace Alchera
{
    public class Draw3DGlove : MonoBehaviour, IHand3DFactory, IHandListConsumer
    {
        public GameObject Parent;
        public GameObject leftPrefab;
        public GameObject rightPrefab;

        Vector3 leftInitPos;
        IHand3D[] leftHands;
        GameObject[] leftPool;

        Vector3 rightInitPos;
        IHand3D[] rightHands;
        GameObject[] rightPool;

        float[] lrBuffer;

        bool need3D;
        bool needMirror;
        int maxCount;
        void Awake()
        {
            maxCount = GetComponent<HandService>().maxCount;
            need3D = GetComponent<HandService>().need3D;

            leftHands = new IHand3D[maxCount];
            leftPool = new GameObject[maxCount];
            leftInitPos = leftPrefab.GetComponent<Transform>().localPosition;

            rightHands = new IHand3D[maxCount];
            rightPool = new GameObject[maxCount];
            rightInitPos = rightPrefab.GetComponent<Transform>().localPosition;

            lrBuffer = new float[maxCount * 2];
            for (int i = 0; i < maxCount; i++)
            {
                leftHands[i] = Create(out leftPool[i], 1);
                leftPool[i].transform.parent = Parent.transform;
                leftPool[i].transform.localPosition = leftInitPos;
                rightHands[i] = Create(out rightPool[i], 2);
                rightPool[i].transform.parent = Parent.transform;
                rightPool[i].transform.localPosition = rightInitPos;
            }
        }

        async void Start()
        {
            await Task.CompletedTask;
        }

        public IHand3D Create(out GameObject obj, int leftOrRight)
        {
            bool isLeft = (leftOrRight == 1);
            obj = Instantiate(isLeft ? leftPrefab : rightPrefab);
            var hand = obj.GetComponent<IHand3D>();
            if (hand == null)
                throw new UnityException(string.Format(
                    "IHand not found with prefab: {0}", isLeft ? leftPrefab.name : rightPrefab.name));
            return hand;
        }

        public void Consume(ref ImageData image, IEnumerable<HandData> list)
        {
            if (need3D == false)
            {
                Debug.LogError("Need3D should be checked for using Draw3DSkeleton");
                return;
            }
            for (int i = 0; i < maxCount; i++)
            {
                leftPool[i].SetActive(false);
                rightPool[i].SetActive(false);
            }

            int l = 0;
            int r = 0;
            int lr = 0;
            HandData hand = default(HandData);
            foreach (var item in list)
            {
                lrBuffer[lr] = Mathf.Lerp(lrBuffer[lr], (float)item.LeftOrRight, 0.3f);
                var leftOrRight = (int)Mathf.Round(lrBuffer[lr]);
                if (leftOrRight == 1)
                {
                    leftPool[l].SetActive(true);
                    hand = item;
                    leftHands[l].UseHandData(ref image, ref hand, leftOrRight);
                    l++;
                }
                else if (leftOrRight == 2)
                {
                    rightPool[r].SetActive(true);
                    hand = item;
                    rightHands[r].UseHandData(ref image, ref hand, leftOrRight);
                    r++;
                }
                else
                {
                    //if hand LeftOrRight is unknown, do nothing
                }
                lr++;
            }
            for (int i = lr; i < maxCount * 2; i++)
            {
                lrBuffer[i] = 1.5f;
            }
        }
    }

}
