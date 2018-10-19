using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PredefinedGenerationConfig : ScriptableObject
{
    [SerializeField]
    private HubbleStruct[] hubbles;

    public bool HasIndex(int index)
    {
        return hubbles.Length > index;
    }

    public int GetColor(int index)
    {
        return hubbles[index].color;
    }

    public HubbleType GetHubbleType(int index)
    {
        return hubbles[index].hubbleType;
    }
    
    [System.Serializable]
    private struct HubbleStruct
    {
        public int color;
        public HubbleType hubbleType;
    }
}
