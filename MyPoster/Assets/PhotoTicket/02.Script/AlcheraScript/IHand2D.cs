using System;
using UnityEngine;

namespace Alchera
{
    public interface IHand2D
    {
        void UseHandData(ref ImageData image, ref HandData hand, int leftOrRight);
    }

    public interface IHand2DFactory
    {
        IHand2D Create(out GameObject obj,int leftOrRight);
    }
}
