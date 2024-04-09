using System;
using System.Collections;
using UnityEngine;

public class FlowController : MonoBehaviour
{
	public static FlowController instance;
	public bool drawStickerGuide;
	public bool removeFalseAlarm;
	[SerializeField] float noTouchTime;
	[SerializeField] float Dimmingspeed;
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
	public CanvasGroup quizCanvas;
	public CanvasGroup quizResultCanvas;
	public CanvasGroup promotionCanvas;
	bool IsDimming;

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
		StartCoroutine(SetCanvasActive(quizCanvas, false));
		StartCoroutine(SetCanvasActive(quizResultCanvas, false));
		StartCoroutine(SetCanvasActive(promotionCanvas, false));
	}

	void Update()
	{
		if (NetClientErrorCanvas.activeSelf == true)
			if (Input.GetMouseButtonDown(0))
			{
				timer = 0; //화면을 터치하면 대기시간 초기화
			}

		if (
			currentCanvas == sendCanvas || 
			currentCanvas == paymentCanvas || 
			currentCanvas == introCanvas || 
			currentCanvas == resultCanvas || 
			currentCanvas == AgreementCanvas || 
			(currentCanvas == quizCanvas && QuizUIScript.quizProcess == true)
			)
		{
			return; //결제할 때는 시간이 흐르지 않는다.
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
}
