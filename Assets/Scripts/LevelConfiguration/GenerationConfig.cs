using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GenerationConfig : ScriptableObject
{   
    [Header("Pops")]
    public float startMinPopAvg = 2f;
    public float endMinPopAvg = .5f;
    [Space(5)]
    public float startMaxPopAvg = 5f;
    public float endMaxPopAvg = 1.5f;
    [Space(5)]
    public float startPopLim = 50;
    public float endPopLim = 10;
    [Space(5)]
    public float popStepShift = 4;
    
    [Header("Rotations")]
    public float startMinRotAvg = 8f;
    public float endMinRotAvg = 2f;
    [Space(5)]
    public float startMaxRotAvg = 12f;
    public float endMaxRotAvg = 6f;
    [Space(5)]
    public float startRotLim = 50;
    public float endRotLim = 20;
    [Space(5)]
    public float rotStepShift = 4;
    
    [Space(10)]
    public float expDecrCoef = .001f;
    
    // Don't touch!
    public float multiplierChance = 0.03f;
}
