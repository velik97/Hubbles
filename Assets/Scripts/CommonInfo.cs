using UnityEngine;
using System.Collections;

public class CommonInfo : MonoSingleton <CommonInfo> {

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

	public bool needLoading = false;

	public Color[] usualColors;
	public Color[] darkColors;
	public Color[] lightColors;

	public Hubble hubblePrefab;
	public GameObject heartPrefab;
	public GameObject textPrefab;

	void Awake () {
		DontDestroyOnLoad (this);
	}

	public void UpdateColors () {
		int length = LevelConfigHandler.CurrentConfig.colors.Length; 

		usualColors = new Color[length];
		darkColors = new Color[length];
		lightColors = new Color[length];

		for (int i = 0; i < length; i++) {
			usualColors [i] = LevelConfigHandler.CurrentConfig.colors [i];
			lightColors [i] = Color.Lerp (Color.white, LevelConfigHandler.CurrentConfig.colors [i], highlightLightness);
			darkColors [i] = Color.Lerp (LevelConfigHandler.CurrentConfig.colors [i], Color.black, textDarkness);
		}
	}

}
