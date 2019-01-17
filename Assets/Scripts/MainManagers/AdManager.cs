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

    private bool rewardedVideoWasShown = false;

    public bool RewardedAdIsReady
    {
        get { return !rewardedVideoWasShown && Advertisement.IsReady(rewardedAdId); }
    }
    
    public void ShowSimpleAd(UnityAction finished, UnityAction skipped, UnityAction failed)
    {
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
