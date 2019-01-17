using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class TutorialManager : MonoSingleton<TutorialManager>
{

    [SerializeField] private GameObject[] hints;
    [SerializeField] private GameObject[] colorHints;
    [SerializeField] private GameObject praise;

    private int biggestGroupNumber;
    
    public void StartGame()
    {
        ActivateHint(-1);
        GameManager.Instance.sceneLoader.onLoaded.AddListener(TeachPop);
        AnalyticsEvent.TutorialStart();
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
        TutorialFinger.Instance.GetAway();
        TutorialFinger.Instance.onExecutedAllCommands.AddListener(LearnPop);
        TutorialFinger.Instance.onExecutedAllCommands.AddListener(() => TutorialFinger.Instance.onExecutedAllCommands
            .RemoveListener(LearnPop));
    }

    private void LearnPop()
    {
        ActivateHint(0);
        TouchManager.Instance.SetUserInput();
        MapGenerator.Instance.onReestablished.AddListener(CheckPopGoal);
    }

    private void CheckPopGoal()
    {
        if (HubblesManager.Instance.totalScore >= 50)
        {
            TeachRotate();
            MapGenerator.Instance.onReestablished.RemoveListener(CheckPopGoal);
            AnalyticsEvent.TutorialStep(0);
        }
    }

    private void TeachRotate()
    {
        ActivateHint(3);
        biggestGroupNumber = MapGenerator.Instance.BiggestGroupNumber;
        CollectGroup();
    }

    private void CollectGroup()
    {
        TouchManager.Instance.SetOtherInput(TutorialFinger.Instance.tutorialFingerTouchSource);
        BoolMap boolMap = new BoolMap(Map.nodeMap, biggestGroupNumber);
        ParallelTaskManager.Instance.CallFuncParallel<IEnumerable<RotationStep>>(
            () => RotatingProblemSolver.FindRotationSequence(boolMap),
            ExecuteRotation);   
    }

    private void ExecuteRotation(IEnumerable<RotationStep> steps)
    {
        if (!steps.Any())
        {
            biggestGroupNumber = (biggestGroupNumber + 1) % 4;
            CollectGroup();
            return;
        }
        foreach (var s in steps)
        {
            TutorialFinger.Instance.RotateOnCoord(s.coord, s.turns);
        }
        TutorialFinger.Instance.onExecutedAllCommands.AddListener(PopCollectedGroup);
        TutorialFinger.Instance.onExecutedAllCommands.AddListener(() => TutorialFinger.Instance.onExecutedAllCommands
            .RemoveListener(PopCollectedGroup));
    }
    
    private void PopCollectedGroup()
    {
        var c = Map.CoordFromNode(MapGenerator.Instance.thisColorNodes[biggestGroupNumber][0]);
        TutorialFinger.Instance.ClickOnCoord(c);
        TutorialFinger.Instance.WaitForSeconds(1f);
        TutorialFinger.Instance.ClickOnCoord(c);
        TutorialFinger.Instance.GetAway();
        TutorialFinger.Instance.onExecutedAllCommands.AddListener(LearnRotate);
        TutorialFinger.Instance.onExecutedAllCommands.AddListener(() => TutorialFinger.Instance.onExecutedAllCommands
            .RemoveListener(LearnRotate));
    }

    private void LearnRotate()
    {
        biggestGroupNumber = MapGenerator.Instance.BiggestGroupNumber;
        while (true)
        {
            BoolMap boolMap = new BoolMap(Map.nodeMap, biggestGroupNumber);  
            if (boolMap.GroupsCount() > 1)
                break;
            biggestGroupNumber = (biggestGroupNumber + 1) % 4;
        }
        ActivateHint(1);
        ActivateColorGroupHint(biggestGroupNumber);
        TouchManager.Instance.SetUserInput();
        HubblesManager.Instance.onPoped.AddListener(CheckRotateGoal);
    }

    private void CheckRotateGoal()
    {
        if (HubblesManager.Instance.allAreOneColor)
        {
            if (HubblesManager.Instance.CurrentNodeColor != biggestGroupNumber)
                return;
            LearnPop2();
            HubblesManager.Instance.onPoped.RemoveListener(CheckRotateGoal);
            AnalyticsEvent.TutorialStep(1);
        }
    }

    private void LearnPop2()
    {
        ActivateHint(3);
        this.InvokeWithDelay(() => ActivateHint(2), 2f);
        TouchManager.Instance.SetUserInput();
        MapGenerator.Instance.onReestablished.AddListener(CheckPopGoal2);
    }

    private void CheckPopGoal2()
    {
        if (HubblesManager.Instance.totalScore >= 500)
        {
            TutorialIsFinished();
            MapGenerator.Instance.onReestablished.RemoveListener(CheckPopGoal2);
        }
    }

    private void TutorialIsFinished()
    {
        AnalyticsEvent.TutorialComplete();
        ActivateHint(3);
        GameManager.Instance.Lose();
    }

    private void ActivateHint(int hintNumber)
    {
        for (var i = 0; i < hints.Length; i++)
        {
            hints[i].SetActive(hintNumber == i);
        }
        praise.SetActive(hintNumber == 3);
    }
    
    private void ActivateColorGroupHint(int hintNumber)
    {
        for (var i = 0; i < colorHints.Length; i++)
        {
            colorHints[i].SetActive(hintNumber == i);
        }
    }
}

