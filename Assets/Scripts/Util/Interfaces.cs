﻿using System.ComponentModel;

public interface IHubbleGenerator
{
	void LoadStepData(int score, int pops, int rots);

	int GetColor();
	int GetColor(int prevColor);
	HubbleType GetHubbleType();
}

public interface IStatusGraphics
{
	void SetStatus(float status);
}


