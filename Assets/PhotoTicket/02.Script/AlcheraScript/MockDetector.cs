// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
//  Author      dh.park@alcherainc.com
//
//  Note
//          This file is under develop
//
// ---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Alchera
{
    public unsafe class MockYolo : IDetectService, ITranslator
    {
        Rect[] regions;

        public MockYolo()
        {
            regions = new Rect[40];
        }

        Task<ITranslator> IDetectService.DetectAsync(ref ImageData image)
        {
            return Task.FromResult(this as ITranslator);
        }

        void IDisposable.Dispose()
        {
            // ... dispose internal detector...

            // ... dispose memory ...
            regions = null;
        }

        IEnumerable<T> ITranslator.Fetch<T>(IEnumerable<T> reuse)
        {
            if (typeof(T) != typeof(Rect))
                throw new ArgumentException(String.Format(
                    "Available Type: {0}", typeof(Rect).FullName));

            return new ArraySegment<Rect>(regions, 1, 3) as IEnumerable<T>;
        }
    }

    class MockTranslator : ITranslator
    {
        HandData[] hint1List;

        void IDisposable.Dispose()
        {
            hint1List = null;
        }

        private IEnumerable<HandData> Handle(IEnumerable<HandData> reuse)
        {
            if (hint1List == null)
                hint1List = new HandData[0];

            return hint1List;
        }

        IEnumerable<T> 
            ITranslator.Fetch<T>(IEnumerable<T> reuse)
        {
            // handle each types...
            if (typeof(T) == typeof(HandData))
                return Handle(reuse as IEnumerable<HandData>) as IEnumerable<T>;
                
            throw new NotSupportedException(String.Format(
                "{0} is not supported for this implementation",
                typeof(T).FullName));
        }
    }

    public class MockDetector :
        MonoBehaviour, IDetectService
    {
        MockTranslator translator;
        TaskCompletionSource<ITranslator> promise;

        public void Awake()
        {
            translator = new MockTranslator();
        }

        Task<ITranslator> 
            IDetectService.DetectAsync(ref ImageData image)
        {
            if (promise != null)
                promise.TrySetCanceled();

            promise = new TaskCompletionSource<ITranslator>();
            promise.SetResult(translator);
            return promise.Task;
        }

        void IDisposable.Dispose()
        {
            if (promise == null)
                return;

            promise.TrySetCanceled();
            promise = null;
        }
    }
}