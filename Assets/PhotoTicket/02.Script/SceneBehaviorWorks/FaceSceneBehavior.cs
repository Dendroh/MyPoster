﻿// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
//  Author      dh.park@alcherainc.com
//
// ---------------------------------------------------------------------------
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Alchera
{
	public class FaceSceneBehavior : MonoBehaviour
	{
		ITextureSequence sequence;
		ITextureConverter converter;
		IDetectService detector;

		public GameObject FaceConsumer;
		IFaceListConsumer consumer;

		public Button QuitButton;

		void Awake()
		{
			sequence = this.GetComponent<ITextureSequence>();
			converter = this.GetComponent<ITextureConverter>();
			detector = this.GetComponent<IDetectService>();
			consumer = FaceConsumer.GetComponent<IFaceListConsumer>();

			QuitButton.onClick.AddListener(Application.Quit);
		}

		async void Start()
		{
			// start a logic loop
			try
			{
				IEnumerable<FaceData> faces = null;

				foreach (Task<Texture> request in sequence.RepeatAsync())
				{
					var texture = await request;
					var image = await converter.ConvertAsync(texture);
					var translator = await detector.DetectAsync(ref image);

					faces = translator.Fetch<FaceData>(faces);
					if (faces != null)
					{
						consumer.Consume(ref image, faces);
					}

					// release holding resources for detection
					translator.Dispose();
				}
			} catch (TaskCanceledException e)
			{
				Debug.LogWarning(e.ToString());

				sequence.Dispose();
				detector.Dispose();

				// For now, there is NO way to recover for this kind of exception ...
			} catch (Exception e)
			{
				Debug.LogError(e.ToString());
				Application.Quit();
			}
		}

		void OnDestroy()
		{
			// ensure disposal
			sequence.Dispose();
			detector.Dispose();

			Debug.LogWarning("FaceSceneBehavior");
		}

	}
}