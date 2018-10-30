using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class VideoUI : MonoBehaviour
{
    private static VideoClipEvent onOpenedUIWithClip;
    private static UnityEvent onClosedUIWithVideo;

    [SerializeField] private VideoClip videoClip;

    public static VideoClipEvent OnOpenedUiWithClip
    {
        get { return onOpenedUIWithClip ?? (onOpenedUIWithClip = new VideoClipEvent()); }
    }
    public static UnityEvent OnClosedUiWithVideo
    {
        get { return onClosedUIWithVideo ?? (onClosedUIWithVideo = new UnityEvent()); }
    }

    private void OnEnable()
    {
        OnOpenedUiWithClip.Invoke(videoClip);
    }

    private void OnDisable()
    {
        OnClosedUiWithVideo.Invoke();
    }
}

public class VideoClipEvent : UnityEvent<VideoClip> {}
