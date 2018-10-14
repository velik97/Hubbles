using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Contains color and sizes for hubbles and colors for theirs highlight and content
/// </summary>
public class HubblesAppearanceInfo : MonoSingleton <HubblesAppearanceInfo> {
 
	[Range(0f,1f)]
	public float highlightLightness;
	[Range(0f,1f)]
	public float textDarkness;
	[Range(0f,1f)]
	public float hubbleSize;

	public float FitHubbleSize {
		get {
			return hubbleSize * Coord.Step.y;
		}
	}

	private Color[] usualColors;
	private Color[] darkColors;
	private Color[] lightColors;

	public Hubble hubblePrefab;
	public GameObject imagePrefab;
	public Sprite popLiveSprite;
	public Sprite rotationLiveSprite;
	public Sprite multiplierSprite;
	public GameObject textPrefab;

	public Color[] UsualColors
	{
		get
		{
			if (usualColors == null)
				UpdateColors();
			return usualColors;
		}
	}

	public Color[] DarkColors
	{
		get
		{
			if (darkColors == null)
				UpdateColors();
			return darkColors;
		}
	}

	public Color[] LightColors
	{
		get
		{
			if (lightColors == null)
				UpdateColors();
			return lightColors;
		}
	}

	public void UpdateColors () {
		int length = LevelConfig.Colors.Length; 

		usualColors = new Color[length];
		darkColors = new Color[length];
		lightColors = new Color[length];

		for (int i = 0; i < length; i++) {
			usualColors [i] = LevelConfig.Colors [i];
			lightColors [i] = Color.Lerp (Color.white, LevelConfig.Colors [i], highlightLightness);
			darkColors [i] = Color.Lerp (LevelConfig.Colors [i], Color.black, textDarkness);
		}
	}

}
