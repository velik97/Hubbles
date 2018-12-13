using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ProceduralHubbleGenerator : MonoBehaviour, IHubbleGenerator
{
    private Random prng = null;

    private Random Prng
    {
        get
        {
            if (prng == null)
                prng = new System.Random(Mathf.RoundToInt(Time.realtimeSinceStartup * 1000f));
            return prng;
        }
    }

    [SerializeField]
    private ProceduralGenerationConfig proceduralGenerationConfig;

    private float popLivesChance;
    private float rotLivesChance;

    private void Awake()
    {
        RemoteSettings.Completed += proceduralGenerationConfig.SetRemoteLevelConfig;
    }

    public void LoadStepData(int score, int pops, int rots)
    {
        popLivesChance = GetProbabilityFromParamsNotShifted(score, pops, proceduralGenerationConfig.startMinPopAvg,
            proceduralGenerationConfig.endMinPopAvg,
            proceduralGenerationConfig.startMaxPopAvg, proceduralGenerationConfig.endMaxPopAvg, proceduralGenerationConfig.startPopLim,
            proceduralGenerationConfig.endPopLim, proceduralGenerationConfig.expDecrCoef);
        rotLivesChance = GetProbabilityFromParamsNotShifted(score, rots, proceduralGenerationConfig.startMinRotAvg,
            proceduralGenerationConfig.endMinRotAvg,
            proceduralGenerationConfig.startMaxRotAvg, proceduralGenerationConfig.endMaxRotAvg, proceduralGenerationConfig.startRotLim,
            proceduralGenerationConfig.endRotLim, proceduralGenerationConfig.expDecrCoef);
        
//        Debug.Log("Pop avg: " + (popLivesChance*proceduralGenerationConfig.AvgToProbCoef) + " prob: " + popLivesChance + "\n" +
//                  "Rot avg: " + (rotLivesChance*proceduralGenerationConfig.AvgToProbCoef) + " prob: " + rotLivesChance);
    }
    
    public int GetColor()
    {
        return Prng.Next(0, LevelConfig.Colors.Length);
    }
    
    public int GetColor(int prevColor)
    {
        int newColor = Prng.Next(0, LevelConfig.Colors.Length-1);
        if (newColor == prevColor)
            newColor = LevelConfig.Colors.Length - 1;
        return newColor;
    }
    
    public HubbleType GetHubbleType()
    {
        int randomNumber = Prng.Next(0, 100000);
        float comparer = 0f;

        comparer += popLivesChance * 100000f;
        if (randomNumber < comparer)
            return HubbleType.PopLive;

        comparer += rotLivesChance * 100000f;
        // TODO Delete rotations live concept
        if (randomNumber < comparer)
            return HubbleType.Usual;
//            return HubbleType.RotationLive;

        comparer += proceduralGenerationConfig.multiplierChance * 100000f;
        if (randomNumber < comparer)
            return HubbleType.Multiplier;

        return HubbleType.Usual;
    }
    
    private float GetProbabilityFromParamsNotShifted(float score, float lives, float startMin, float endMin,
        float startMax, float endMax, float startLivesLim, float endLivesLim, float expDecr)
    {
        // how difficult the game is in general according to the score; goes from 0 (easy) to 1 (hard) 
        float exp = Mathf.Exp(-expDecr * Mathf.Sqrt(score));
        
        // borders between witch avg value of gained lives is possible 
        float maxBorder = endMax + (startMax - endMax) * exp;
        float minBorder = endMin + (startMin - endMin) * exp;

        // how many lives is enough to stop helping the player
        float livesLim = endLivesLim + (startLivesLim - endLivesLim) * exp;
        // how much do we help player in according to his current lives
        // goes from 0 (many lives, don't help) to 1 (few lives, help)
        float help = lives < livesLim ? (1 - lives / livesLim) : 0f;

        // avg lives predicted 
        float avgLives = minBorder + (maxBorder - minBorder) * help;

        // 173 is a magic number gained in simulation,
        // that connects probability of generating lives with average lives gained per step 
        return avgLives / proceduralGenerationConfig.AvgToProbCoef;
    }
    
}
