using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredefinedHubbleGenerator : MonoBehaviour, IHubbleGenerator
{
	[SerializeField]
	private PredefinedGenerationConfig predefinedHubbleGenerator;
	[SerializeField]
	private ProceduralHubbleGenerator proceduralHubbleGenerator;

	private int index = 0;

	private void Awake()
	{
		proceduralHubbleGenerator = GetComponentInChildren<ProceduralHubbleGenerator>();
		if (proceduralHubbleGenerator == null)
			Debug.LogError("[PredefinedHubbleGenerator] Need default hubble generator!");
	}

	public void LoadStepData(int score, int pops, int rots) {}

	public int GetColor()
	{
		if (predefinedHubbleGenerator.HasIndex(index))
			return predefinedHubbleGenerator.GetColor(index++);

		return proceduralHubbleGenerator.GetColor();
	}

	public int GetColor(int prevColor)
	{
		if (predefinedHubbleGenerator.HasIndex(index))
			return predefinedHubbleGenerator.GetColor(index++);

		return proceduralHubbleGenerator.GetColor(prevColor);
	}

	public HubbleType GetHubbleType()
	{
		if (predefinedHubbleGenerator.HasIndex(index))
			return predefinedHubbleGenerator.GetHubbleType(index);

		return proceduralHubbleGenerator.GetHubbleType();
	}
}