using System;
using UnityEngine;

namespace Alchera
{
    public interface IFace2D
    {
        void UseFaceData(ref ImageData image, ref FaceData face);
    }

    public interface IFace2DFactory
    {
        IFace2D Create(out GameObject obj);
    }
}
