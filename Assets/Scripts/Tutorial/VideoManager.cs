using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoManager : MonoSingleton<VideoManager>
{
    private VideoPlayer videoPlayer;
        
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        VideoUI.OnOpenedUiWithClip.AddListener(StartClip);
        VideoUI.OnClosedUiWithVideo.AddListener(StopClip);
    }

    private void StartClip(VideoClip videoClip)
    {
        videoPlayer.clip = videoClip;
        videoPlayer.Play();
    }
    
    private void StopClip()
    {
        videoPlayer.Stop();
    }
}
