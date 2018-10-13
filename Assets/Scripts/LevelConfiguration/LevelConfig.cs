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

	
	[SerializeField] [Range(0,1)]
	private float popLiveChance;
	[SerializeField] [Range(0,1)]
	private float rotationLiveChance;

	[SerializeField] [Range(0,1)]
	private float multiplierChance;
	
	[SerializeField]
	private Color[] colors;

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
	
	public float RotationLiveChance
	{
		get { return rotationLiveChance; }
	}

	public static float MultiplierChance
	{
		get { return instance.multiplierChance; }
	}
	
	/// <summary>
	/// Also considers map size
	/// </summary>
	public static float WeightedPopLiveChance
	{
		get { return instance.popLiveChance / (instance.width * instance.height); }
	}
	
	/// <summary>
	/// Also considers map size
	/// </summary>
	public static float WeightedRotationLiveChance
	{
		get { return instance.rotationLiveChance / (instance.width * instance.height); }
	}

	/// <summary>
	/// Also considers map size
	/// </summary>
	public static float WeightedMultiplierChance
	{
		get { return instance.multiplierChance / (instance.width * instance.height); }
	}

	public static Color[] Colors
	{
		get { return instance.colors; }
	}

	// ========================

	// // Types

	// 0 -- usual
	// 1 -- health
	// 2 -- multiplier

	// ========================
}
