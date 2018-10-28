using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    [SerializeField] private MenuPanel[] hintMenuPanels;
    [SerializeField] private int scoreForPopTutorial;
    [SerializeField] private GameObject rotLivesUI;

    private MenuPanel currentMenuPanel;

    public void StartGame()
    {
       GameManager.Instance.sceneLoader.onLoaded.AddListener(OpenPopHint); 
    }
    
    private void OpenPopHint()
    {
        if (currentMenuPanel != null)
            currentMenuPanel.RemoveAllListeners();
        currentMenuPanel = hintMenuPanels[0];
        currentMenuPanel.onClosed.AddListener(StartTeachingPop);
        MenuManager.Instance.OpenMenuPanel(currentMenuPanel);
    }

    private void StartTeachingPop()
    {
        HubblesManager.Instance.onPoped.AddListener(CheckPopTutorial);
    }

    private void CheckPopTutorial()
    {
        if (HubblesManager.Instance.totalScore >= scoreForPopTutorial)
        {
            OpenRotHint();
            HubblesManager.Instance.onPoped.RemoveListener(CheckPopTutorial);
        }
    }
    
    private void OpenRotHint()
    {
        if (currentMenuPanel != null)
            currentMenuPanel.RemoveAllListeners();
        currentMenuPanel = hintMenuPanels[1];
        currentMenuPanel.onClosed.AddListener(StartTeachingRot);
        MenuManager.Instance.OpenMenuPanel(currentMenuPanel);
    }
    
    private void StartTeachingRot()
    {
        foreach (var text in rotLivesUI.GetComponentsInChildren<Text>())
        {
            var textColor = text.color;
            textColor.a = 1f;
            text.color = textColor;
        }
        
        foreach (var image in rotLivesUI.GetComponentsInChildren<Image>())
        {
            var imageColor = image.color;
            imageColor.a = 1f;
            image.color = imageColor;
        }

        HubblesManager.Instance.onPoped.AddListener(CheckRotTutorial);
    }

    private void CheckRotTutorial()
    {
        if (HubblesManager.Instance.allAreOneColor)
        {
            EndTutorial();
            HubblesManager.Instance.onPoped.RemoveListener(CheckRotTutorial);
        }
    }

    private void EndTutorial()
    {
        if (currentMenuPanel != null)
            currentMenuPanel.RemoveAllListeners();
        currentMenuPanel = hintMenuPanels[2];
        MenuManager.Instance.OpenMenuPanel(currentMenuPanel);
    }
}

