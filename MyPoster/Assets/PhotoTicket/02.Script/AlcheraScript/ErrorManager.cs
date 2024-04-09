using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ErrorManager : MonoBehaviour
{
    public static ErrorManager instance;
    [SerializeField] Text errorText;
    [SerializeField] GameObject ErrorPanel;
    [SerializeField] Button confirmButton;
    [SerializeField] Text confirmText;
    bool needtoQuit;
    void Start()
    {
        DontDestroyOnLoad(this);
        ErrorPanel.SetActive(false);
        if (!instance)
            instance = this;

        confirmButton.onClick.AddListener(() =>
        {
            if (needtoQuit == true)//꺼야한다
            {
#if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
#else
                 Application.Quit();
#endif
            }
            else
            {
                ErrorPanel.gameObject.SetActive(false);
            }
        });
    }
    public void PopUpError(string s, bool needQuit)
    {
        errorText.text = s;
        ErrorPanel.gameObject.SetActive(true);
        needtoQuit = needQuit;
        confirmText.text = needtoQuit ? "종료" : "확인";
    }
}
