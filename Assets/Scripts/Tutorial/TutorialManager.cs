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
       GameManager.Instance.sceneLoader.onLoaded.AddListener(TeachPop); 
    }

    private void TeachPop()
    {
        TouchManager.Instance.SetOtherInput(TutorialFinger.Instance.tutorialFingerTouchSource);
        for (int i = 0; i < 3; i++)
        {
            Coord c = Coord.Random();
            TutorialFinger.Instance.ClickOnCoord(c);
            TutorialFinger.Instance.ClickOnCoord(c);
        }
        TutorialFinger.Instance.done.AddListener(TeachRotate);
    }

    private void TeachRotate()
    {
        BoolMap boolMap = new BoolMap(Map.nodeMap, 0);
        ParallelTaskManager.Instance.CallFuncParallel<IEnumerable<RotationStep>>(
            () => RotatingProblemSolver.FindRotationSequence(boolMap),
            ExecuteRotation);
    }
    
    private void ExecuteRotation(IEnumerable<RotationStep> steps)
    {
        foreach (var s in steps)
        {
            TutorialFinger.Instance.RotateOnCoord(s.coord, s.turns);
        }
    }
}

