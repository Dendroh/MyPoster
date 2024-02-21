using System;
using UnityEngine;

namespace Alchera
{
    public interface IFace3D
    {
        //Pose HeadPose{set;}
        void SetEyeRotation(ref Quaternion left, ref Quaternion right);

        unsafe void SetAnimation(float* weights);
        void UseFaceData(ref ImageData image, ref FaceData face);
    }

    public interface IFace3DFactory
    {
        IFace3D Create(out GameObject obj);
    }
}
