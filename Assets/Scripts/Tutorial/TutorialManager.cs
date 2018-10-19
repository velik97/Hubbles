using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    [SerializeField] private PlayerAllowedAction teachingPopAllowedAction;
    [SerializeField] private PlayerAllowedAction teachingRotateAllowedAction;
    [SerializeField] private PlayerAllowedAction teachingPopingAllAllowedAction;

    [Space(10)]
    [SerializeField] private Animator pointerHandAnimator;
    [SerializeField] private Animator popHolderAnimator;
    [SerializeField] private Animator rotHolderAnimator;
    
    [Space(10)]
    [SerializeField] private Text hintText;
    [SerializeField] private List<string> hints;
    
    [Space(10)]
    [SerializeField] private float rotationTime = .1f;
    
    private PlayerAllowedAction currentPlayerAllowedAction;
    private Animator hintTextAnimator;

    private float currentAngleWhileAdjustingRotation = 0f;

    public bool CoordIsAllowed(Coord coord)
    {
        if (currentPlayerAllowedAction == null)
            return true;
        return currentPlayerAllowedAction.allowedCoords.Contains(coord);
    }
    
    public bool ActionTypeIsAllowed(PlayerActionType actionType)
    {
        if (currentPlayerAllowedAction == null)
            return true;
        return currentPlayerAllowedAction.allowedAction == actionType;
    }

    public void StartGame()
    {
        hintTextAnimator = hintText.GetComponent<Animator>();
        GoToNextState(TutorialState.TeachingHighLight);
    }
    
    private void GoToNextState(TutorialState tutorialState)
    {
        StartCoroutine(GoToNextStateCorutine(tutorialState));
    }

    private IEnumerator GoToNextStateCorutine(TutorialState tutorialState)
    {
        yield return new WaitForSeconds(2f);
        switch (tutorialState)
        {
            case TutorialState.TeachingHighLight:
                ProceedTeachingHighLight();
                break;
            case TutorialState.TeachingPop:
                ProceedTeachingPop();
                break;
            case TutorialState.TeachingRotate:
                ProceedTeachingRotate();
                break;
            case TutorialState.TeachingPopingAll:
                ProceedTeachingPopingAll();
                break;
            case TutorialState.CompletingTutorial:
                ProceedCompletingTutorial();
                break;
        }
    }
    
    private void ProceedTeachingHighLight()
    {
        hintText.text = hints[0];
        hintTextAnimator.SetTrigger("Appear");
        pointerHandAnimator.SetTrigger("Shake");
        popHolderAnimator.SetTrigger("Appear");
        
        currentPlayerAllowedAction = teachingPopAllowedAction;
        
        HubblesManager.Instance.onHighlighted.AddListener(delegate
        {
            hintTextAnimator.SetTrigger("Disappear");
            pointerHandAnimator.SetTrigger("Disappear");
            GoToNextState(TutorialState.TeachingPop);
            HubblesManager.Instance.onHighlighted.RemoveAllListeners();
        });
    }
    
    private void ProceedTeachingPop()
    {
        hintText.text = hints[1];
        hintTextAnimator.SetTrigger("Appear");
        pointerHandAnimator.SetTrigger("Shake");
        
        currentPlayerAllowedAction = teachingPopAllowedAction;
        
        HubblesManager.Instance.onPoped.AddListener(delegate
        {
            hintTextAnimator.SetTrigger("Disappear");
            pointerHandAnimator.SetTrigger("Disappear");
            popHolderAnimator.SetTrigger("Hide");
            GoToNextState(TutorialState.TeachingRotate);
            HubblesManager.Instance.onPoped.RemoveAllListeners();
        });
    }
    
    private void ProceedTeachingRotate()
    {
        hintText.text = hints[2];
        hintTextAnimator.SetTrigger("Appear");        
        pointerHandAnimator.SetTrigger("Rotate");
        rotHolderAnimator.SetTrigger("Appear");
        
        currentPlayerAllowedAction = teachingRotateAllowedAction;
        
        HubblesManager.Instance.onRotated.AddListener(delegate (int turns)
        {
            hintTextAnimator.SetTrigger("Disappear");
            pointerHandAnimator.SetTrigger("Disappear");
            rotHolderAnimator.SetTrigger("Hide");
            
            if (turns == 3)
            {
                GoToNextState(TutorialState.TeachingPopingAll);
                HubblesManager.Instance.onRotated.RemoveAllListeners();
            }
            else
            {
                StartCoroutine(ProceedRotationIsNotDoneCompletelyCoroutine(turns));
                HubblesManager.Instance.onRotated.RemoveAllListeners();
            }
        });
    }

    private IEnumerator ProceedRotationIsNotDoneCompletelyCoroutine(int doneTurns)
    {
        currentPlayerAllowedAction = new PlayerAllowedAction()
        {
            allowedCoords = new List<Coord>(),
            allowedAction = PlayerActionType.Tap
        };
        yield return new WaitForSeconds(AnimationManager.Instance.pullTime*2f);
        HubblesManager.Instance.StartRotating(GetCurrentAngleWhileAdjustingRotation);
        float t = 0f;
        float startTime = Time.time;
        int turns = 3 - doneTurns;
        float angle = turns * 60f;
        
        while ((t = (Time.time - startTime)/(rotationTime * Mathf.Abs(turns))) < 1f)
        {
            currentAngleWhileAdjustingRotation = Mathf.Lerp(0f, angle, t);
            yield return null;
        }
        HubblesManager.Instance.EndRotating(GetCurrentAngleWhileAdjustingRotation);
        
        GoToNextState(TutorialState.TeachingPopingAll);
    }

    public float GetCurrentAngleWhileAdjustingRotation()
    {
        return currentAngleWhileAdjustingRotation;
    }
    
    private void ProceedTeachingPopingAll()
    {
        hintText.text = hints[3];
        hintTextAnimator.SetTrigger("Appear");        
        pointerHandAnimator.SetTrigger("Shake");
        
        currentPlayerAllowedAction = teachingPopingAllAllowedAction;
        
        HubblesManager.Instance.onPoped.AddListener(delegate
        {
            hintTextAnimator.SetTrigger("Disappear");        
            pointerHandAnimator.SetTrigger("Disappear");
            GoToNextState(TutorialState.CompletingTutorial);
            HubblesManager.Instance.onPoped.RemoveAllListeners();
        });
    }
    
    private void ProceedCompletingTutorial()
    {
        hintText.text = hints[4];
        hintTextAnimator.SetTrigger("Appear"); 
        
        pointerHandAnimator.SetTrigger("Disappear");
        currentPlayerAllowedAction = null;
        InvokeDelayed(() => hintTextAnimator.SetTrigger("Disappear"), 5f);
    }

    private void InvokeDelayed(Action action, float delay)
    {
        StartCoroutine(InvokeDelayedCoroutine(action, delay));
    }

    private IEnumerator InvokeDelayedCoroutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }
}

[System.Serializable]
public class PlayerAllowedAction
{
    public List<Coord> allowedCoords;
    public PlayerActionType allowedAction;
}

public enum TutorialState
{
    TeachingHighLight = 0,
    TeachingPop = 1,
    TeachingRotate = 2,
    TeachingPopingAll = 3,
    CompletingTutorial = 4
}

