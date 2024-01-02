﻿// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
//  Author      sg.ju@alcherainc.com
//
// ---------------------------------------------------------------------------
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NatCorder;
using NatCorder.Clocks;

namespace Alchera
{
    /// <summary>
    /// Myposter에서 알체라 SDK를 사용하는 예제 클래스
    /// 
    /// PhotoCanvas 인 경우 인식 상태에 따른 처리가 있어 PhotoCanas 일 때의 flow를 담당하기도 합니다.
    /// 지속적 & 비동기적으로 [텍스쳐받기 -> 이미지데이터변환 -> Detection] 을 수행합니다.
    /// </summary>

    public class ComplexSceneBehavior : MonoBehaviour, ISceneBehavior
    {
        ITextureSequence sequence;
        ITextureConverter converter;

        IDetectService faceService;
        IDetectService handService;

        IFaceListConsumer faceConsumer;
        IHandListConsumer handConsumer;
        [SerializeField] GameObject FaceConsumer = null;///< Detection 결과를 이용하는 객체. FaceData를 직접 이용하는 것은 아니고, multiFace를 관리해 주는 중간자 역할
        [SerializeField] GameObject HandConsumer = null;///< Detection 결과를 이용하는 객체. HandData를직접 이용하는 것은 아니고, multiHand를 관리해 주는 중간자 역할

        [SerializeField] ResultUIScript result_Script;
        [SerializeField] GameObject jpgResult;
        [SerializeField] GameObject gifResult;
        [SerializeField] AudioSource resultAudioKr;
        [SerializeField] AudioSource resultAudioEn;
        [SerializeField] AudioSource pictureAudio;

        // 숫자 카운트 안내 음성
        [SerializeField] AudioSource[] countAudioKr;
        [SerializeField] AudioSource[] countAudioEn;

        Moments.Recorder recoder; //화면 녹화용 레코더
        SaveLastTexture textureSaver;
        PhotoUIScript photoCanvas;
        Coroutine videoCoroutine;

        bool isCapturing;
        bool isPhotoTaken;
        public float posingWatingTime; ///< 촬영 전 대기 count. 마지막 1초 내외를 gif 촬영, 마지막 프레임을 jpg로 촬영하며, 도중에 인식 실패 시 초기화됩니다. 
        float posingTimer;

        bool isInitMP4Recorder;
        MP4Recorder videoRecorder;
        IClock clock;
        Color32[] pixelBuffer;

        void Awake()
        {
            sequence = this.GetComponent<ITextureSequence>();
            converter = this.GetComponent<ITextureConverter>();
            textureSaver = this.GetComponent<SaveLastTexture>();

            var detectors = this.GetComponents<IDetectService>();
            Debug.Log("Number of detectors : " + detectors.Length);
            faceService = detectors[0];
            handService = detectors[1];

            faceConsumer = FaceConsumer.GetComponent<IFaceListConsumer>();
            handConsumer = HandConsumer.GetComponent<IHandListConsumer>();
        }

        public async void Start()  //detect 로직 수행 중 
        {
            recoder = this.GetComponent<Moments.Recorder>();

            photoCanvas = FlowController.instance.photoCanvas.GetComponent<PhotoUIScript>();
            recoder.OnFileSaved = (int id, string filePath) => {
                result_Script.OnFileSaved();
            };
            recoder.OnFileSaveProgress = (int id, float progress) =>
            {
                result_Script.OnFileSaveProgress(progress);
            };

            // start a logic loop
            IEnumerable<FaceData> faces = null;
            IEnumerable<HandData> hands = null;

            try
            {
                foreach (Task<Texture> request in sequence.RepeatAsync())
                {
                    int detectCount = 0;
                    var texture = await request;

                    var image = await converter.ConvertAsync(texture);
                    var faceTranslator = await faceService.DetectAsync(ref image);
                    faces = faceTranslator.Fetch<FaceData>(faces);

                    if (faces != null)
                    {
                        foreach (var item in faces)
                            detectCount++;

                        faceConsumer.Consume(ref image, faces);
                    }
                    //release holding resources for detection
                    faceTranslator.Dispose();

                    image = await converter.ConvertAsync(texture);
                    var handTranslator = await handService.DetectAsync(ref image);
                    hands = handTranslator.Fetch<HandData>(hands);
                    if (hands != null)
                    {
                        foreach (var item in hands)
                            detectCount++;

                        handConsumer.Consume(ref image, hands);
                    }
                    //release holding resources for detection
                    handTranslator.Dispose();

                    if (detectCount > 0)    //1개라도 인식하면 진행
                    {
                        FlowController.instance.timer = 0; //처음 화면으로 되돌아가지 않음
                        if (!isCapturing)    //촬영 버튼 누르지 않으면 아래 촬영코드를 수행하지 않는다. 디텍팅은 하고 잇음 
                            continue;

                        for (int i = 4; i >= 0; i--) {
                            if ((int)posingTimer == i + 1) {
                                StartCoroutine(playAudioList(countAudioKr, countAudioEn, i));
                            }
                        }

                        if (posingTimer < 0.1f && isPhotoTaken == false)    //이미지 저장. 한번만 동작한다.
                        {
                            isPhotoTaken = true;

                            if (recoder != null)// photo/1.jpg
                            {
                                StartCoroutine(recoder.SavePhoto(jpgResult.GetComponent<RawImage>(), "1"));
                                await textureSaver.SaveTexture(texture, "1_raw");
                            }
                        }
                        else if (posingTimer < 0)   // GIF 저장
                        {
                            // 결과 안내 멘트 출력
                            StartCoroutine(playAudioWaitSeconds(resultAudioKr, resultAudioEn, 3));

                            StartCoroutine(UtilsScript.playEffectAudio(pictureAudio));  // 찰칵 효과음 출력

                            isCapturing = false;
                            var frameCnt = 0;
                            Queue<Texture2D> mp4Frames = recoder.m_Frames;

                            foreach (Texture2D m_FramesTexture2D in mp4Frames)
                            {
                                if (!isInitMP4Recorder)
                                {
                                    isInitMP4Recorder = true;
                                    clock = new RealtimeClock();

                                    videoRecorder = new MP4Recorder(
                                        m_FramesTexture2D.width,
                                        m_FramesTexture2D.height,
                                        20,
                                        0,
                                        0,
                                        recordingPath =>
                                        {
                                            Debug.Log($"Saved recording to: {recordingPath}");
#if UNITY_IPHONE || UNITY_ANDROID
                                            var prefix = Application.platform == RuntimePlatform.IPhonePlayer ? "file://" : "";
                                            Handheld.PlayFullScreenMovie($"{prefix}{recordingPath}");
#endif
                                        }
                                    );
                                }

                                pixelBuffer = m_FramesTexture2D.GetPixels32();
                                videoRecorder?.CommitFrame(pixelBuffer, clock.Timestamp + (50000000L * frameCnt));

                                frameCnt++;
                            }

                            videoRecorder.Dispose();
                            videoRecorder = null;

                            if (recoder != null)
                            {
                                // /photo/1.gif
                                recoder.Save("1");

                                //photo is saved in Save function   
                                videoCoroutine = StartCoroutine(PlayVideo());
                            }
                            FlowController.instance.ChangeFlow(FlowController.instance.resultCanvas);
                        }
                    }
                    else
                    {
                        // 숫자 카운트 음성 멘트 초기화
                        for (int i = 0; i < 5; i++) {
                            countAudioKr[i].Stop();
                            countAudioEn[i].Stop();
                        }

                        posingTimer = posingWatingTime; //촬영 중 인식 실패 시 타이머 초기화
                                                        //Recorder가 Save호출하기 이전 최근의 Record Time 길이의 영상 저장하므로 수동 flush는 불필요
                    }

                    photoCanvas.SetPhotoCountText(posingTimer, posingWatingTime); //화면에 보여지는 3,2,1, 카운터
                }
            }

            catch (TaskCanceledException e)
            {
                Debug.LogWarning(e.ToString());

                sequence.Dispose();
                faceService.Dispose();
                handService.Dispose();

                // For now, there is NO way to recover for this kind of exception ...
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                Application.Quit();
            }
        }
        /**
        PhotoCanvas 일때 값을 초기화합니다.
         */
        public void Init()
        {
            isCapturing = false;
            posingTimer = posingWatingTime;
        }
        /**
        촬영 버튼을 눌렀을 시 호출됩니다.
         */
        public void StartTimer()
        {
            isCapturing = true;
            isPhotoTaken = false;
            isInitMP4Recorder = false;
            recoder.FlushMemory();

            foreach (var item in GameObject.FindGameObjectsWithTag("GuideText")) //촬영버튼 누를 시 모든 텍스트 가이드 제거
            {
                item.SetActive(false);
            }

            recoder = GetComponent<Moments.Recorder>();

            if (recoder != null)
            {
                recoder.Record();
            }
        }
        void Update()
        {
            if (isCapturing) {
                posingTimer -= Time.deltaTime;  //3,2,1숫자 셀 카운터
            }
        }
        IEnumerator PlayVideo()
        {
            yield return new WaitForSeconds(0.5f);
            DateTime startPlay = DateTime.Now;

            while (true)
            {
                var dt = DateTime.Now - startPlay;
                var record = GetComponent<Moments.Recorder>();
                record.ShowVideo(gifResult.GetComponent<RawImage>(), dt.TotalSeconds);
                yield return new WaitForSeconds(0.033f);
            }
        }
        /**
        촬영 후 video 재생을 막습니다.
       */
        public void StopPlayVideo()
        {
            recoder.FlushMemory();

            if (videoCoroutine != null)
                StopCoroutine(videoCoroutine);
        }

        void OnDestroy()
        {
            // ensure disposal
            //sequence.Dispose();
            //handService.Dispose();

            Debug.LogWarning("ComplexSceneBehavior");
        }

        /**
         * 언어 모드에 따른 오디오 출력(지연 출력)
         * @param audioKr   국문 음성
         * @param audioEn   영문 음성
         * @param seconds   지연 시간
         * @return IEnumerator
         */
        IEnumerator playAudioWaitSeconds(AudioSource audioKr, AudioSource audioEn, int seconds) {
            yield return new WaitForSeconds(seconds);

            // 언어모드에 따른 오디오 출력 설정
            switch (UtilsScript.checkConfig()) {
                case "kr": audioKr.Play(); break;
                case "en": audioEn.Play(); break;
            }
        }

        /**
         * 언어 모드에 따른 오디오 목록 출력
         * @param audioKr   국문 음성 목록
         * @param audioEn   영문 음성 목록
         * @param index
         * @return IEnumerator
         */
        IEnumerator playAudioList(AudioSource[] audioKr, AudioSource[] audioEn, int index) {
            // 언어모드에 따른 오디오 출력 설정
            switch (UtilsScript.checkConfig()) {
                case "kr": audioKr[index].Play(); break;
                case "en": audioEn[index].Play(); break;
            }

            yield return null;
        }
    }
}
 
 
 
 
 