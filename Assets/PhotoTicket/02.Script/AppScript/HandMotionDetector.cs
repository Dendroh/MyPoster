using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Alchera;
public class HandMotionDetector : MonoBehaviour
{
    List<Animator> Paper;
    List<Animator> Victory;
    List<Animator> MiniHeart;
    List<Animator> OK;
    List<Animator> One;
    List<Animator> ThumbsUp;
    List<Animator> Peace;

    List<Animator> Rock;

    private void Start()
    {
        Paper = new List<Animator>();
        Victory = new List<Animator>();
        MiniHeart = new List<Animator>();
        OK = new List<Animator>();
        One = new List<Animator>();
        ThumbsUp = new List<Animator>();
        Peace = new List<Animator>();

        Rock = new List<Animator>();
    }
    public void AddAnimator(Animator animator, int motionType)
    {
        switch (motionType)
        {
            case 1: Paper.Add(animator); break;
            case 2: Victory.Add(animator); break;
            case 3: MiniHeart.Add(animator); break;
            case 4: OK.Add(animator); break;
            case 5: One.Add(animator); break;
            case 6: ThumbsUp.Add(animator); break;
            case 7: Peace.Add(animator); break;
            case 8: Rock.Add(animator); break;
            default: break;
        }
    }
    public void RemoveAllAnimator()
    {
        Paper.Clear();
        Victory.Clear();
        MiniHeart.Clear();
        OK.Clear();
        One.Clear();
        ThumbsUp.Clear();
        Peace.Clear();

        Rock.Clear();
    }
    public void CheckPosture(PostureCode posture)
    {
        CheckPaperIfNeed(posture);
        CheckVictoryIfNeed(posture);
        CheckMiniHeartIfNeed(posture);
        CheckOKIfNeed(posture);
        CheckOneIfNeed(posture);
        CheckThumbsUpIfNeed(posture);
        CheckPeaceIfNeed(posture);

        CheckRockIfNeed(posture);
    }

    void CheckRockIfNeed(PostureCode posture)
    {
        var count = 0;
        if (posture == PostureCode.rock) count++;

        SetAnimationState(count, ref Rock);
    }

    void CheckPaperIfNeed(PostureCode posture)
    {
        var count = 0;
        if (posture == PostureCode.paper) count++;

        SetAnimationState(count, ref Paper);
    }

    void CheckVictoryIfNeed(PostureCode posture)
    {
        var count = 0;
        if (posture == PostureCode.v) count++;
        //if (posture == PostureCode.one) count++;
        //if (posture == PostureCode.gun) count++;

        SetAnimationState(count, ref Victory);
    }

    void CheckMiniHeartIfNeed(PostureCode posture)
    {
        var count = 0;
        if (posture == PostureCode.mini_heart) count++;
        //if (posture == PostureCode.rock) count++;

        SetAnimationState(count, ref MiniHeart);
    }

    void CheckOKIfNeed(PostureCode posture)
    {
        var count = 0;
        if (posture == PostureCode.okay) count++;

        SetAnimationState(count, ref OK);
    }

    void CheckOneIfNeed(PostureCode posture)
    {
        var count = 0;
        if (posture == PostureCode.one) count++;

        SetAnimationState(count, ref One);
    }

    void CheckThumbsUpIfNeed(PostureCode posture)
    {
        var count = 0;
        if (posture == PostureCode.thumbs_up) count++;
        //if (posture == PostureCode.one) count++;

        SetAnimationState(count, ref ThumbsUp);
    }

    void CheckPeaceIfNeed(PostureCode posture)
    {
        var count = 0;
        if (posture == PostureCode.peace) count++;
        //if (posture == PostureCode.gun) count++;

        SetAnimationState(count, ref Peace);
    }

    void SetAnimationState(int count, ref List<Animator> anims)
    {
        if (count > 0)
        {
            //print("activate");
            for (int i = 0; i < anims.Count; i++)
            {
                anims[i].SetBool("AnimateSticker", true);
            }
        }
        else
        {
            //print("deactivate");
            for (int i = 0; i < anims.Count; i++)
            {
                anims[i].SetBool("AnimateSticker", false);
            }
        }
    }

}
