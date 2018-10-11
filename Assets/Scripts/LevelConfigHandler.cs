using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles logic for level configs. Deprecated
/// </summary>
[CreateAssetMenu]
public class LevelConfigHandler : ScriptableSingleton <LevelConfigHandler> {

	public List <LevelConfig> configs;

	public static int currentIndex = 1;
	private static bool indexChanged = true;

	private static LevelConfig currentConfig;

	public static void WinLevel () {
		if (CurrentIndex == LastDoneLevel + 1 && CurrentIndex < ConfigsCount)
			LastDoneLevel++;
	}

	private static int lastDoneLevel = 0;

	public static int LastDoneLevel {
		get {
			if (lastDoneLevel == 0) {
				if (PlayerPrefs.HasKey ("LastDoneLevel"))
					lastDoneLevel = PlayerPrefs.GetInt ("LastDoneLevel");
				else {
					PlayerPrefs.SetInt ("LastDoneLevel", lastDoneLevel);
				}
			}
			return lastDoneLevel;
		}
		set {
			lastDoneLevel = value;
			PlayerPrefs.SetInt ("LastDoneLevel", value);
		}
	}

	public static void SetStarsCount (int level, int stars) {
		PlayerPrefs.SetInt ("Stars for level " + level.ToString (), stars);
	}

	public static int GetStarsCount (int level) {
		int stars = 0;
		if (PlayerPrefs.HasKey ("Stars for level " + level.ToString ())) {
			stars = PlayerPrefs.GetInt ("Stars for level " + level.ToString ());
		} else {
			PlayerPrefs.SetInt ("Stars for level " + level.ToString (), stars);
		}
		return stars;
	}

	public static LevelConfig GetConfig (int i) {
		if (ConfigsCount < i) {
			Debug.LogError ("[Level Config Handler] Config wasn't loaded\n There is only " + ConfigsCount +
				" level(s), but want to take the " + (i + 1).ToString () + " one (index is " + i.ToString () + ")");
		}
		return Instance.configs[i - 1];
	}

	public static LevelConfig CreateConfig (int i) {
		if (i < ConfigsCount) {
			Debug.LogError ("[Level Config Handler exception] Config wasn't created\n" +
			" Level " + i.ToString () + " already exists, but you are trying to create it");
			return null;
		} else if (i > ConfigsCount) {
			Debug.LogError ("[Level Config Handler exception] Config wasn't created\n" +
			"There is only " + ConfigsCount + "levels, but you are trying to create " + i.ToString ());
			return null;
		}
		LevelConfig newLevelConfig = new LevelConfig();
		Instance.configs.Add (newLevelConfig);

		return newLevelConfig;
	}

	public static void DeleteLastConfig () {
		Instance.configs.RemoveAt (Instance.configs.Count - 1);
	}

	public static int ConfigsCount {
		get {
			if (Instance == null) {
				Debug.Log ("null instance"); 
			} else if (Instance.configs == null) {
				Debug.Log ("null list"); 
			}

			return Instance.configs.Count;
		}
	}

	public static int CurrentIndex {
		get {
			return currentIndex;
		}
		set {
			if (currentIndex != value)
				indexChanged = true;
			currentIndex = value;
		}
	}

	public static LevelConfig CurrentConfig {
		get {
			if (indexChanged || currentConfig == null) {
				currentConfig = GetConfig (CurrentIndex);
				indexChanged = false;
			}

			return currentConfig;
		}
	}

}
