using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HotFixTMP : MonoBehaviour
{
    public float delay = 0.01f;

    private TextMeshPro TMP;

    // Start is called before the first frame update
    void Start()
    {
        TMP = this.gameObject.GetComponent<TextMeshPro>();
        StartCoroutine("HotFixRoutine");
    }

    IEnumerator HotFixRoutine()
    {
        Color32 color = TMP.color;

        bool flag = true;
        while(true)
        {
            if (flag)
                color.a = --color.a;
            else
                color.a = ++color.a;

            TMP.color = color;
            flag = !flag;
            yield return new WaitForSeconds(delay);
        }
    }
}
