using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryController : MonoBehaviour
{
    public int id;
    QuizUIScript script;

    public void init() {
        script = GameObject.Find("8.Quiz Canvas").GetComponent<QuizUIScript>();
        GetComponent<Button>().onClick.AddListener(delegate { script.clickCategory(id); });
    }
}