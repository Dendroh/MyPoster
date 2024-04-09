// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
//  Author      sg.js@alcherainc.com
//
// ---------------------------------------------------------------------------
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Alchera
{
    public class CaptureSceneBehavior : MonoBehaviour
    {
        ITextureSequence sequence;
        ITextureConverter converter;
        IConsumer<Texture> consumer;

        public Button SwapButton;
        public Button QuitButton;

        void Awake()
        {
            sequence = this.GetComponent<ITextureSequence>();
            converter = GetComponent<ITextureConverter>();
            consumer = this.GetComponent<IConsumer<Texture>>();

            QuitButton.onClick.AddListener(Application.Quit);
            SwapButton.onClick.AddListener(() =>
            {
                var webcamReader = this.GetComponent<ReadWebcamInSequence>();
                webcamReader.Swap();
            });
        }

        async void Start()
        {
            try
            {
                foreach (Task<Texture> request in sequence.RepeatAsync())
                {
                    if (request == null)
                        continue;
                    var texture = await request;
                    var image = await converter.ConvertAsync(texture);
                    if (consumer != null)
                        consumer.Consume(ref image, texture);
                }
            }
            catch (TaskCanceledException exception)
            {
                Debug.LogWarning(exception.ToString());
                sequence.Dispose();
            }
        }

    }
}
