using UnityEngine;
using System.Collections;

/// <summary>
/// Level Configuration.
/// </summary>
[CreateAssetMenu]
public class LevelConfig : ScriptableObject
{
	[SerializeField]
	private float mapScale = 1f;

	[SerializeField]
	private int width, height;

	[SerializeField]
	private int startPopLives;
	[SerializeField]
	private int startRotLives;
	
	[SerializeField]
	private int addBonusPopLives;
	
	[SerializeField]
	private Color[] colors;

	[SerializeField]
	private int[] levelScores;

	public static LevelConfig instance;

	public float MapScale
	{
		get { return mapScale; }
	}

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
	
	public static int StartRotLives
	{
		get { return instance.startRotLives; }
	}

	public static int AddBonusPopLives
	{
		get { return instance.addBonusPopLives; }
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
