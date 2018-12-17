using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class AdManager : MonoSingleton<AdManager>
{
    private const string simpleAdId = "video";
    private const string rewardedAdId = "rewardedVideo";

    private const float secondsBetweenAds = 60f;
    private float lastTimeAdShown = 0f;
    private bool rewardedVideoWasShown = false;

    private bool EnoughTimeHasPassed
    {
        get { return Time.time - lastTimeAdShown > secondsBetweenAds; }
    }

    public bool RewardedAdIsReady
    {
        get { return !rewardedVideoWasShown && Advertisement.IsReady(rewardedAdId); }
    }
    
    public bool SimpleAdIsReady
    {
        get { return Advertisement.IsReady(simpleAdId); }
    }

    public bool RewardedVideoWasShown
    {
        get { return rewardedVideoWasShown; }
    }
    
    public void ShowSimpleAd(UnityAction finished, UnityAction skipped, UnityAction failed)
    {
        if (!EnoughTimeHasPassed)
        {
            failed.Invoke();
            return;
        }
        ShowAd(simpleAdId, finished, skipped, finished);
    }

    public void ShowRewardedAd(UnityAction finished, UnityAction skipped, UnityAction failed)
    {
        rewardedVideoWasShown = true;
        ShowAd(rewardedAdId, finished, skipped, finished);
    }

    private void ShowAd(string placementId, UnityAction finished, UnityAction skipped, UnityAction failed)
    {
        if (Advertisement.IsReady(placementId))
        {
            var options = new ShowOptions
            {
                resultCallback = (ShowResult s) =>
                {
                    switch (s)
                    {
                        case ShowResult.Failed:
                            failed.Invoke();
                            break;
                        case ShowResult.Skipped:
                            skipped.Invoke();
                            break;
                        case ShowResult.Finished:
                            lastTimeAdShown = Time.time;
                            finished.Invoke();
                            break;
                    }
                }
            };
            Advertisement.Show(placementId, options);
        }
        else
        {
            failed.Invoke();
        }
    }
    
    
}
