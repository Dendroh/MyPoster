using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AnimatorTester : MonoBehaviour
{
    public Toggle AnimToggle;
    public GameObject targetPrefab;
    Animator targetAnimator;

    void Start()
    {
        targetAnimator = targetPrefab.GetComponent<Animator>();
        if (targetAnimator == null)
        {
            Debug.LogError("targetPrefab 에 Animator component가 없습니다. 정지 후 Animator component를 추가해주세요");
        }
        else
        {
            print("Animator :" + targetAnimator);
        }
        AnimToggle.onValueChanged.AddListener((flag) => {
            print("Toggle click :" + flag);
            targetAnimator.SetBool("AnimateSticker", flag);
        });
    }

    void Update()
    {
    }

}
