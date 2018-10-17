using UnityEngine;
using System.Collections;

/// <summary>
/// Level Configuration.
/// </summary>
[CreateAssetMenu]
public class LevelConfig : ScriptableObject {

	[SerializeField]
	private int width, height;

	[SerializeField]
	private int startPopLives;
	[SerializeField]
	private int startRotations;
	
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
