using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Invokes most important game events. Need change
/// </summary>
public class GameManager : MonoSingleton <GameManager>
{
	/// <summary>
	/// Configuration for the game
	/// </summary>
	public LevelConfig levelConfig;
	/// <summary>
	/// Invokes on start of game, if you have enough popLives. Need change
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

	private void Awake()
	{
		LevelConfig.instance = levelConfig;
		
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

	public void Restart () {
		// TODO Change to scene index 1
		sceneLoader.StartLoadingScene (0);
	}

	public void GoToMenu () {
		sceneLoader.StartLoadingScene (0);
	}


}
