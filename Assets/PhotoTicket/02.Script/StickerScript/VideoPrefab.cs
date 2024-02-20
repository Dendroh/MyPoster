using UnityEngine;
using UnityEngine.Video;
public class VideoPrefab : MonoBehaviour
{
    private void OnDestroy()
    {
        GetComponent<VideoPlayer>().Stop();
    }
}
