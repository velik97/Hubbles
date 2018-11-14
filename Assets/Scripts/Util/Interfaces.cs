using System.ComponentModel;
using UnityEngine;

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

public interface IAnimatableIntegerText
{
	void SetValue(int value);
}

public interface ITouchSource
{
	Vector2 TouchPos();
	bool IsTouching();
	bool TouchIsOnField();
}


