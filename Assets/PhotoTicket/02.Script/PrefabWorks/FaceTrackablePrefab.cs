using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Alchera
{
	/// <summary>
	/// Detection 된 FaceData list들 중 한 값을 이용하여 직접 사용하는 클래스.
	/// 
	/// IFace2D 구현체. 실제로 화면에 보여지는 prefab을 조절합니다.
	/// </summary>
	public class FaceTrackablePrefab : MonoBehaviour, IFace3D
	{
		SkinnedMeshRenderer smr;
		[SerializeField] Transform[] pivotPoses;
		FaceMotionDetector motionDetector;
		Transform leftEye;
		Transform rightEye;

		Dictionary<string, int> blendIndices;

		public float distance = 1f;
		private bool isStarted = false;
		public Pose HeadPose
		{
			set
			{
				var lerpRatio = 0.8f;   // smoother with moving average

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
			motionDetector = GetComponent<FaceMotionDetector>();

			isStarted = true;
			transform.localScale = new Vector3(1, -1, -1);


		}
		public void SetEyeRotation(ref Quaternion left, ref Quaternion right)
		{
			if (leftEye == null || rightEye == null)
				return;
			leftEye.localRotation = left;
			rightEye.localRotation = right;
		}

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
					} else
					{
						Debug.Log(key + " is not founded..");
					}
				}
			}
		}
		/**
		인덱스에 맞는 스티커 프리팹들을 추가 
		@param  sticker 추가할 sticker 프리팹
		@param  index 영화 인덱스
		 */
		public void SetPivot(GameObject sticker, int index, int movieNumber)
		{
			var prefab = Instantiate(sticker, pivotPoses[index]);
			TryEnrollAnimator(prefab);
			prefab.AddComponent<Text>();
			prefab.GetComponent<Text>().text = movieNumber.ToString();
			leftEye = transform.Find("LeftEye");
			rightEye = transform.Find("RightEye");
			// Acquire renderer for blendshape
			smr = this.GetComponentInChildren<SkinnedMeshRenderer>();
			if (smr == null) return;

			blendIndices = new Dictionary<string, int>();
			for (int i = 0; i < smr.sharedMesh.blendShapeCount; ++i)
			{
				var name = smr.sharedMesh.GetBlendShapeName(i);
				blendIndices.Add(name.ToLower(), i);
			}

		}

		void TryEnrollAnimator(GameObject prefab)
		{
			prefab.tag = "sticker";
			var animator = prefab.GetComponent<StickerPose>();
			if (animator != null)
				motionDetector.AddAnimator(prefab.GetComponent<Animator>(), (int)animator.motionType);
		}

		/**
		FaceData를 이용하여 xyz축을 그리고 pivot의 transform 을 조절합니다.
		@param  face facedata 데이터값 
		@param  needMirror 좌우반전요소. FaceService.cs에서 온 값
		 */
		public unsafe void UseFaceData(ref ImageData image, ref FaceData face)
		{
			if (isStarted == false)
			{
				return;
			}
			var landmark = face.Landmark;

			var pose = face.HeadPose;

			ReadWebcam.instance.GetMirrorValue(out var mirrorX, out var mirrorY);
			pose.position = new Vector3(mirrorX * pose.position.x, -pose.position.y, pose.position.z) * distance;
			pose.rotation = new Quaternion(pose.rotation.x, -1 * pose.rotation.y, pose.rotation.z, -1 * pose.rotation.w);

			HeadPose = pose;

			motionDetector.CheckMotion(landmark);

			if (smr == null) return;

			float* weights = stackalloc float[FaceData.NumAnimationWeights];
			face.GetAnimation(weights);
			SetAnimation(weights);

		}
	}
}