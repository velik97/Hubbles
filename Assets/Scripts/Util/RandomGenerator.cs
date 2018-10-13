using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class RandomGenerator
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
    
    public static int RandomColor()
    {
        return Prng.Next(0, LevelConfig.Colors.Length);
    }
    
    public static HubbleType RandomType()
    {
        int randomNumber = Prng.Next(0, 1000);
        float comparer = 0f;

        comparer += LevelConfig.WeightedPopLiveChance;
        if (randomNumber < comparer)
            return HubbleType.PopLive;

        comparer += LevelConfig.WeightedRotationLiveChance;
        if (randomNumber < comparer)
            return HubbleType.RotationLive;

        comparer += LevelConfig.WeightedMultiplierChance;
        if (randomNumber < comparer)
            return HubbleType.Multiplier;

        return HubbleType.Usual;
    }
    
    
    
}
