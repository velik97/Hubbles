using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseMenu : AnimatedMenuPanel
{
    [SerializeField] private Text yourScoreText;
    [SerializeField] private Text bestScoreText;
    
    [Space(5)]
    [SerializeField] private GameObject restartButtonWithAd;
    [SerializeField] private GameObject adButton;
    [SerializeField] private GameObject restartButton;
    
    public override void OpenPanel()
    {
        yourScoreText.text = "YOUR SCORE: " + HubblesManager.Instance.totalScore.FormatThousands();
        bestScoreText.text = HubblesManager.Instance.totalScore == GameManager.Instance.Record
            ? "NEW RECORD!"
            : "BEST SCORE: " + GameManager.Instance.Record.FormatThousands();

        if (AdManager.Instance.RewardedAdIsReady)
        {
            restartButtonWithAd.SetActive(true);
            adButton.SetActive(true);
            restartButton.SetActive(false);
        }
        else
        {
            restartButtonWithAd.SetActive(false);
            adButton.SetActive(false);
            restartButton.SetActive(true);
        }
        base.OpenPanel();
    }
}
