using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisappearableAnimatableIntegerText : MonoBehaviour, IAnimatableIntegerText
{
    [SerializeField] private int defaultValue = 0;
    [SerializeField] private string prefix = "";
    [SerializeField] private bool formatThousands = false;

    private Text text;
    private Animator animator;
    
    private bool highlighted = true;
    private bool disappearing = false;
    
    private float disappearAnimationLength = 0f;
    private float DisappearAnimationLength
    {
        get
        {
            if (disappearAnimationLength == 0f)
            {
                disappearAnimationLength = animator.AnimationDuration("Disappear");
            }

            return disappearAnimationLength;
        }
    }

    private int previousValue;

    private void Awake()
    {
        text = GetComponentInChildren<Text>();
        animator = GetComponentInChildren<Animator>();
        previousValue = int.MinValue;
    }

    public void SetValue(int value)
    {
        if (value == previousValue)
            return;
        previousValue = value;
        
        if (value == defaultValue)
        {
            if (highlighted)
                Disappear();
            highlighted = false;
        }
        else
        {
            this.InvokeAfter(() =>
            {
                if (!highlighted)
                    Appear();
                text.text = prefix + (formatThousands ? value.FormatThousands() : value.ToString());
                highlighted = true;
            }, () => !disappearing);
        }
    }

    private void Appear()
    {
        animator.SetTrigger("Appear");
    }
    
    private void Disappear()
    {
        animator.SetTrigger("Disappear");
        disappearing = true;
        this.InvokeWithDelay(() => disappearing = false, DisappearAnimationLength);
    } 
}
