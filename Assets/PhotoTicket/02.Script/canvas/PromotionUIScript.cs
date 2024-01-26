using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PromotionUIScript : MonoBehaviour, UIScript
{
	[SerializeField] Toggle sendMessageToggle;
	[SerializeField] Image resultImage;
	[SerializeField] Text resultText;
	[SerializeField] AudioSource toggleAudio;
	[SerializeField] GameObject advCanvas;

	void Start()
	{
		sendMessageToggle.onValueChanged.AddListener(delegate
		{
			agreeValueChanged();
		});
	}

	public void Init()
	{
		advCanvas.gameObject.SetActive(false);  // 광고 화면 블라인드
	}

	public void Dispose()
	{
		sendMessageToggle.isOn = false;
		advCanvas.gameObject.SetActive(true);  // 광고 화면 활성화
	}

	public void agreeValueChanged()
	{
		// 토글 클릭 효과음 출력
		StartCoroutine(UtilsScript.playEffectAudio(toggleAudio));
	}

	public void sendPoster()
	{
		if (sendMessageToggle.isOn)
		{
			PlayerPrefs.SetString("bSmsSend", "true");
		} else
		{
			PlayerPrefs.SetString("bSmsSend", "false");
		}

		FlowController.instance.ChangeFlow(FlowController.instance.sendCanvas);
	}

	/**
	 * 서버로부터 이미지 정보 가져와 이미지 생성하기
	 * @param filePathList
	 * @param rectTransform  이미지 영역 크기
	 */
	IEnumerator addImages(List<string> filePathList, RectTransform rectTransform)
	{
		// 이미지 크기 설정
		float imageWidth = rectTransform.rect.width;
		float imageHeight = rectTransform.rect.height;

		for (int i = 0; i < filePathList.Count(); i++)
		{
			using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(filePathList[i]))
			{    // 서버로부터 이미지 정보 가져오기
				yield return unityWebRequest.SendWebRequest();
				if (unityWebRequest.result != UnityWebRequest.Result.ConnectionError)
				{  // 성공
					Texture2D texture = DownloadHandlerTexture.GetContent(unityWebRequest);

					float cropWidth;
					float cropHeight;

					float imageRatio = (float)texture.width / (float)texture.height; // 이미지 비율
													 // center crop and resize
					if (imageRatio > 1.0)
					{ // 가로가 긴 경우
						cropHeight = imageHeight;
						cropWidth = cropHeight * imageRatio;
					} else
					{    // 세로가 긴 경우
						cropWidth = imageWidth;
						cropHeight = cropWidth / imageRatio;
					}

					if (cropWidth < imageWidth)
					{
						float widthRatio = (float)imageWidth / (float)cropWidth;
						cropWidth = imageWidth;
						cropHeight = cropHeight * widthRatio;
					}

					if (cropHeight < imageHeight)
					{
						float heightRatio = (float)imageHeight / (float)cropHeight;
						cropHeight = imageHeight;
						cropWidth = cropWidth * heightRatio;
					}

					// 이미지 생성
					Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

					// 이미지 영역에 추가
					GameObject imageObject = new GameObject("Image");
					imageObject.transform.SetParent(rectTransform.transform, false);
					Image image = imageObject.AddComponent<Image>();
					image.sprite = sprite;
					image.rectTransform.sizeDelta = new Vector2(cropWidth, cropHeight);
				} else
				{
					Debug.LogError("Failed to load image from server: " + unityWebRequest.error);
				}
			}
		}
	}

	IEnumerator loadWebPage(GameObject gameObject, string url)
	{
		UnityWebRequest request = UnityWebRequest.Get(url);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
		{
			Debug.LogError("Failed to load web page: " + request.error);
		} else
		{
			string html = request.downloadHandler.text;
			gameObject.GetComponentInChildren<Text>().text = html;
		}
	}

	/**
	 * 운영 모드에 따른 효과음 출력
	 * @param effect
	 * @return IEnumerator
	 */
	IEnumerator playEffectAudio(AudioSource effect)
	{
		if (UtilsScript.checkConfig() != null && UtilsScript.checkConfig() != "")
		{
			effect.Play();
		}

		yield return null;
	}
}
