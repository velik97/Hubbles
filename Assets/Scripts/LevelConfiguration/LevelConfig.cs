using UnityEngine;
using System.Collections;

/// <summary>
/// Level Configuration. Need change
/// </summary>
[CreateAssetMenu]
public class LevelConfig : ScriptableObject {

	[SerializeField]
	private int width, height;

	[SerializeField]
	private int startPopLives;
	[SerializeField]
	private int startRotations;

	
	[SerializeField] [Range(0,.33f)]
	public float popLiveChance;
	[SerializeField] [Range(0,.33f)]
	public float rotationLiveChance;
	[SerializeField] [Range(0,.33f)]
	public float multiplierChance;
	
	[SerializeField]
	private Color[] colors;

	[SerializeField]
	private int[] levelScores;

	public static LevelConfig instance;

	public static int Width {
		get { return instance.width; }
	}

	public static int Height {
		get { return instance.height; }
	}

	public static int StartPopLives
	{
		get { return instance.startPopLives; }
	}
	
	public static int StartRotationLives
	{
		get { return instance.startRotations; }
	}

	public static float PopLiveChance
	{
		get { return instance.popLiveChance; }
	}
	
	public static float RotationLiveChance
	{
		get { return instance.rotationLiveChance; }
	}

	public static float MultiplierChance
	{
		get { return instance.multiplierChance; }
	}

	public static Color[] Colors
	{
		get { return instance.colors; }
	}
	
	public static int[] LevelScores
	{
		get { return instance.levelScores; }
	}

	// ========================

	// // Types

	// 0 -- usual
	// 1 -- health
	// 2 -- multiplier

	// ========================
}
