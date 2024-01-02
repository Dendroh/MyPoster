using UnityEngine;
using System.Collections;

public class ChromakeyMovie : MonoBehaviour
{
    public MovieTexture movie;
    void Start()
    {
        GetComponent<Renderer>().material.mainTexture = movie as MovieTexture;
        movie.Play();
        movie.loop = true;

    }
    void Update()
    {

    }
}
