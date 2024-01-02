using UnityEngine;
using System;
using System.Collections.Generic;
[Serializable]
public class MovieInfo
{
    public String MovieTitle;
    public Sprite MoviePoster;
    public bool Free;
    public MovieSticker MovieSticker;
}
[Serializable]
public class MovieSticker
{
    public GameObject[] faceCenters;
    public GameObject[] handCenters;
    public GameObject[] foregrounds;
}