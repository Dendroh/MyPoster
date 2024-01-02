﻿using UnityEngine;
using UnityEditor;

public class ConstantsScript : ScriptableObject
{
	public static string OPERATE_URL = "http://myposter.kr";
    public static string LOCAL_URL = "http://192.168.1.22";

	// Lang Type
    public static string LANG_KR = "kr";
    public static string LANG_EN = "en";
    
	// Quiz Type
    public static string OX = "0";
    public static string MULTIPLE_CHOICE = "1";

    // OX Answer Type
    public static string ANSWER_IS_X = "0";
    public static string ANSWER_IS_O = "1";

    // Answer Status
    public static string WRONG_ANSWER = "0";
    public static string CORRECT_ANSWER = "1";

    // OX Type
    public static string O = "o";
    public static string X = "x";

    // Quiz Done Button Type
    public static string URL = "0";
    public static string SMS = "1";
}