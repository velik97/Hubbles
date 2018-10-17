using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GenerationConfig : ScriptableObject
{   
    [Header("Pops")]
    public float minPopAvg = .5f;
    [Space(5)]
    public float startMaxPopAvg = 5f;
    public float endMaxPopAvg = 1.5f;
    [Space(5)]
    public float startPopLim = 20;
    public float endPopLim = 10;
    
    [Header("Rotations")]
    public float minRotAvg = 2f;
    [Space(5)]
    public float startMaxRotAvg = 12f;
    public float endMaxRotAvg = 6f;
    [Space(5)]
    public float startRotLim = 30;
    public float endRotLim = 20;
    
    [Space(10)]
    public float expDecrCoef = .1f;
    
    // Don't touch!
    public float multiplierChance = 0.03f;
}
