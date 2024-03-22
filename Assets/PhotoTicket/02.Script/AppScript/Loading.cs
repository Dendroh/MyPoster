using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Loading : MonoBehaviour
{
	[SerializeField] Sprite[] loadingSprite;
	Image loadingImage;
	float count;
	int length;
	private Coroutine loadingCoroutine;

	void Start()
	{
		loadingImage = GetComponent<Image>();
		length = loadingSprite.Length;
	}
	void OnEnable()
	{
		count = 0;
		loadingCoroutine = StartCoroutine(setLoading());
	}

	// Update is called once per frame
	void Update()
	{
		//count += Time.deltaTime * 10;
		//loadingImage.sprite = loadingSprite[(int)count%length];
	}

	IEnumerator setLoading()
	{
		while (true)
		{
			count += Time.deltaTime * 10;
			if (length > 0)
			{
				loadingImage.sprite = loadingSprite[(int)count % length];
			}

			yield return null;
		}
	}

	void Dispose()
	{
		if (loadingCoroutine != null)
		{
			StopCoroutine(setLoading());
		}
	}
}
