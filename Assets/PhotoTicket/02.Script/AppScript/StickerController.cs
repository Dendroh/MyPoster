using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alchera;
using System.IO;

public class StickerController : MonoBehaviour
{
    [SerializeField] RectTransform framePos;
    [SerializeField] GameObject frameTemplate;
    [SerializeField] MovieDownManager downManager;

    GameObject[] faceTrackables;
    FaceTrackablePrefab[] faceTrackablePrefab;
    FaceMotionDetector[] faceMotionDetectors;

    GameObject[] handTrackables;
    HandTrackablePrefab[] handTrackablePrefab;
    HandMotionDetector[] handMotionDetectors;

    List<int> isGuideNeed = new List<int>();
    float guideDelayTimer = 2.3f;
    float delayTime;
    void Start()
    {
        faceTrackables = GameObject.FindGameObjectsWithTag("FaceTrackable");    //face max Count
        faceTrackablePrefab = new FaceTrackablePrefab[faceTrackables.Length];
        faceMotionDetectors = new FaceMotionDetector[faceTrackables.Length];

        handTrackables = GameObject.FindGameObjectsWithTag("HandTrackable");    //hand max Count *2 (왼쪽 + 오른쪽)
        handTrackablePrefab = new HandTrackablePrefab[handTrackables.Length];
        handMotionDetectors = new HandMotionDetector[handTrackables.Length];

        for (int i = 0; i < faceMotionDetectors.Length; i++)
        {
            faceMotionDetectors[i] = faceTrackables[i].GetComponent<FaceMotionDetector>();
            faceTrackablePrefab[i] = faceTrackables[i].GetComponent<FaceTrackablePrefab>();
        }
        for (int i = 0; i < handMotionDetectors.Length; i++)
        {
            handMotionDetectors[i] = handTrackables[i].GetComponent<HandMotionDetector>();
            handTrackablePrefab[i] = handTrackables[i].GetComponent<HandTrackablePrefab>();
        }
    }
    private void Update()
    {
        if (delayTime > 0)
            delayTime -= Time.deltaTime;    //타이머 공유를 위해  update문에서 관리
    }
    public void SetSticker(int movieNumber) //1부터 시작
    {
        // RemoveAllStickers();
        SetFaceSticker(movieNumber);
        SetHandSticker(movieNumber);
        SetForegroundSticker(movieNumber);
    }

    GameObject[] GetAllStickerPrefabs(string[] assetArray)
    {
        GameObject[] stickerPrefabs = new GameObject[assetArray.Length];
        
        for (int i = 0; i < assetArray.Length; i++)
        {
            var myLoadedPrefabBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, assetArray[i]));
            if (myLoadedPrefabBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return null;
            }

            var downloadRequest = myLoadedPrefabBundle.LoadAsset<GameObject>(assetArray[i]);
            print(downloadRequest);
            stickerPrefabs[i] = Instantiate(downloadRequest);
            myLoadedPrefabBundle.Unload(false);
        }
        return stickerPrefabs;
    }

    void GetAllStickerPrefabs(string[] assetArray, ref GameObject[] stickerPrefabs)
    {
        stickerPrefabs = new GameObject[assetArray.Length];
        print("array Length: " + assetArray.Length);
        for (int i = 0; i < assetArray.Length; i++)
        {
            print("bundle path: " + Path.Combine(Application.persistentDataPath + "/Assetbundles", assetArray[i]));

            string filePath = Path.Combine(Application.persistentDataPath, "Assetbundles", assetArray[i]);
            if (File.Exists(filePath)) {
                // 파일이 존재하면 접근할 수 있음
                var myLoadedPrefabBundle = AssetBundle.LoadFromFile(filePath);
                if (myLoadedPrefabBundle == null) {
                    Debug.Log("Failed to load AssetBundle!");
                    return;
                }
                var downloadRequest = myLoadedPrefabBundle.LoadAsset<GameObject>(assetArray[i]);
                stickerPrefabs[i] = downloadRequest;
                myLoadedPrefabBundle.Unload(false);
            } else {
                Debug.Log("File does not exist: " + filePath);
                // 파일이 존재하지 않을 경우 처리할 내용 추가
            }
        }
    }

    private void SetFaceSticker(int movieNumber)
    {
        Debug.Log("movie info = " + downManager.jsonData.movieInfo);
        //Movie Down Manager에서 스티커 정보 읽어옴
        var stickerInfo = downManager.jsonData.movieInfo;
        int pivotCount = 1;
        GameObject[][] Stickers = new GameObject[pivotCount][];
        // Stickers[0] = GetAllStickerPrefabs(stickerInfo[movieNumber].FaceCenters);
        GetAllStickerPrefabs(stickerInfo[movieNumber].FaceCenters, ref Stickers[0]);
        print(Stickers[0].Length + " DVsfe");
        for (int i = 0; i < faceTrackablePrefab.Length; i++) // maxcount
        {
            for (int j = 0; j < pivotCount; j++) // pivot 이 늘어날 것을 고려하여 for문으로 대체
            {
                for (int k = 0; k < Stickers[j].Length; k++)
                {
                    faceTrackablePrefab[i].SetPivot(Stickers[j][k], j, movieNumber);
                }
            }
        }
    }
    private void SetHandSticker(int movieNumber)
    {
        //Movie Down Manager에서 스티커 정보 읽어옴
        var stickerInfo = downManager.jsonData.movieInfo;
        int pivotCount = 1;
        GameObject[][] Stickers = new GameObject[pivotCount][]; // 현재는 pivot이 1개지만, 언제 늘어날 지 모르니 일단 2차원으로 보류
        // Stickers[0] = GetAllStickerPrefabs(stickerInfo[movieNumber].HandCenters);
        GetAllStickerPrefabs(stickerInfo[movieNumber].HandCenters, ref Stickers[0]);
        for (int i = 0; i < handTrackablePrefab.Length; i++) // maxcount
        {
            var isLeft = i < handTrackablePrefab.Length / 2 ? 0 : 1;//0 0 0 0 1 1 1 1

            for (int j = 0; j < pivotCount; j++) // pivot 이 늘어날 것을 고려하여 for문으로 대체
            {
                //왼손일 때 j 012345, 오른손일때 67891011
                for (int k = 0; k < Stickers[j].Length; k++)    //pivot 하나 당 할당된 스티커 갯수
                {
                    handTrackablePrefab[i].SetPivot(Stickers[j][k], j, movieNumber);
                }
            }
        }
    }

    private void SetForegroundSticker(int movieNumber)
    {
        var foregroundInfo = downManager.jsonData.movieInfo[movieNumber].Foregrounds;
        GameObject[] Foregrounds = new GameObject[foregroundInfo.Length];
        // Foregrounds = GetAllStickerPrefabs(foregroundInfo);
        GetAllStickerPrefabs(foregroundInfo, ref Foregrounds);
        for (int i = 0; i < Foregrounds.Length; i++)
        {
            var frame = Instantiate(Foregrounds[i], framePos);
            try
            {
                var rt = frame.AddComponent<RectTransform>();
                rt.anchorMax = new Vector2(1, 1);
                rt.anchorMin = new Vector2(0, 0);
                rt.offsetMax = new Vector2(0, 0);
                rt.offsetMin = new Vector2(0, 0);
                rt.localScale = new Vector3(100f, 100f, 100f);
                frame.GetComponent<SpriteRenderer>().sortingOrder = i + 10;
            }
            finally
            {
                Debug.LogWarning("다른 형식의 프리팹입니다.");
            }

            frame.tag = "sticker";
            frame.AddComponent<Text>();
            frame.GetComponent<Text>().text = movieNumber.ToString();
        }
    }
    IEnumerator ShowInSeconds(GameObject guide, float delayTimer)
    {
        yield return null;
        delayTime = delayTimer;
        guide.SetActive(true);
        while (delayTime > 0)
        {
            yield return null;
        }
        guide.SetActive(false);
    }


    public void RemoveAllStickers()
    {
        StopCoroutine("ShowInSeconds");

        for (int i = 0; i < faceTrackables.Length; i++)
            foreach (var sticker in faceTrackables[i].transform.GetComponentsInChildren<Transform>(true))
                if (sticker.tag == "sticker" || sticker.tag == "GuideText")
                    Destroy(sticker.gameObject);

        for (int i = 0; i < handTrackables.Length; i++)
            foreach (var sticker in handTrackables[i].transform.GetComponentsInChildren<Transform>(true))
                if (sticker.tag == "sticker" || sticker.tag == "GuideText")
                    Destroy(sticker.gameObject);

        foreach (var frame in framePos.transform.GetComponentsInChildren<Transform>())
            if (frame.tag == "sticker")
                Destroy(frame.gameObject);

        foreach (var i in faceMotionDetectors)
            i.RemoveAllAnimator();

        foreach (var i in handMotionDetectors)
            i.RemoveAllAnimator();
    }

    public void setActiveStickers(int index) {
        // 얼굴 이미지 활성화
        for (int i = 0; i < faceTrackables.Length; i++) {
            foreach (var sticker in faceTrackables[i].transform.GetComponentsInChildren<Text>(true)) {
                if (sticker.tag == "sticker" || sticker.tag == "GuideText") {
                    sticker.gameObject.SetActive(false);

                    if (sticker.GetComponent<Text>().text == index.ToString()) {
                        sticker.gameObject.SetActive(true);
                    }
                }
            }
        }

        // 손 이미지 활성화
        for (int i = 0; i < handTrackables.Length; i++) {
            foreach (var sticker in handTrackables[i].transform.GetComponentsInChildren<Text>(true)) {
                if (sticker.tag == "sticker" || sticker.tag == "GuideText") {
                    sticker.gameObject.SetActive(false);

                    if (sticker.GetComponent<Text>().text == index.ToString()) {
                        sticker.gameObject.SetActive(true);
                    }
                }
            }
        }

        // 배경 이미지 활성화
        foreach (Transform frame in framePos.transform) {
            if (frame.tag == "sticker") {
                frame.gameObject.SetActive(false);   

                if (frame.GetComponent<Text>().text == index.ToString()) {
                    frame.gameObject.SetActive(true);
                }
            }
        }
    }
}
