using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Screenshot : MonoBehaviour
{

    //     Texture2D screenCap;
    //     public Material material;
    //     Texture2D border;
    //     bool shot = false;
    //     public Image image;
    //     void Start()
    //     {
    //         screenCap = new Texture2D(926, 1231, TextureFormat.RGB24, false);
    //         border = new Texture2D(2, 2, TextureFormat.ARGB32, false);
    //         border.Apply();
    //     }


    //     void Update()
    //     {
    //         if (Input.GetKeyUp(KeyCode.Space))
    //         {
    //             StartCoroutine("Capture");
    //         }

    //     }
    //     IEnumerator Capture()
    //     {
    //         yield return new WaitForEndOfFrame();
    //         screenCap.ReadPixels(new Rect(77, 539, 926, 1231), 0, 0);
    //         screenCap.Apply();
    //         // material.mainTexture = screenCap;
    //         image.sprite = Sprite.Create(screenCap, new Rect(0, 0, 627, 1231),Vector2);
    //     }
    //     void onGui()
    //     {
    //         // GUI.DrawTexture(new Rect(0, 0, 926, 1231))

    //     }
}
