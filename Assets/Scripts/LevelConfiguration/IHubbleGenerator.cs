using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHubbleGenerator
{
    void LoadStepData(int score, int pops, int rots);

    int GetColor();
    int GetColor(int prevColor);
    HubbleType GetType();
}
