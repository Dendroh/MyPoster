// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
//  Author      dh.park@alcherainc.com
//
// ---------------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Alchera
{

    public interface ITextureSequence : IDisposable
    {
        Task<Texture> CaptureAsync();
        IEnumerable<Task<Texture>> RepeatAsync();
    }

}