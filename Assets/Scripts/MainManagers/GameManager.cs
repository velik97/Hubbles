using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using RemoteSettings = UnityEngine.RemoteSettings;

/// <summary>
/// Invokes most important game events.
/// </summary>
public class GameManager : MonoSingleton <GameManager>
{
	/// <summary>
	/// Configuration for the game
	/// </summary>
	public LevelConfig levelConfig;
	/// <summary>
	/// Invokes on start of game, if you have enough popLives.
	/// </summary>
	public UnityEvent onStartGame;
	/// <summary>
	/// Invokes on loss
	/// </summary>
	public UnityEvent onLose;

	/// <summary>
	/// Progress animation for scene load
	/// </summary>
	public SceneLoader sceneLoader;

	public bool tutorialMode;

	private Dictionary<string, object> analyticsData = new Dictionary<string, object>();

	private void Awake()
	{
		LevelConfig.instance = levelConfig;
		Coord.mapScale = levelConfig.MapScale;
		
		onStartGame.AddListener(MapGenerator.Instance.StartGame);
		onStartGame.AddListener(HubblesManager.Instance.StartGame);
		onStartGame.AddListener(AnimationManager.Instance.StartGame);
		onStartGame.AddListener(InGameUIManager.Instance.StartGame);

		if (tutorialMode)
		{
			onStartGame.AddListener(TutorialManager.Instance.StartGame);
		}
		else
			PrepareAnalyticsData();
	}

	void Start () {
		onStartGame.Invoke ();
	}

	public void Lose () {
		if (tutorialMode)
			GoToMenu();
		else
			onLose.Invoke ();
	}

	public void LevelDone(int level, int popLives, int rotLives, int totalPopsPerLevel, int totalRotsPerLevel,
		int totalOneColorGroupPopPerLevel)
	{
		if (tutorialMode)
			return;
		
		analyticsData["Pop Lives"] = popLives;
		analyticsData["Rotation Lives"] = rotLives;
		analyticsData["Total Pops"] = totalPopsPerLevel;
		analyticsData["Total Rotations"] = totalRotsPerLevel;
		analyticsData["Total One Color Group Pop Per Level"] = totalOneColorGroupPopPerLevel;
		AnalyticsEvent.LevelComplete(level, analyticsData);
	}

	public void Restart () {
		// TODO Change to scene index 1
		sceneLoader.StartLoadingScene (1);
	}

	public void GoToMenu () {
		sceneLoader.StartLoadingScene (1);
	}

	private void PrepareAnalyticsData()
	{
		analyticsData.Add("Pop Lives", 0);
		analyticsData.Add("Rotation Lives", 0);
		analyticsData.Add("Total Pops", 0);
		analyticsData.Add("Total Rotations", 0);
		analyticsData.Add("Total One Color Group Pop Per Level", 0);
	}
}
