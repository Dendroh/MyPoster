using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HiddenButton : MonoBehaviour
{
	float count = 0;
	// Start is called before the first frame update
	void Start()
	{
		DontDestroyOnLoad(this);
		GetComponentInChildren<Button>().onClick.AddListener(() =>
		{
			count++;
		});
	}

	// Update is called once per frame
	void Update()
	{
		if (count > 0)
		{
			count -= Time.deltaTime * 2;
			if (count > 3) Application.Quit();
		}
	}
}
