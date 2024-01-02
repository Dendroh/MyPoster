// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
//  Author      dh.park@alcherainc.com
//
// ---------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alchera
{
    public class Draw3DAnimoji : MonoBehaviour, IFace3DFactory, IFaceListConsumer
    {
        const int Capacity = 19;

        public GameObject Parent;
        public GameObject Prefab;

        IFace3D[] Faces;
        GameObject[] pool;
        ImageData imageInfo;

        bool need3D;
        bool needMirror;
        int maxCount;

        void Awake()
        {
            maxCount = GetComponent<FaceService>().maxCount;
            need3D = GetComponent<FaceService>().need3D;
            Faces = new IFace3D[Capacity];
            pool = new GameObject[Capacity];
            for (int i = 0; i < maxCount; i++)
            {
                Faces[i] = Create(out pool[i]);
                pool[i].transform.parent = Parent.transform;
            }
        }

        public IFace3D Create(out GameObject obj)
        {
            obj = Instantiate(Prefab);
            var Face = obj.GetComponent<IFace3D>();
            if (Face == null)
                throw new UnityException(string.Format(
                    "IHead3D not found with prefab: {0}", Prefab.name));
            return Face;
        }

        public unsafe void Consume(ref ImageData image, IEnumerable<FaceData> list)
        {
            if (need3D == false)
            {
                Debug.LogError("Need3D should be checked for using Draw3DAnimoji");
                return;
            }
            for (int k = 0; k < maxCount; k++)
            {
                pool[k].SetActive(false);
            }

            int i = 0;

            FaceData face = default(FaceData);
            foreach (var item in list)
            {
                pool[i].SetActive(true); 
                face = item;
                Faces[i].UseFaceData(ref image, ref face);
                i++;
            }
        }


    }
}
