// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
//  Author      sg.ju@alcherainc.com
//
// ---------------------------------------------------------------------------
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace Alchera
{
	public class AutoBackgroundQuad : MonoBehaviour
	{
		[SerializeField] Material chromarkeyWebcamMaterial = null;
		[SerializeField] Material normalWebcamMaterial = null;
		[SerializeField] Texture blackTexture = null;
		[HideInInspector] public Texture texture;
		WaitForSeconds ws = new WaitForSeconds(0.5f);

		public static bool bChromarkey = false;
		public static bool updateQuad = false;

		void Start()
		{
			chromarkeyWebcamMaterial.mainTexture = blackTexture;
			normalWebcamMaterial.mainTexture = blackTexture;
			texture = new Texture2D(16, 16);
		}

		public void SetQuadMaterial(bool flag)
		{
			chromarkeyWebcamMaterial.mainTexture = flag ? texture : blackTexture;
			normalWebcamMaterial.mainTexture = flag ? texture : blackTexture;
		}

		public IEnumerator UpdateQuad()
		{
			texture = new Texture2D(16, 16);

			while (texture.width < 20)
			{
				var request = (ReadWebcam.instance as ITextureSequence).CaptureAsync();
				yield return request;
				texture = request.Result;
				yield return ws;
			}
			SetQuadMaterial(true);
			updateQuad = true;
		}
	}

}