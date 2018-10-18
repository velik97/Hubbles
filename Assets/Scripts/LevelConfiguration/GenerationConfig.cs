using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GenerationConfig : ScriptableObject
{   
    [Header("Pops")]
    public float startMinPopAvg = .5f;
    public float endMinPopAvg = 2f;
    [Space(5)]
    public float startMaxPopAvg = 5f;
    public float endMaxPopAvg = 1.5f;
    [Space(5)]
    public float startPopLim = 20;
    public float endPopLim = 10;
    
    [Header("Rotations")]
    public float startMinRotAvg = 6f;
    public float endMinRotAvg = 2f;
    [Space(5)]
    public float startMaxRotAvg = 12f;
    public float endMaxRotAvg = 6f;
    [Space(5)]
    public float startRotLim = 30;
    public float endRotLim = 20;
    
    [Space(10)]
    public float expDecrCoef = .1f;
    
    public float multiplierChance = 0.03f;
    
    private float avgToProbCoef = 0f;

    public float AvgToProbCoef
    {
        get
        {
            if (avgToProbCoef == 0f)
                avgToProbCoef = 47.6f * Mathf.Exp(43f * multiplierChance);
            return avgToProbCoef;
        }
        private set { avgToProbCoef = value; }
    }

    public void SetRemoteLevelConfig(bool wasUpdatedFromServer, bool settingsChanged, int serverResponse)
    {
        if (RemoteSettings.GetCount() == 0)
            return;
        
        startMinPopAvg = RemoteSettings.GetFloat("StartMinPopAvg", startMinPopAvg);
        endMinPopAvg = RemoteSettings.GetFloat("EndMinPopAvg", endMinPopAvg);
        
        startMaxPopAvg = RemoteSettings.GetFloat("StartMaxPopAvg", startMaxPopAvg);
        endMaxPopAvg = RemoteSettings.GetFloat("EndMaxPopAvg", endMaxPopAvg);
        
        startPopLim = RemoteSettings.GetFloat("StartPopLim", startPopLim);
        endPopLim = RemoteSettings.GetFloat("EndPopLim", endPopLim);
        
        startMinRotAvg = RemoteSettings.GetFloat("StartMinRotAvg", startMinRotAvg);
        endMinRotAvg = RemoteSettings.GetFloat("EndMinRotAvg", endMinRotAvg);
        
        startMaxRotAvg = RemoteSettings.GetFloat("StartMaxRotAvg", startMaxRotAvg);
        endMaxRotAvg = RemoteSettings.GetFloat("EndMaxRotAvg", endMaxRotAvg);
        
        startRotLim = RemoteSettings.GetFloat("StartRotLim", startRotLim);
        endRotLim = RemoteSettings.GetFloat("EndRotLim", endRotLim);
        
        expDecrCoef = RemoteSettings.GetFloat("ExpDecrCoef", expDecrCoef);
        
        multiplierChance = RemoteSettings.GetFloat("MultiplierChance", multiplierChance);
        
        AvgToProbCoef = 47.6f * Mathf.Exp(43f * multiplierChance);
    }
}
