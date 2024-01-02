using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class FaceMotionDetector : MonoBehaviour
{
    List<Animator> mouseOpenClose;
    List<Animator> eyeOpenClose;

    private void Start()
    {
        mouseOpenClose = new List<Animator>();
        eyeOpenClose = new List<Animator>();
    }

    public unsafe void CheckMotion(Vector2* landmark)
    {
        
        CheckMouseOpenClose(landmark);
        CheckEyeOpenClose(landmark);
    }
    unsafe void CheckMouseOpenClose(Vector2* landmark)
    {
        if (mouseOpenClose.Count == 0) {
            return;
        }
        var Vertical = Vector2.Distance(landmark[98], landmark[102]);
        var Horizontal = Vector2.Distance(landmark[96], landmark[100]);
        
        if (Vertical / Horizontal > 0.3f)
        {
            //open
            for (int i = 0; i < mouseOpenClose.Count; i++)
            {
                mouseOpenClose[i].SetBool("AnimateSticker", true);
            }
        }
        else
        {
            //close
            for (int i = 0; i < mouseOpenClose.Count; i++)
            {
                mouseOpenClose[i].SetBool("AnimateSticker", false);
            }
        }
    }
    unsafe void CheckEyeOpenClose(Vector2* landmark)
    {
        var Vertical = Vector2.Distance(landmark[75], landmark[76]);
        var Horizontal = Vector2.Distance(landmark[58], landmark[61]);

        if (Vertical / Horizontal > 0.4f)
        {
            //open
        }
        else
        {
            //close
        }
    }
    public void AddAnimator(Animator animator, int motionType)
    {
        switch (motionType)
        {
            case 0: mouseOpenClose.Add(animator); break;
            case 1: eyeOpenClose.Add(animator); break;
            default: break;
        }
    }
    public void RemoveAllAnimator()
    {
        mouseOpenClose.Clear();
        eyeOpenClose.Clear();
    }
}