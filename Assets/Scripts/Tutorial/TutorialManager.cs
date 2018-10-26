using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    [SerializeField] private Animator pointerHandAnimator;
    
    [Space(10)]
    [SerializeField] private float waitBeforeHint = 5f;
    
    private bool hintIsShown = false;
    private bool playerIsGoingRight = false;

    private float waitedBeforeHint = 0f; 

    public void StartGame()
    {
        HubblesManager.Instance.onHighlighted.AddListener(PlayerInteracted);
        HubblesManager.Instance.onPoped.AddListener(PlayerInteracted);
        waitedBeforeHint = waitBeforeHint;
    }

    private void PlayerInteracted()
    {
        waitedBeforeHint = 0f;
        if (hintIsShown)
            HideHint();
    }

    private void Update()
    {
        waitedBeforeHint += Time.deltaTime;
        playerIsGoingRight = waitedBeforeHint < waitBeforeHint;
                
        if (!hintIsShown && !playerIsGoingRight)
            ShowHint();
    }

    private void ShowHint()
    {
        pointerHandAnimator.SetTrigger("Shake");
        hintIsShown = true;
    }

    private void HideHint()
    {
        pointerHandAnimator.SetTrigger("Disappear");
        hintIsShown = false;
    }
}

