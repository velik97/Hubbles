using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class RandomHubbleGenerator
{
    private static System.Random prng = null;

    private static Random Prng
    {
        get
        {
            if (prng == null)
                prng = new System.Random(Mathf.RoundToInt(Time.realtimeSinceStartup * 1000f));
            return prng;
        }
    }

    private static GenerationConfig c;

    public static GenerationConfig GenerationConfig
    {
        set
        {
            multiplierChance = value.multiplierChance;
            c = value;
        }
    }

    private static float multiplierChance;
    private static float popLivesChance;
    private static float rotLivesChance;

    private static Queue<float> popLivesChanceQueue;
    private static Queue<float> rotLivesChanceQueue;

    public static void LoadStepData(int score, int pops, int rots) 
    {
        if (popLivesChanceQueue == null)
            popLivesChanceQueue = new Queue<float>();
        if (rotLivesChanceQueue == null)
            rotLivesChanceQueue = new Queue<float>();

        popLivesChanceQueue.Enqueue(GetProbabilityFromParamsNotShifted(score, pops, c.startMinPopAvg, c.endMinPopAvg,
            c.startMaxPopAvg, c.endMaxPopAvg, c.startPopLim, c.endPopLim, c.expDecrCoef));
        rotLivesChanceQueue.Enqueue(GetProbabilityFromParamsNotShifted(score, rots, c.startMinRotAvg, c.endMinRotAvg,
            c.startMaxRotAvg, c.endMaxRotAvg, c.startRotLim, c.endRotLim, c.expDecrCoef));

        popLivesChance = popLivesChanceQueue.Count > c.popStepShift
            ? popLivesChanceQueue.Dequeue()
            : popLivesChanceQueue.Peek();
        rotLivesChance = rotLivesChanceQueue.Count > c.rotStepShift
            ? rotLivesChanceQueue.Dequeue()
            : rotLivesChanceQueue.Peek();
        
//        Debug.Log("Pop avg: " + (popLivesChance*173f) + " prob: " + popLivesChance + "\n" +
//                  "Rot avg: " + (rotLivesChance*173f) + " prob: " + rotLivesChance);
    }
    
    public static int RandomColor()
    {
        return Prng.Next(0, LevelConfig.Colors.Length);
    }
    
    public static int RandomColor(int prevColor)
    {
        int newColor = Prng.Next(0, LevelConfig.Colors.Length-1);
        if (newColor == prevColor)
            newColor = LevelConfig.Colors.Length - 1;
        return newColor;
    }
    
    public static HubbleType RandomType()
    {
        int randomNumber = Prng.Next(0, 100000);
        float comparer = 0f;

        comparer += popLivesChance * 100000f;
        if (randomNumber < comparer)
            return HubbleType.PopLive;

        comparer += rotLivesChance * 100000f;
        if (randomNumber < comparer)
            return HubbleType.RotationLive;

        comparer += multiplierChance * 100000f;
        if (randomNumber < comparer)
            return HubbleType.Multiplier;

        return HubbleType.Usual;
    }
    
    private static float GetProbabilityFromParamsNotShifted(float score, float lives, float startMin, float endMin,
        float startMax, float endMax, float startLivesLim, float endLivesLim, float expDecr)
    {
        // how difficult the game is in general according to the score; goes from 0 (easy) to 1 (hard) 
        float exp = Mathf.Exp(-expDecr * Mathf.Sqrt(score));
        
        // borders between witch avg value of gained lives is possible 
        float minBorder = endMin + (startMin - endMin) * exp;
        float maxBorder = endMax + (startMax - endMax) * exp;

        // how many lives is enough to stop helping the player
        float livesLim = endLivesLim + (startLivesLim - endLivesLim) * exp;
        // how much do we help player in according to his current lives
        // goes from 0 (many lives, don't help) to 1 (few lives, help)
        float help = lives < livesLim ? (1 - lives / livesLim) : 0f;

        // avg lives predicted 
        float avgLives = minBorder + (maxBorder - minBorder) * help;

        // 173 is a magic number gained in simulation,
        // that connects probability of generating lives with average lives gained per step 
        return avgLives / 173f;
    }
    
}
