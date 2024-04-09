using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Alchera
{
	public struct Box
	{
		public Vector2 min;
		public Vector2 max;
		public Box(float minX, float minY, float maxX, float maxY)
		{
			min = new Vector2(minX, minY);
			max = new Vector2(maxX, maxY);
		}
	}
	/// <summary>
	/// SceneBehavior에서 Detection 된 FaceData list를 관리하는 클래스 
	/// 
	/// 등록된 Maxcount 만큼 프리팹 풀을 생성하며, 현재 인식되는 갯수만큼 프리팹을 보여줍니다.
	/// </summary>
	public class Draw2DFacemark : MonoBehaviour, IFace3DFactory, IFaceListConsumer
	{
		public GameObject Parent; ///< Hierarchy 창에서 어디에 프리팹 풀을 생성할 지 결정.
		public GameObject Prefab;///<생성할 프리팹. IFace2D 인터페이스를 구현하여야 합니다.

		IFace3D[] faces;
		GameObject[] pool;

		bool need3D;
		int maxCount;

		public List<Box> faceBoxList;///< Hand 오검출 개선용 얼굴 BoxList

		void Awake()
		{
			faceBoxList = new List<Box>();
			maxCount = GetComponent<FaceService>().maxCount;
			need3D = GetComponent<FaceService>().need3D;
			faces = new IFace3D[maxCount];
			pool = new GameObject[maxCount];

			for (int i = 0; i < maxCount; i++)
			{
				faces[i] = Create(out pool[i]);
				pool[i].transform.SetParent(Parent.transform);
			}
		}
		/**
		처음에 maxCount만큼 obj를 생성하며 풀을만들고, 이를 SetActive를 이용하여 사용합니다. 
		@param  obj 생성할 GameObject 프리팹 
		@returns 생성된 GameObject Clone
		 */
		public IFace3D Create(out GameObject obj)
		{
			obj = Instantiate(Prefab);
			var face = obj.GetComponent<IFace3D>();
			if (face == null)
				throw new UnityException(string.Format(
				    "IFace2D not found : {0}", Prefab.name));
			return face;
		}

		/**
		Detection후 받아온 FaceData list를 받아서 IFace Prefab에 넘겨주는 함수.
		추가로 받아온 
		@param  list FaceData의 list. list의 크기는 현재 Detect한 얼굴의 갯수입니다.
	       */
		public unsafe void Consume(ref ImageData image, IEnumerable<FaceData> list)
		{
			if (pool[0] == null) return; //풀이 없으면 아무일도 하지 않는다.

			int i = 0;
			FaceData face = default(FaceData);
			faceBoxList.Clear();
			foreach (var item in list)
			{

				pool[i].transform.position = new Vector3(0, 0, 0);
				pool[i].transform.localScale = new Vector3(1, -1, -1);
				
				face = item;
				faces[i].UseFaceData(ref image, ref face);
				var ratio = 720f / 1080;
				var minX = item.Landmark[0].x * ratio;
				var minY = (1920 - Mathf.Max(item.Landmark[35].y, item.Landmark[40].y)) * ratio;
				var maxX = item.Landmark[32].x * ratio;
				var maxY = (1920 - item.Landmark[16].y) * ratio;
				var width = maxX - minX;
				var height = maxY - minY;

				var rot = pool[i].transform.localRotation;

				if (rot.z < 0) maxX -= width * rot.z * 1.7f;
				else minX -= width * rot.z * 1.7f;
				minY -= width * Mathf.Abs(rot.w) * 1.7f;

				faceBoxList.Add(new Box(minX, minY, maxX, maxY));
				i++;
			}

			for (int j = i; j < maxCount; j++)
			{
				pool[j].transform.position = new Vector3(0, 0, -100);
				pool[j].transform.localScale = Vector3.zero;
			}
		}
	}

}
