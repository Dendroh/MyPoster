using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alchera;
public class IntroUIScript : MonoBehaviour, UIScript
{
	[SerializeField] float changeTime = 5;
	[SerializeField] Image background;
	[SerializeField] Image thumbNail;
	[SerializeField] Button transparentTouch;
	[SerializeField] MovieDownManager downManager;
	[SerializeField] Sprite[] idlePosters;
	[SerializeField] AudioSource introAudioKr;
	[SerializeField] AudioSource introAudioEn;

	Animation idleAnimation;
	ReadWebcamInSequence camReader;
	int posterIndex = 1;
	float timerChangeScean = 0;
	float timerAudio = 10;
	float second = 10;
	bool isIntroUIInitialized = false;
	bool isIntroCanvas = true;

	void Start() {
		idleAnimation = GetComponent<Animation>();
		camReader = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ReadWebcamInSequence>();

		thumbNail.sprite = idlePosters[1 % idlePosters.Length];
		transparentTouch.onClick.AddListener(() => {
			introAudioKr.enabled = false;
			introAudioEn.enabled = false;
			if (PlayerPrefs.GetString("quiz") == "true") {
				FlowController.instance.ChangeFlow(FlowController.instance.quizCanvas);
			} else {
				FlowController.instance.ChangeFlow(FlowController.instance.selectCanvas);
			}
		});
	}

	public void InstantiatePoster() {
		Debug.Log("InstantiatePoster");
		background.sprite = downManager.idlePosterSprites[0];
		thumbNail.sprite = downManager.idlePosterSprites[1 % downManager.idlePosterSprites.Length];
		isIntroUIInitialized = true;
	}

	void Update() {
		if (isIntroUIInitialized == false)
			return;
		if (isIntroCanvas == false)
			return;
		if (idlePosters.Length == 1)
			return;

		string introAudio = PlayerPrefs.GetString("introAudio");
		string lang = PlayerPrefs.GetString("lang");

		// 타이머 동작
		timerChangeScean += Time.deltaTime;
		if (timerChangeScean > changeTime) {
			UpdateNextImage();

			timerChangeScean = 0;
		}

		// 기본 - 10초마다 루프 동작(러닝 타임 포함)
		if (introAudio == "true") {
			timerAudio += Time.deltaTime;
			if (timerAudio > second) {
				if (lang == "kr") {
					if (introAudioKr && introAudioKr.isActiveAndEnabled) {
						introAudioKr.Play();   // 음성 출력
					}
				} else if (lang == "en") {
					if (introAudioEn && introAudioEn.isActiveAndEnabled) {
						introAudioEn.Play();   // 음성 출력
					}
				}

				timerAudio = 0; // 타이머 초기화
			}
		}
	}

	public void Init() {
		print("IntroInit");
		isIntroCanvas = true;
		if (PlayerPrefs.GetString("introAudio") == "true") {
			introAudioKr.enabled = true;
			introAudioEn.enabled = true;
		}

		if (PlayerPrefs.GetString("quiz") != "true") {  // 퀴즈 모드가 아닌 경우, 포토 모드에서만 사용
			camReader.changeCameraState(false);
		}

		FlowController.instance.currentMovieNumber = -1;
	}

	public void Dispose() {
		isIntroCanvas = false;
		print("DisposeInit");
	}

	public void UpdateNextImage() {
		// 배경 설정 2개 이상인 경우 애니메이션 적용
		if (downManager.idlePosterSprites.Length > 1) {
			thumbNail.sprite = downManager.idlePosterSprites[posterIndex];
			idleAnimation.Play();
		}
	}

	public void ResetAnim() {
		// 배경 설정 2개 이상인 경우 애니메이션 적용
		if (downManager.idlePosterSprites.Length > 1) {
			background.sprite = downManager.idlePosterSprites[posterIndex];
			posterIndex = (posterIndex + 1) % (downManager.idlePosterSprites.Length);
		}
	}
}
