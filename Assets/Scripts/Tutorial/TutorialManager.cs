using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class TutorialManager : MonoSingleton<TutorialManager>
{

    [SerializeField] private GameObject[] hints;
    [SerializeField] private GameObject praise;
    
    public void StartGame()
    {
        ActivateHint(-1);
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
        }
    }

    private void TeachRotate()
    {
        ActivateHint(3);
        CollectGroup();
    }

    private void CollectGroup()
    {
        TouchManager.Instance.SetOtherInput(TutorialFinger.Instance.tutorialFingerTouchSource);
        var biggestGroupNumber = MapGenerator.Instance.BiggestGroupNumber;
        BoolMap boolMap = new BoolMap(Map.nodeMap, biggestGroupNumber);
        ParallelTaskManager.Instance.CallFuncParallel<IEnumerable<RotationStep>>(
            () => RotatingProblemSolver.FindRotationSequence(boolMap),
            ExecuteRotation);
        TutorialFinger.Instance.onExecutedAllCommands.AddListener(PopCollectedGroup);
        TutorialFinger.Instance.onExecutedAllCommands.AddListener(() => TutorialFinger.Instance.onExecutedAllCommands
            .RemoveListener(PopCollectedGroup));
    }

    private void PopCollectedGroup()
    {
        var biggestGroupNumber = MapGenerator.Instance.BiggestGroupNumber;
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
        ActivateHint(1);
        TouchManager.Instance.SetUserInput();
        HubblesManager.Instance.onPoped.AddListener(CheckRotateGoal);
    }

    private void CheckRotateGoal()
    {
        if (HubblesManager.Instance.allAreOneColor)
        {
            LearnPop2();
            HubblesManager.Instance.onPoped.RemoveListener(CheckRotateGoal);
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
        ActivateHint(3);
        GameManager.Instance.Lose();
    }
    
    private void ExecuteRotation(IEnumerable<RotationStep> steps)
    {
        print("steps: " + steps.Count());
        foreach (var s in steps)
        {
            TutorialFinger.Instance.RotateOnCoord(s.coord, s.turns);
        }
    }

    private void ActivateHint(int hintNumber)
    {
        for (var i = 0; i < hints.Length; i++)
        {
            hints[i].SetActive(hintNumber == i);
        }
        praise.SetActive(hintNumber == 3);
    }
}

