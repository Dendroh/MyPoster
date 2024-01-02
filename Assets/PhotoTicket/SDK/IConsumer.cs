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

    public interface IConsumer<T>
    {
        void Consume(ref ImageData image,T obj);
    }

    public interface IHandListConsumer : IConsumer<IEnumerable<HandData>>{}
    public interface IFaceListConsumer : IConsumer<IEnumerable<FaceData>>{}

}