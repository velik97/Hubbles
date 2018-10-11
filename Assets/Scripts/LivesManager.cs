using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Deprecated
/// </summary>
public class LivesManager : MonoSingleton <LivesManager> {

	public int maxLivesCount = 5;
	public int currentLivesCount;
	public int minutesBetweenLivesAdding;

	public HTime TimeScinceLastLiveAdd {
		get {
			HTime lastWorldTimeLiveAdded = HTime.LastWorldTimeLiveAdded;
			if (lastWorldTimeLiveAdded != null)
				return HTime.Now - HTime.LastWorldTimeLiveAdded;

			return null;
		}
	}

	public HTime TimeToNextLiveAdd {
		get {
			if (CurrentLivesCount < maxLivesCount) {
				HTime timeScinceLastLiveAdd = TimeScinceLastLiveAdd;
				if (timeScinceLastLiveAdd != null) {
					return HTime.Minutes (minutesBetweenLivesAdding) - timeScinceLastLiveAdd;
				} 
				return null;
			}
			return null;
		}
	}
		
	public int CurrentLivesCount {
		get {
			if (currentLivesCount <= 0) {
				if (PlayerPrefs.HasKey ("LastLivesCount")) {
					currentLivesCount = PlayerPrefs.GetInt ("LastLivesCount");
					return currentLivesCount;
				} else {
					currentLivesCount = maxLivesCount;
					PlayerPrefs.SetInt ("LastLivesCount", currentLivesCount);
					return currentLivesCount;
				} 
			} else {
				return currentLivesCount;
			}
		}

		set {
			currentLivesCount = value;

			if (currentLivesCount > maxLivesCount) {
				currentLivesCount = maxLivesCount;
			} else if (currentLivesCount < 0) {
				Debug.LogError ("Lives should not decrease, when they equals 'zero'");
				currentLivesCount = 0;
			}
			PlayerPrefs.SetInt ("LastLivesCount", currentLivesCount);

			if (LivesDisplay.Instance)
				LivesDisplay.Instance.UpdateDisplay ();
		}
	}

	public bool LivesAreFull {
		get {
			return CurrentLivesCount == maxLivesCount;
		}
	}

	public bool LivesAreEmpty {
		get {
			return CurrentLivesCount == 0;
		}
	}

	public void MaximizeLives () {
		CurrentLivesCount = maxLivesCount;
	}

	void Awake () {
		currentLivesCount = -1;
		Update ();
	}

	void Update () {
		if (CurrentLivesCount < maxLivesCount) {
			while (TimeToNextLiveAdd != null && TimeToNextLiveAdd <= HTime.Zero) {
				AddLives (1);
				HTime.LastWorldTimeLiveAdded += HTime.Minutes (minutesBetweenLivesAdding);
			}
		}
	}

	public void AddLives (int lives) {
		CurrentLivesCount += lives;
	}
		
	public void TakeLive () {
		if (CurrentLivesCount == maxLivesCount) {
			HTime.LastWorldTimeLiveAdded = HTime.Now;
		}

		int lives = CurrentLivesCount - 1;

		if (lives < 0) {
			
			HTime.LastWorldTimeLiveAdded -= HTime.Minutes (minutesBetweenLivesAdding);
			lives = 0;
		}

		CurrentLivesCount = lives;
	}

}
