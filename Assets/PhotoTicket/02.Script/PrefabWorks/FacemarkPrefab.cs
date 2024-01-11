using UnityEngine;

namespace Alchera
{
	public class FacemarkPrefab : MonoBehaviour, IFace2D
	{
		Transform[] facePoints;
		AutoBackgroundQuad quad;

		void Start()
		{
			// use "Alchera/Resources/LandmarkGuide.jpg" to see Facemark index
			facePoints = new Transform[FaceData.NumLandmark];
			for (int i = 0; i < facePoints.Length; i++)
			{
				facePoints[i] = transform.GetChild(i);
				facePoints[i].localScale = Vector3.one * 3;
			}
		}

		public void UseFaceData(ref ImageData image, ref FaceData face)
		{
			if (facePoints == null) return;
			if (quad == null)
			{
				Debug.LogWarning("Finding WebCamQuad...");
				quad = FindObjectOfType<AutoBackgroundQuad>();
				return;
			}

			SetPoints(ref image, ref face);
		}

		public unsafe void SetPoints(ref ImageData image, ref FaceData face)
		{
			var ptr = face.Landmark;

			var height = quad.texture.height < 16 ? 1 : quad.texture.height; //divide by zero 방지 
			float adjustment = quad.transform.localScale.y / height;

			float centerX = quad.texture.width / 2;
			float centerY = quad.texture.height / 2;
			if (ReadWebcam.instance.GetAdjustedVideoRotationAngle() % 180 != 0)
			{
				centerX = quad.texture.height / 2;
				centerY = quad.texture.width / 2;
			}
			ReadWebcam.instance.GetMirrorValue(out int mirrorX, out int mirrorY);

			for (int p = 0; p < FaceData.NumLandmark; ++p)
			{
				var posX = -mirrorX * (ptr[p].x - centerX + image.OffsetX) * adjustment;
				var posY = -mirrorY * (ptr[p].y - centerY + image.OffsetY) * adjustment; // opencv 와 unity 의 이미지 y 좌표계가 반대.
				var posZ = quad.transform.localPosition.z;

				facePoints[p].localPosition = new Vector3(posX, posY, posZ);
				facePoints[p].localScale = Vector3.one * Screen.width * 0.002f; //적당한 크기로 사이즈 조절
			}
		}
	}
}