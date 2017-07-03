using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoSingleton <GameManager> {

	public UnityEvent OnStartScene;
	public UnityEvent OnStartGameSuccess;
	public UnityEvent OnStartGameNotSuccess;
	public UnityEvent OnWin;
	public UnityEvent OnLose;

	public SceneLoader sceneLoader;

	public bool gameIsStarted;

	public Button nextButton;

	void Start () {
		gameIsStarted = false;
		if (LevelConfigHandler.CurrentIndex == LevelConfigHandler.ConfigsCount)
			nextButton.interactable = false;
		OnStartScene.Invoke ();
	}

	public void StartGame () {
		if (LivesManager.Instance.LivesAreEmpty) {
			OnStartGameNotSuccess.Invoke ();
		} else {
			LivesManager.Instance.TakeLive ();
			gameIsStarted = true;
			OnStartGameSuccess.Invoke ();
		}
	}

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
		OnWin.Invoke ();
	}

	public void Lose () {
		OnLose.Invoke ();
	}

	public void Restart () {
		sceneLoader.StartLoadingScene (1);
	}

	public void GoToMenu () {
		sceneLoader.StartLoadingScene (0);
	}

	public void GoToNextLevel () {
		LevelConfigHandler.CurrentIndex++;
		sceneLoader.StartLoadingScene (1);
	}

}
