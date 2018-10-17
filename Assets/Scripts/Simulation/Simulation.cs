using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simulating perfect gameplay to find parameters. Doesn't work!!! Deprecated
/// </summary>
public class Simulation : MonoBehaviour
{
	[SerializeField]
	private StatusBar statusBar;
	[SerializeField]
	private Text progressText;
	[SerializeField]
	private int simulationSteps;
	[SerializeField]
	private LevelConfig levelConfig;
	
	private SimulationNode[] nodes;

	private int[] gainedLives;

	private float minLiveChance = 0.01f;
	private float minMultChance = 0.01f;
	private float liveChanceStep = 0.002f;
	private float multChanceStep = 0.002f;

	private float avg;

	private void Awake()
	{
		StartCoroutine(HandleSimulation());
	}

	private IEnumerator HandleSimulation()
	{
		progressText.text = "0/3, 0/3";
		StringBuilder csv = new StringBuilder();
		StringBuilder line = new StringBuilder();
		line.Append("chance, ");
		for (int j = 0; j < 11; j++)
		{
			line.Append((minLiveChance + liveChanceStep * j).ToString() + (j < 11 ? ", " : ""));
		}
		csv.AppendLine(line.ToString());
		line = new StringBuilder();

		for (int i = 0; i < 11; i++)
		{
			line.Append((minMultChance + multChanceStep * i).ToString() + ", ");
			for (int j = 0; j < 11; j++)
			{
				progressText.text = (i+1).ToString() + "/11, " + (j+1).ToString() + "/11";
				//levelConfig.multiplierChance = minMultChance + multChanceStep * i;
				//levelConfig.popLiveChance = minLiveChance + liveChanceStep * j;

				yield return HandleSingleSimulation();
				line.Append(avg + (j < 11 ? ", " : ""));
			}

			csv.AppendLine(line.ToString());
			line = new StringBuilder();
		}
		File.WriteAllText(Application.dataPath + "/statistics.csv", csv.ToString());
		progressText.text = "DONE";
	}

	private IEnumerator HandleSingleSimulation()
	{
		LevelConfig.instance = levelConfig;
		
		gainedLives = new int[simulationSteps];
		nodes = new SimulationNode[LevelConfig.Width * LevelConfig.Height];
		
		for (var i = 0; i < nodes.Length; i++)
		{
			nodes[i].color = RandomHubbleGenerator.RandomColor();
			nodes[i].hubbleType = RandomHubbleGenerator.RandomType();
		}
		
		int randColor = RandomHubbleGenerator.RandomColor();
		for (int i = 0; i < simulationSteps; i++)
		{
			int lives = 0;
			int mult = 2;
			for (var j = 0; j < nodes.Length; j++)
			{
				if (nodes[j].color == randColor)
				{
					if (nodes[j].hubbleType == HubbleType.PopLive)
						lives++;
					else if (nodes[j].hubbleType == HubbleType.Multiplier)
						mult *= 2;
					nodes[j].color = RandomHubbleGenerator.RandomColor(randColor);
					nodes[j].hubbleType = RandomHubbleGenerator.RandomType();
				}
			}

			gainedLives[i] = lives * mult;
			randColor = RandomHubbleGenerator.RandomColor(randColor);
			if (statusBar != null)
			    statusBar.SetPercentage(i, simulationSteps);

			if (i % 2000 == 0)
				yield return null;
		}

		avg = 0;
		for (int i = 0; i < simulationSteps; i++)
		{
			avg += gainedLives[i];
		}
		avg /= simulationSteps;
		yield return null;
	}
}

public struct SimulationNode
{
	public int color;
	public HubbleType hubbleType;
}
