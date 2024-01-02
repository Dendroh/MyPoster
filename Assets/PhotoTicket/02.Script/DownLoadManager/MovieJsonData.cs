using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;
using System;

public class MovieDOB
{
	public string ID;
	public string MovieTitle;
	public string MoviePoster;
	public string MoviePosterSize;
	public bool MovieIsFree;
	public string[] FaceCenters;
	public string[] FaceCentersSize;
	public string[] HandCenters;
	public string[] HandCentersSize;
	public string[] Foregrounds;
	public string[] ForegroundsSize;
	// 크로마키 정보 23-09-22 by flash
	public bool isChromakey = false;    // 관리자 웹에서 ON 설정을 안한 경우 컨텐츠가 있어도 크로마키 적용X, 기본값 false
	public string chromakeyType;
	public string chromakeySize;
	public string chromakeyBackground;
}

public class MovieJsonData : MonoBehaviour
{
	MovieDownManager movieDownManager;
	DownloadImageProcess downloadImageProcess;

	string rawJson;
	public List<MovieDOB> movieInfo;
	[HideInInspector] public string Version;
	[HideInInspector] public string[] IdlePoster;
	[HideInInspector] public string[] IdlePosterSize;

	void Awake()
	{
		movieDownManager = GetComponent<MovieDownManager>();
		movieInfo = new List<MovieDOB>();

		downloadImageProcess = GetComponent<DownloadImageProcess>();
	}

	public void Read(string jsonName)
	{
		var path = Path.Combine(Application.persistentDataPath, jsonName);
		if (File.Exists(path))
		{
			rawJson = File.ReadAllText(path);
		}
		Parse();
	}

	void CreateArray(JToken token, ref string[] temp)
	{
		JArray jArr = JArray.Parse(token.ToString());
		temp = new string[jArr.Count];
		for (int i = 0; i < temp.Length; i++)
		{
			temp[i] = jArr[i].ToString();
		}
		return;
	}

	void Parse()
	{
		try
		{
			JObject jobj = JObject.Parse(rawJson);

			Version = jobj["version"].ToString();
			CreateArray(jobj["idleposter"], ref IdlePoster);
			CreateArray(jobj["idleposter_size"], ref IdlePosterSize);

			JArray movies = JArray.Parse(jobj["movieindex"].ToString());

			for (int i = 0; i < movies.Count; i++)
			{
				MovieDOB temp = new MovieDOB();

				temp.ID = movies[i]["id"].ToString();
				temp.MovieTitle = movies[i]["movietitle"].ToString();
				temp.MoviePoster = movies[i]["movieposter"].ToString();
				temp.MovieIsFree = bool.Parse(movies[i]["movieisfree"].ToString());

				// 크로마키 정보 추가
				if (movies[i]["chromakey_background"] != null && !string.IsNullOrEmpty(movies[i]["chromakey_background"].ToString()))
				{
					temp.isChromakey = bool.Parse(movies[i]["chromakey"].ToString());
					temp.chromakeyType = movies[i]["chromakey_type"].ToString();
					temp.chromakeySize = movies[i]["chromakeye_size"].ToString();
					temp.chromakeyBackground = movies[i]["chromakey_background"].ToString();
				}

				CreateArray(movies[i]["facecenters"], ref temp.FaceCenters);
				CreateArray(movies[i]["handcenters"], ref temp.HandCenters);
				CreateArray(movies[i]["foregrounds"], ref temp.Foregrounds);

				temp.MoviePosterSize = movies[i]["movieposter_size"].ToString();
				CreateArray(movies[i]["facecenters_size"], ref temp.FaceCentersSize);
				CreateArray(movies[i]["handcenters_size"], ref temp.HandCentersSize);
				CreateArray(movies[i]["foregrounds_size"], ref temp.ForegroundsSize);

				movieInfo.Add(temp);
			}
		} catch (Exception e)
		{
			FlowController.instance.ChangeFlow(FlowController.instance.networkCheckCanvas);
		}
	}

	public void initEverything()
	{
		for (int i = 0; i < IdlePoster.Length; i++)
		{
			movieDownManager.InitPoster(IdlePoster[i]);
		}
		foreach (var movie in movieInfo)
		{
			movieDownManager.InitPoster(movie.MoviePoster);
			foreach (var face in movie.FaceCenters)
				movieDownManager.InitSticker(face);

			foreach (var hand in movie.HandCenters)
				movieDownManager.InitSticker(hand);

			foreach (var fore in movie.Foregrounds)
				movieDownManager.InitSticker(fore);
		}
	}
}
