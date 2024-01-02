using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;


namespace Alchera
{
    /// <summary>
    /// SceneBehavior에서 Detection 된 HandData list를 관리하는 클래스 
    /// 
    /// 등록된 Maxcount 만큼 프리팹 풀을 생성하며, 현재 인식되는 갯수만큼 프리팹을 보여줍니다.
    /// </summary>
    public class Draw2DSkeleton : MonoBehaviour, IHand2DFactory, IHandListConsumer
    {
        public GameObject Parent; ///< Hierarchy 창에서 어디에 프리팹 풀을 생성할 지 결정.
        public GameObject leftPrefab;///<생성할 프리팹. IHand2D 인터페이스를 구현하여야 합니다.
        public GameObject rightPrefab;
        
        Draw2DFacemark draw_face_script;

        IHand2D[] leftHands;
        GameObject[] leftPool;

        IHand2D[] rightHands;
        GameObject[] rightPool;
        
        bool need3D;
        int maxCount;
        void Awake()
        {
            draw_face_script = GetComponent<Draw2DFacemark>();
            maxCount = GetComponent<HandService>().maxCount;
            need3D = GetComponent<HandService>().need3D;
            
            leftHands = new IHand2D[maxCount];
            leftPool = new GameObject[maxCount];

            rightHands = new IHand2D[maxCount];
            rightPool = new GameObject[maxCount];

            for (int i = 0; i < maxCount; i++)
            {
                leftHands[i] = Create(out leftPool[i], 1);
                leftPool[i].transform.SetParent(Parent.transform);

                rightHands[i] = Create(out rightPool[i], 2);
                rightPool[i].transform.SetParent(Parent.transform);
            }
        }

        void Start()
        {
            if (FlowController.instance.drawStickerGuide == true)
                return;
            foreach (var item in GameObject.FindGameObjectsWithTag("DebugGuide"))
                item.SetActive(false);
        }
        /**
        처음에 maxCount만큼 obj를 생성하며 풀을만들고, 이를 SetActive를 이용하여 사용합니다. 
        @param  obj 생성할 GameObject 프리팹 
        @returns 생성된 GameObject Clone
         */
        public IHand2D Create(out GameObject obj, int leftOrRight)
        {
            bool isLeft = (leftOrRight == 1);

            obj = Instantiate(isLeft ? leftPrefab : rightPrefab);
            var hand = obj.GetComponent<IHand2D>();
            if (hand == null)
                throw new UnityException(string.Format(
                    "IHand not found with prefab: {0}", isLeft ? leftPrefab.name : rightPrefab.name));
            return hand;
        }
        /**
        Detection후 받아온 HandData list를 받아서 IHand Prefab에 넘겨주는 함수.
        추가로 받아온 
        @param  list HandData의 list. list의 크기는 현재 Detect한 얼굴의 갯수입니다.
       */
        public void Consume(ref ImageData image, IEnumerable<HandData> list)
        {
            if (leftPool[0] == null || rightPool[0] == null) return; //풀이 없으면 아무일도 하지 않는다.
            if (need3D == true)
            {
                Debug.LogError("Need3D should NOT be checked for using Draw2DSkeleton");
                return;
            }
            
            int l = 0;
            int r = 0;
            HandData hand = default(HandData);
           
            var CalculateFalseAlarm = FlowController.instance.removeFalseAlarm;
            foreach (var item in list)
            {
                Box handBox = new Box(item.Box.cx,
                                                            item.Box.cy,
                                                            item.Box.cx + item.Box.width,
                                                            item.Box.cy + item.Box.height);
                if (CalculateFalseAlarm && CheckIntersection(ref handBox) == true) 
                    continue;

                var leftOrRight = (int)item.LeftOrRight;
                if (leftOrRight == 1)
                {
                    leftPool[l].transform.position = Vector3.zero;
                    leftPool[l].transform.localScale = Vector3.one;
                    hand = item;
                    leftHands[l].UseHandData(ref image, ref hand, leftOrRight);
                    l++;
                }
                else if (leftOrRight == 2)
                {
                    rightPool[r].transform.position = Vector3.zero;
                    rightPool[r].transform.localScale = Vector3.one;
                    hand = item;
                    rightHands[r].UseHandData(ref image, ref hand, leftOrRight);
                    r++;
                }//if hand LeftOrRight is unknown, do nothing
            }
            for (int i = l; i < maxCount; i++)
            {
                leftPool[i].transform.position = new Vector3(0, 0, -100);
                leftPool[i].transform.localScale = Vector3.zero;
            }
            for (int i = r; i < maxCount; i++) { 
                rightPool[i].transform.position = new Vector3(0, 0, -100);
                rightPool[i].transform.localScale = Vector3.zero;
            }
         }
        bool CheckIntersection(ref Box H)
        {
            var faceBoxes = draw_face_script.faceBoxList;
            var count = 0;
            foreach (var F in faceBoxes)
            {
                if(F.max.x > H.min.x && F.min.x < H.max.x &&
                    F.max.y > H.min.y && F.min.y < H.max.y)
                {
                    //겹쳤다.
                    var Xbig = Mathf.Min(F.max.x, H.max.x);
                    var Xsmall = Mathf.Max(F.min.x, H.min.x);
                    var Ybig = Mathf.Min(F.max.y, H.max.y);
                    var Ysmall = Mathf.Max(F.min.y, H.min.y);

                    var s = (Xbig - Xsmall) * (Ybig - Ysmall);
                    var faceS = (F.max.x - F.min.x) * (F.max.y - F.min.y);
                    var handS = (H.max.x - H.min.x) * (H.max.y - H.min.y);
                    if (s / faceS > 0.45f || s / handS>0.95f) //겹친 넓이 / 손 넓이
                         count++;
                }
            }
            if(count == 0) 
                return false; //안겹쳤다. 생성함
            
            else
                return true; //겹쳤다. 생성 안함
        }
    }
}
