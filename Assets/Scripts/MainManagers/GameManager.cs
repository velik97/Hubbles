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
	/// Generation configuration for the game
	/// </summary>
	public GenerationConfig generationConfig;
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

	private Dictionary<string, object> analyticsData = new Dictionary<string, object>();

	private void Awake()
	{
		LevelConfig.instance = levelConfig;
		Coord.mapScale = levelConfig.MapScale;
		PrepareAnalyticsData();
		
		onStartGame.AddListener(sceneLoader.EndLoadingScene);
		onStartGame.AddListener(MapGenerator.Instance.StartGame);
		onStartGame.AddListener(AnimationManager.Instance.StartGame);
		onStartGame.AddListener(HubblesManager.Instance.StartGame);		
	}

	void Start () {
		onStartGame.Invoke ();
	}

	public void Lose () {
		onLose.Invoke ();
	}

	public void LevelDone(int level, int popLives, int rotLives, int totalPopsPerLevel, int totalRotsPerLevel,
		int totalOneColorGroupPopPerLevel)
	{
		analyticsData["Pop Lives"] = popLives;
		analyticsData["Rotation Lives"] = rotLives;
		analyticsData["Total Pops"] = totalPopsPerLevel;
		analyticsData["Total Rotations"] = totalRotsPerLevel;
		analyticsData["Total One Color Group Pop Per Level"] = totalOneColorGroupPopPerLevel;
		AnalyticsEvent.LevelComplete(level, analyticsData);
	}

	public void Restart () {
		// TODO Change to scene index 1
		sceneLoader.StartLoadingScene (0);
	}

	public void GoToMenu () {
		sceneLoader.StartLoadingScene (0);
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
