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

	public Color[] usualColors;
	public Color[] darkColors;
	public Color[] lightColors;

	public Hubble hubblePrefab;
	public GameObject imagePrefab;
	public Image popLiveImage;
	public Image rotationLiveImage;
	public Image multiplierImage;
	public GameObject textPrefab;

	void Awake () {
		DontDestroyOnLoad (this);
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
