using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVGImporter;
using UnityEngine.UI;
public class NumpadButton : MonoBehaviour
{
    SVGFrameAnimator fa;
    SVGImage buttonImage;
    Text number;

    void Start()
    {
        fa = GetComponent<SVGFrameAnimator>();
        buttonImage = GetComponent<SVGImage>();

        if (this.gameObject.name != ("Image (10)"))
        {
            number = GetComponentInChildren<Text>();
        }
    }
    //on clicked
    public void onDown()
    {
        buttonImage.vectorGraphics = fa.frames[1];
        if (this.gameObject.name != ("Image (10)"))
        {
            number.color = new Color32(174, 0, 0, 255);
        }
    }
    //on clickExited
    public void onExit()
    {
        buttonImage.vectorGraphics = fa.frames[0];
        if (this.gameObject.name != ("Image (10)"))
        {
            number.color = new Color32(255, 255, 255, 255);
        }
    }
}
