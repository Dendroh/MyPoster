﻿using System;
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
		SetFaceSticker(movieNumber);
		SetHandSticker(movieNumber);
		SetForegroundSticker(movieNumber);
	}

	void GetAllStickerPrefabs(string[] assetArray, ref GameObject[] stickerPrefabs)
	{
		stickerPrefabs = new GameObject[assetArray.Length];
		for (int i = 0; i < assetArray.Length; i++)
		{
			string filePath = Path.Combine(Application.persistentDataPath, "Assetbundles", assetArray[i]);
			try
			{
				if (File.Exists(filePath))
				{
					var myLoadedPrefabBundle = AssetBundle.LoadFromFile(filePath);
					if (myLoadedPrefabBundle == null)
					{
						print("포스터 스티커 파일로드 중 예외발생 filePath :" + filePath);
						return;
					}
					var downloadRequest = myLoadedPrefabBundle.LoadAsset<GameObject>(assetArray[i]);
					stickerPrefabs[i] = downloadRequest;
					myLoadedPrefabBundle.Unload(false);
				} else
				{
					print("포스터 스티커 파일이 존재하지 않는 예외발생 filePath : " + filePath);
					return;
				}
			} catch
			{
				print("포스터 스티커 로딩 중 예외발생 filePath :" + filePath);
				return;
			}
		}
	}

	private void SetFaceSticker(int movieNumber)
	{
		var stickerInfo = downManager.jsonData.movieInfo;
		int pivotCount = 1;
		GameObject[][] Stickers = new GameObject[pivotCount][];
		GetAllStickerPrefabs(stickerInfo[movieNumber].FaceCenters, ref Stickers[0]);
		for (int i = 0; i < faceTrackablePrefab.Length; i++)
		{
			for (int j = 0; j < pivotCount; j++)
			{
				for (int k = 0; k < Stickers[j].Length; k++)
				{
					if (Stickers[j][k] != null)
						faceTrackablePrefab[i].SetPivot(Stickers[j][k], j, movieNumber);
				}
			}
		}
	}
	private void SetHandSticker(int movieNumber)
	{
		var stickerInfo = downManager.jsonData.movieInfo;
		int pivotCount = 1;
		GameObject[][] Stickers = new GameObject[pivotCount][];
		GetAllStickerPrefabs(stickerInfo[movieNumber].HandCenters, ref Stickers[0]);
		for (int i = 0; i < handTrackablePrefab.Length; i++)
		{
			for (int j = 0; j < pivotCount; j++)
			{
				for (int k = 0; k < Stickers[j].Length; k++)
				{
					if (Stickers[j][k] != null)
						handTrackablePrefab[i].SetPivot(Stickers[j][k], j, movieNumber);
				}
			}
		}
	}

	private void SetForegroundSticker(int movieNumber)
	{
		var foregroundInfo = downManager.jsonData.movieInfo[movieNumber].Foregrounds;
		GameObject[] Foregrounds = new GameObject[foregroundInfo.Length];
		GetAllStickerPrefabs(foregroundInfo, ref Foregrounds);
		for (int i = 0; i < Foregrounds.Length; i++)
		{
			if (Foregrounds[i] != null)
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
				} catch (Exception e)
				{
					Debug.Log(e);
				}

				frame.tag = "sticker";
				frame.AddComponent<Text>();
				frame.GetComponent<Text>().text = movieNumber.ToString();
			}
		}
	}

	public void setActiveStickers(int index)
	{
		// 얼굴 이미지 활성화
		for (int i = 0; i < faceTrackables.Length; i++)
		{
			foreach (var sticker in faceTrackables[i].transform.GetComponentsInChildren<Text>(true))
			{
				if (sticker.tag == "sticker" || sticker.tag == "GuideText")
				{
					sticker.gameObject.SetActive(false);

					if (sticker.GetComponent<Text>().text == index.ToString())
					{
						sticker.gameObject.SetActive(true);
					}
				}
			}
		}

		// 손 이미지 활성화
		for (int i = 0; i < handTrackables.Length; i++)
		{
			foreach (var sticker in handTrackables[i].transform.GetComponentsInChildren<Text>(true))
			{
				if (sticker.tag == "sticker" || sticker.tag == "GuideText")
				{
					sticker.gameObject.SetActive(false);

					if (sticker.GetComponent<Text>().text == index.ToString())
					{
						sticker.gameObject.SetActive(true);
					}
				}
			}
		}

		// 배경 이미지 활성화
		foreach (Transform frame in framePos.transform)
		{
			if (frame.tag == "sticker")
			{
				frame.gameObject.SetActive(false);

				if (frame.GetComponent<Text>().text == index.ToString())
				{
					frame.gameObject.SetActive(true);
				}
			}
		}
	}
}