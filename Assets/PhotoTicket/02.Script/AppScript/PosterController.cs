using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PosterController : MonoBehaviour
{
    public int id;
    SelectUIScript script;

    public void init()
    {
        script = GameObject.Find("1.Select Canvas").GetComponent<SelectUIScript>();
        GetComponent<Button>().onClick.AddListener(delegate { script.StartPhoto(id); });
    }
}
