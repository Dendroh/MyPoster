﻿using System;
using System.Collections;
using UnityEngine;

public class FlowController : MonoBehaviour
{
	public static FlowController instance;
	[Tooltip("체크 시 얼굴에 디버그 가이드도 같이 보여줍니다.")]
	public bool drawStickerGuide;
	[Tooltip("체크 False Alarm을 앱에서 추가연산합니다.")]
	public bool removeFalseAlarm;
	[Tooltip("이 시간동안 반응이 없으면 대기화면으로 돌아갑니다.")]
	[SerializeField] float noTouchTime;
	[Tooltip("씬이 전환될 때 디졸브 효과의 속도입니다. 높을수록 빠르게 전환됩니다.")]
	[SerializeField] float Dimmingspeed;
	[Tooltip("로딩창입니다. 로딩창이 켜져있는 동안에는 유저가 화면을 클릭해도 아무런 반응이 없습니다.")]
	[SerializeField] GameObject LoadingCanvas;
	[SerializeField] GameObject NetClientErrorCanvas;
	[SerializeField] GameObject isReconnectPopup;
	[SerializeField] GameObject failReconnectPopup;

	[HideInInspector] public float timer;
	public int currentMovieNumber;
	public string currentMovieId;
	[HideInInspector] public CanvasGroup beforeCanvas;
	[HideInInspector] public CanvasGroup currentCanvas;

	[Space]
	public CanvasGroup introCanvas;
	public CanvasGroup selectCanvas;
	public CanvasGroup photoCanvas;
	public CanvasGroup resultCanvas;
	public CanvasGroup paymentCanvas;
	public CanvasGroup sendCanvas;
	public CanvasGroup endCanvas;
	public CanvasGroup AgreementCanvas;
	public CanvasGroup networkCheckCanvas;
	public CanvasGroup quizCanvas;
	public CanvasGroup quizResultCanvas;
	public CanvasGroup promotionCanvas;
	bool IsDimming;

	// 네트워크 체크 타임
	int checkTime = 1;
	float waitTime = 0.5f;
	float checkTimer;

	void Awake()
	{
		if (!instance)
			instance = this;
	}

	void Start()
	{
		Screen.SetResolution(1080, 1920, true);

		currentCanvas = introCanvas;

		Loading(false);
		StartCoroutine(SetCanvasActive(introCanvas, true));
		StartCoroutine(SetCanvasActive(selectCanvas, false));
		StartCoroutine(SetCanvasActive(photoCanvas, false));
		StartCoroutine(SetCanvasActive(resultCanvas, false));
		StartCoroutine(SetCanvasActive(AgreementCanvas, false));
		StartCoroutine(SetCanvasActive(sendCanvas, false));
		StartCoroutine(SetCanvasActive(endCanvas, false));
		StartCoroutine(SetCanvasActive(paymentCanvas, false));
		StartCoroutine(SetCanvasActive(networkCheckCanvas, false));
		StartCoroutine(SetCanvasActive(quizCanvas, false));
		StartCoroutine(SetCanvasActive(quizResultCanvas, false));
		StartCoroutine(SetCanvasActive(promotionCanvas, false));
		StartCoroutine(IsInternetReachable());
	}
	public void Loading(bool flag)  //로딩 창 필요시에 생성하여 터치를 막는다.
	{
		LoadingCanvas.SetActive(flag);
	}

	public void ChangeFlow(CanvasGroup canvas)
	{
		if (IsDimming)
			return;
		IsDimming = true;
		currentCanvas.GetComponent<UIScript>().Dispose();
		StartCoroutine(SetCanvasActive(currentCanvas, false));

		beforeCanvas = currentCanvas;
		currentCanvas = canvas;

		StartCoroutine(SetCanvasActive(currentCanvas, true));
		currentCanvas.GetComponent<UIScript>().Init();
	}
	IEnumerator SetCanvasActive(CanvasGroup cg, bool flag) //활성화되는 canvas를 선택한다. 디졸브를 위해 코루틴으로 실행
	{
		cg.interactable = flag;
		cg.blocksRaycasts = flag;
		if (flag)
		{
			while (cg.alpha < 1)
			{
				cg.alpha += Time.deltaTime * Dimmingspeed * 10;
				yield return null;
			}
		} else
		{
			while (cg.alpha > 0)
			{
				cg.alpha -= Time.deltaTime * Dimmingspeed * 10;
				yield return null;
			}
		}
		IsDimming = false;
	}
	void Update()
	{
		if (NetClientErrorCanvas.activeSelf == true)
			if (Input.GetMouseButtonDown(0))
			{
				timer = 0; //화면을 터치하면 대기시간 초기화
			}

		if (currentCanvas == sendCanvas || currentCanvas == paymentCanvas || currentCanvas == introCanvas || currentCanvas == resultCanvas || currentCanvas == AgreementCanvas || currentCanvas == networkCheckCanvas)
		{
			return; //결제할 때는 시간이 흐르지 않는다.
		}

		// 퀴즈 화면의 경우, 퀴즈 진행 중일 때만 인트로 전환 안함
		if (currentCanvas == quizCanvas && QuizUIScript.quizProcess == true)
		{
			return;
		}

		timer += Time.deltaTime;

		string autoReturn = PlayerPrefs.GetString("autoReturn");
		if (autoReturn == "true")
		{
			if (noTouchTime < timer)
			{
				ChangeFlow(introCanvas);
				timer = 0;
			}
		}
	}

	IEnumerator IsInternetReachable()
	{
		WaitForSeconds seconds = new WaitForSeconds(waitTime);

		while (true)
		{
			yield return seconds;

			// 인터넷 연결이 끊긴 경우 에러 화면 전환
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				ChangeFlow(networkCheckCanvas);
				yield break;
			}

			checkTimer += waitTime;
			if (checkTimer > checkTime)
			{    // 일정 시간 체크 후 코루틴 종료
				yield break;
			}
		}
	}
}
