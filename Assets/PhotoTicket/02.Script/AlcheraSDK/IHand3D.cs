using System;
using UnityEngine;

namespace Alchera
{
    public interface IHand3D
    {
        void UseHandData(ref ImageData image, ref HandData hand, int leftOrRight);
    }

    public interface IHand3DFactory
    {
        IHand3D Create(out GameObject obj,int leftOrRight);
    }
    
}
