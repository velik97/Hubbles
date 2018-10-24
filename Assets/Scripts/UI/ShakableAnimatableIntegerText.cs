using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ShakableAnimatableIntegerText : MonoBehaviour, IAnimatableIntegerText
{
    [SerializeField] private string prefix = "";
    [SerializeField] private bool formatThousands = false;

    private Text text;
    private Animator animator;
    private float shakeAnimationLength = 0f;
    private float ShakeAnimationLength
    {
        get
        {
            if (shakeAnimationLength == 0f)
            {
                shakeAnimationLength = animator.AnimationDuration("Appear")/2f;
            }

            return shakeAnimationLength;
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
        
        Shake();
        this.InvokeWithDelay(
            () => text.text = prefix + (formatThousands ? FormatNumber(value) : value.ToString()),
            shakeAnimationLength);
        

    }
    
    private void Shake()
    {
        animator.SetTrigger("Shake");
    }
    
    private static string FormatNumber(int number)
    {
        var f = new NumberFormatInfo {NumberGroupSeparator = " "};
        return number.ToString("#,0", f);
    } 
}
