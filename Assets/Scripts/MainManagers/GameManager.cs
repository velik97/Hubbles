﻿using UnityEngine;
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
	
	private int record = -1;

	public int Record
	{
		get
		{
			if (record != -1) return record;
			record = 0;
			if (PlayerPrefs.HasKey("Record"))
				record = PlayerPrefs.GetInt("Record");
			return record;
		}
		set
		{
			if (value <= record)
				return;
			PlayerPrefs.SetInt("Record", value);
			record = value;
		}
	}

	private bool PassedTutorial
	{
		get
		{
			bool passed = false;
			if (PlayerPrefs.HasKey("PassedTutorial"))
				passed = PlayerPrefs.GetInt("PassedTutorial") == 1;
			return passed;
		}
		set
		{
			PlayerPrefs.SetInt("PassedTutorial", value ? 1 : 0);
		}
	}

	private void Awake()
	{
		Camera.main.ResizeInGame();
		if (!tutorialMode && !PassedTutorial)
		{
			GoToTutorial();
			return;
		}
		
		sceneLoader.EndLoadingScene();

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
		{
			PrepareAnalyticsData();
		}
	}

	void Start () {
		onStartGame.Invoke ();
	}

	public void Lose () {
		if (tutorialMode)
		{
			PassedTutorial = true;
			AnalyticsEvent.TutorialComplete();
			this.InvokeWithDelay(() => sceneLoader.StartLoadingScene("Main"), 2f);
		}
		else
		{
			onLose.Invoke();
		}
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

	public void ShowRewardedAd()
	{
		AdManager.Instance.ShowRewardedAd(ContinueAfterAd, StartNewGame, StartNewGame);
	}

	public void Restart() {
		AdManager.Instance.ShowSimpleAd(StartNewGame, StartNewGame, StartNewGame);
	}

	public void StartNewGame()
	{
		sceneLoader.StartLoadingScene("Main");
	}

	public void GoToTutorial() {
		sceneLoader.StartLoadingScene("Tutorial");
	}

	public void ContinueAfterAd()
	{
		HubblesManager.Instance.ContinueWithNewSteps(LevelConfig.AddBonusPopLives);
		MenuManager.Instance.CloseLoseMenu();
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
