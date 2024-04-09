using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizController : MonoBehaviour
{
    public int categoryId;
    public int quizIndex;
    public List<int> answerNumber;
    QuizUIScript script;

    public void init() {
        script = GameObject.Find("8.Quiz Canvas").GetComponent<QuizUIScript>();
        GetComponent<Button>().onClick.AddListener(delegate { script.clickNext(quizIndex); });
    }

    public void exit() {
        script = GameObject.Find("8.Quiz Canvas").GetComponent<QuizUIScript>();
        GetComponent<Button>().onClick.AddListener(delegate { script.exit(); });
    }
}