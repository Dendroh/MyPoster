// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
//  Author      sg.ju@alcherainc.com
//
// ---------------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Alchera
{
	/// <summary>
	/// 하드웨어 카메라로부터 Input값을 가져와 텍스쳐를 만드는 클래스 
	/// </summary>
	public sealed class ReadWebcamInSequence : ReadWebcam, ITextureSequence
	{
		TaskCompletionSource<Texture> promise;

		[HideInInspector] public bool needCamera; ///< 카메라 실행상태여부

		public static bool bUpdateQuad = false;
		public static bool bSendTexture = false;

		/**
		카메라상태를 변경합니다. flag를 이용해 카메라를 켜거나 끌 수 있습니다.
		@param flag 이 값에 따라 카메라 상태가 변경됩니다.
		 */
		public void changeCameraState(bool flag)
		{
			needCamera = flag;
			if (needCamera)
			{
				if (bWebcam == false)
				{
					loadCamera();
				}

				if (!bUpdateQuad)
				{
					bUpdateQuad = true;
					Debug.Log("update quad");
					StartCoroutine(normalQuad.UpdateQuad());
					StartCoroutine(chromarkeyQuad.UpdateQuad());
				}

				if (bSendTexture)
				{
					StartCoroutine(SendTextureFrom(current));
				}

				if (current.isPlaying == false)
				{
					current.Play();
				}
			} else
			{
				current.Stop();
				chromarkeyQuad.SetQuadMaterial(flag);
				normalQuad.SetQuadMaterial(flag);
			}
		}

		IEnumerable<Task<Texture>> ITextureSequence.RepeatAsync()
		{
			promise = new TaskCompletionSource<Texture>();
			while (true)
			{
				yield return promise.Task;
				if (current != null && current.isPlaying)
				{
					promise = new TaskCompletionSource<Texture>();
				}
			}
		}

		IEnumerator SendTextureFrom(WebCamTexture webcam)
		{
			do
			{
				if (promise != null)
					promise.TrySetResult(webcam as Texture);
				yield return null;
			}
			while (webcam.isPlaying);
		}

		Task<Texture> ITextureSequence.CaptureAsync()
		{
			if (current.isPlaying == false)
				current.Play();
			var source = new TaskCompletionSource<Texture>();
			source.SetResult(this.current as Texture);
			return source.Task;
		}

		public void Swap()
		{
			Debug.LogFormat("Number of Device: {0}", devices.Length);
			if (devices.Length >= 2)
			{
				current.Stop();
				current = (current == rear) ? front : rear;
				current.Play();
				isCameraFront = GetNextCameraFR();
			}

			var behavior = GetComponent<ISceneBehavior>();
			if (behavior != null)
				behavior.Start();
			else
				Debug.LogError("Cannot find any behaviors");

			StartCoroutine(chromarkeyQuad.UpdateQuad());
			StartCoroutine(normalQuad.UpdateQuad());
		}

		void IDisposable.Dispose()
		{
			if (promise == null)
				return;

			promise.TrySetCanceled();
			promise = null;
		}

		new void OnDestroy()
		{
			base.OnDestroy();
			(this as IDisposable).Dispose();
		}


	}
}