using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[RequireComponent(typeof(Animator)), DisallowMultipleComponent]
public class StickerPose : MonoBehaviour
{
	[TextArea]
	public string 필수공지사항1 = "- 문의: 박재훈 jhoon.park@alcherainc.com || 010-7284-5995";

	[Header("인식할 모션")]
	public MotionType motionType;

	public enum MotionType
	{
		mouseOpenClose,
		handPaper,
		handVictory,
		handMiniHeart,
		handOK,
		handOne,
		handThumbsUp,
		handPeace,
		handRock,
	}
}