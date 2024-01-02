using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerController : MonoBehaviour
{
    public int quizIndex;
    public int answerIndex;
    public string isAnswer;
    public string type;
    public string answerCount;
    public string ox;
    QuizUIScript script;

    public void init() {
        script = GameObject.Find("8.Quiz Canvas").GetComponent<QuizUIScript>();
        GetComponent<Button>().onClick.AddListener(delegate { script.clickAnswer(quizIndex, isAnswer, answerIndex, type, answerCount, ox); });
    }
}