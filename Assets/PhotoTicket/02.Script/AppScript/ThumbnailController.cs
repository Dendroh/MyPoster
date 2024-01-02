using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ThumbnailController : MonoBehaviour
{
    public int id;
    PhotoUIScript script;
    public void init()
    {
        script = GameObject.Find("2.Photo Canvas").GetComponent<PhotoUIScript>();
        GetComponent<Button>().onClick.AddListener(delegate { script.selectPoster(id); });
    }
}
