using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Alchera
{
	/// <summary>
	/// Detection 된 HandData list들 중 한 값을 이용하여 직접 사용하는 클래스.
	/// 
	/// IHand2D 구현체. 실제로 화면에 보여지는 Prefab을 조절합니다.
	/// </summary>
	public class HandTrackablePrefab : MonoBehaviour, IHand2D
	{
		[SerializeField] Transform[] pivotPoses;    //한 손에 대한 pivots. 현재는 Center만 있음 
		HandMotionDetector motionDetector;

		Transform DebugDrawingAnchor;
		Transform[] debugPrefabs;
		TextMesh tm;
		//Vector3[] initScale;
		AutoBackgroundQuad quad;
		float handPosZ = -300;
		void Start()
		{
			motionDetector = GetComponent<HandMotionDetector>();
			DebugDrawingAnchor = transform.GetChild(0);
			//  point  : 0~3
			//  link     : 4~7
			// text   : 8
			debugPrefabs = new Transform[4 + 4 + 1];

			for (int i = 0; i < debugPrefabs.Length; i++)
			{
				debugPrefabs[i] = DebugDrawingAnchor.GetChild(i);
			}
			tm = debugPrefabs[8].GetComponent<TextMesh>();
		}
		/**
		인덱스에 맞는 스티커 프리팹들을 추가 
		@param  sticker 추가할 sticker 프리팹
		@param  index 영화 인덱스
		 */
		public void SetPivot(GameObject sticker, int index, int movieNumber)
		{
			var prefab = Instantiate(sticker, pivotPoses[index]);
			prefab.AddComponent<Text>();
			prefab.GetComponent<Text>().text = movieNumber.ToString();
			TryEnrollAnimator(prefab);
		}
		private void TryEnrollAnimator(GameObject prefab)
		{
			prefab.tag = "sticker";
			var animator = prefab.GetComponent<StickerPose>();
			if (animator != null)
				motionDetector.AddAnimator(prefab.GetComponent<Animator>(), (int)animator.motionType);
		}
		/**
		HandData를 이용하여 box를 그리고,  posture text를 적고, pivot의 transform 을 조절합니다.
		@param  hand handdata 데이터값 
		@param  needMirror 좌우반전요소. 
		    
		    .cs에서 온 값
		@param  offset 유니티와 opencv 좌표계를 맞추기 위한 offset
		 */
		public void UseHandData(ref ImageData image, ref HandData hand, int leftOrRight)
		{
			//if (hand.Points[0].x > 10000|| hand.Points[0].y > 10000|| hand.Points[0].z > 10000) 
			//    return;
			if (debugPrefabs == null)
				return;
			if (quad == null)
			{
				Debug.LogWarning("Finding WebCamQuad...");
				quad = FindObjectOfType<AutoBackgroundQuad>();
				return;
			}

			SetPoints(ref image, ref hand);
			SetLinks();
			motionDetector.CheckPosture(hand.Posture);
		}

		unsafe void SetPoints(ref ImageData image, ref HandData hand)
		{
			BoundBox box = hand.Box;

			float[] vertexPosX = { box.cx,
						    box.cx + box.width,
						    box.cx + box.width,
						    box.cx };
			float[] vertexPosY = { box.cy + box.height,
						    box.cy + box.height,
						    box.cy ,
						    box.cy };

			var height = quad.texture.height < 16 ? 1 : quad.texture.height; //divide by zero 방지 
			float adjustment = quad.transform.localScale.y / height;

			float centerX = quad.texture.width / 2;
			float centerY = quad.texture.height / 2;
			if (ReadWebcam.instance.GetAdjustedVideoRotationAngle() % 180 != 0)
			{
				centerX = quad.texture.height / 2;
				centerY = quad.texture.width / 2;
			}

			var ratio = handPosZ / quad.transform.localPosition.z;
			for (var p = 0; p < 4; ++p)
			{
				var st = debugPrefabs[p];

				var posX = (vertexPosX[p] - centerX + image.OffsetX) * adjustment * ratio;
				var posY = -(vertexPosY[p] - centerY + image.OffsetY) * adjustment * ratio;
				var posZ = handPosZ;

				st.localPosition = new Vector3(posX, posY, posZ);
			}

			var postureText = debugPrefabs[8];
			var textPosX = (hand.Center.x - centerX + image.OffsetX) * adjustment * ratio;
			var textPosY = -(hand.Center.y - centerY + image.OffsetY) * adjustment * ratio;
			var textPosZ = handPosZ;

			postureText.localPosition = new Vector3(textPosX, textPosY, textPosZ);
			postureText.localScale = Vector3.one * ((box.width + box.height) / 2) * adjustment * ratio;
			tm.text = hand.Posture.ToString();

			pivotPoses[0].localPosition = new Vector3(textPosX, textPosY, textPosZ);
			pivotPoses[0].localScale = Vector3.one * ((box.width + box.height) / 2) * adjustment * ratio;
			pivotPoses[0].localRotation = new Quaternion(0, 0, 0, 0);
		}

		void SetLinks()
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
			} else
			{
				debugPrefabs[linkIdx].localScale = new Vector3(scaleFactor * 0.25f, dist / 2.0f, scaleFactor * 0.25f);

				Quaternion localQuaternion = Quaternion.identity;
				localQuaternion.SetFromToRotation(new Vector3(0, 1, 0), diff / dist);

				debugPrefabs[linkIdx].localRotation = localQuaternion;
			}
		}
	}
}

