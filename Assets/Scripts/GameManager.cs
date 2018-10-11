using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Invokes most important game events. Need change
/// </summary>
public class GameManager : MonoSingleton <GameManager> {

	/// <summary>
	/// Invokes on scene start. Deprecated
	/// </summary>
	public UnityEvent onStartScene;
	/// <summary>
	/// Invokes on start of game, if you have enough lives. Need cahnge
	/// </summary>
	public UnityEvent onStartGameSuccess;
	/// <summary>
	/// Invokes on start of game, if don't you have enough lives. Deprecated
	/// </summary>
	public UnityEvent onStartGameNotSuccess;
	/// <summary>
	/// Invokes on victory. Deprecated
	/// </summary>
	public UnityEvent onWin;
	/// <summary>
	/// Invokes on loss
	/// </summary>
	public UnityEvent onLose;

	/// <summary>
	/// Progress animation for scene load
	/// </summary>
	public SceneLoader sceneLoader;

	/// <summary>
	/// Is game successfully started? Deprecated
	/// </summary>
	public bool gameIsStarted;

	/// <summary>
	/// Button to next level. Deprecated
	/// </summary>
	public Button nextButton;

	void Start () {
		gameIsStarted = false;
		if (LevelConfigHandler.CurrentIndex == LevelConfigHandler.ConfigsCount)
			nextButton.interactable = false;
		onStartScene.Invoke ();
	}

	/// <summary>
	/// Checks if you have lives to star game. Need change
	/// </summary>
	public void StartGame () {
		if (LivesManager.Instance.LivesAreEmpty) {
			onStartGameNotSuccess.Invoke ();
		} else {
			LivesManager.Instance.TakeLive ();
			gameIsStarted = true;
			onStartGameSuccess.Invoke ();
		}
	}

	/// <summary>
	/// Counts collected stars. Deprecated
	/// </summary>
	public void Win () {
		LevelConfigHandler.WinLevel ();

		int stars = 0;
		if (HubblesManager.Instance.totalScore > LevelConfigHandler.CurrentConfig.oneStarScore)
			stars++;
		if (HubblesManager.Instance.totalScore > LevelConfigHandler.CurrentConfig.twoStarsScore)
			stars++;
		if (HubblesManager.Instance.totalScore > LevelConfigHandler.CurrentConfig.threeStarsScore)
			stars++;
		
		int prevStars = LevelConfigHandler.GetStarsCount (LevelConfigHandler.CurrentIndex);
		if (stars > prevStars) {
			LevelConfigHandler.SetStarsCount (LevelConfigHandler.CurrentIndex, stars);
		}
		onWin.Invoke ();
	}

	public void Lose () {
		onLose.Invoke ();
	}

	public void Restart () {
		sceneLoader.StartLoadingScene (1);
	}

	public void GoToMenu () {
		sceneLoader.StartLoadingScene (0);
	}

	/// <summary>
	/// Deprecated
	/// </summary>
	public void GoToNextLevel () {
		LevelConfigHandler.CurrentIndex++;
		sceneLoader.StartLoadingScene (1);
	}

}
