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
    
    public static int RandomColor(int prevColor)
    {
        int newColor = Prng.Next(0, LevelConfig.Colors.Length-1);
        if (newColor == prevColor)
            newColor = LevelConfig.Colors.Length - 1;
        return newColor;
    }
    
    public static HubbleType RandomType()
    {
        int randomNumber = Prng.Next(0, 1000);
        float comparer = 0f;

        comparer += LevelConfig.PopLiveChance * 1000f;
        if (randomNumber < comparer)
            return HubbleType.PopLive;

        comparer += LevelConfig.RotationLiveChance * 1000f;
        if (randomNumber < comparer)
            return HubbleType.RotationLive;

        comparer += LevelConfig.MultiplierChance * 1000f;
        if (randomNumber < comparer)
            return HubbleType.Multiplier;

        return HubbleType.Usual;
    }
    
    
    
}
