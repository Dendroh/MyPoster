using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlay : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayVideo());
    }

    IEnumerator PlayVideo()
    {
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(1);
            break;
        }

        rawImage.texture = videoPlayer.texture;
        videoPlayer.Play();
    }
}
