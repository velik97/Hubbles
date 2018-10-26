using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoSingleton<InGameUIManager>
{
	/// <summary>
	/// Text of popLives
	/// </summary>
	[Header("Lives")]
	[SerializeField] private GameObject popLivesTextObj;
	[SerializeField] private IAnimatableIntegerText popLivesText;
	/// <summary>
	/// Text of rotationLives
	/// </summary>
	[SerializeField] private GameObject rotsLivesTextObj;
	[SerializeField] private IAnimatableIntegerText rotsLivesText;
	
	/// <summary>
	/// Pop lives collected on highlighted hubbles
	/// </summary>
	[Header("Extra lives")]
	[SerializeField] private GameObject extraPopsTextObj;
	private IAnimatableIntegerText extraPopsText;
	/// <summary>
	/// Rot lives collected on highlighted hubbles
	/// </summary>
	[SerializeField] private GameObject extraRotsTextObj;
	private IAnimatableIntegerText extraRotsText;
	/// <summary>
	/// Multiplier collected on highlighted hubbles
	/// </summary>
	[SerializeField] private GameObject multiplierTextObj;
	private IAnimatableIntegerText multiplierText;
	
	/// <summary>
	/// Text of total scores
	/// </summary>
	[Header("Score/Level")]
	[SerializeField] private GameObject scoreTextObj;
	private IAnimatableIntegerText scoreText;
	/// <summary>
	/// Text of next level scores
	/// </summary>
	[SerializeField] private GameObject recordTextObj;
	private IAnimatableIntegerText recordText;
	/// <summary>
	/// Text of total scores
	/// </summary>
	[SerializeField] private GameObject levelTextObj;
	private IAnimatableIntegerText levelText;
	/// <summary>
	/// Status of current level
	/// </summary>
	[SerializeField] private GameObject levelScoreStatusObj;
	private IStatusGraphics levelScoreStatus;

	public void StartGame()
	{
		FindObjectsAndNullReferences();
	}

	public void SetPopLives(int lives)
	{
		popLivesText.SetValue(lives);
	}

	public void SetRotLives(int lives)
	{
		rotsLivesText.SetValue(lives);
	}
	
	public void SetExtraPopLives(int lives)
	{
		extraPopsText.SetValue(lives);
	}

	public void SetExtraRotLives(int lives)
	{
		extraRotsText.SetValue(lives);
	}

	public void SetScore(int score)
	{
		scoreText.SetValue(score);
		int maxScoreAchievableInThisLevel = LevelConfig.LevelScores[HubblesManager.Instance.level] -
		                                    LevelConfig.LevelScores[HubblesManager.Instance.level - 1];
		int scoreAchievedInThisLevel = HubblesManager.Instance.totalScore - 
		                               LevelConfig.LevelScores[HubblesManager.Instance.level - 1];
		levelScoreStatus.SetStatus(scoreAchievedInThisLevel, maxScoreAchievableInThisLevel);
	}

	public void SetRecord(int score)
	{
		recordText.SetValue(score);
	}

	public void SetLevel(int level)
	{
		levelText.SetValue(level);
	}

	public void SetMultiplier(int multiplier)
	{
		multiplierText.SetValue(multiplier);
	}

	private void FindObjectsAndNullReferences()
	{
		popLivesText = popLivesTextObj.GetComponentInChildren<IAnimatableIntegerText>();
		rotsLivesText = rotsLivesTextObj.GetComponentInChildren<IAnimatableIntegerText>();
		
		extraPopsText = extraPopsTextObj.GetComponentInChildren<IAnimatableIntegerText>();
		extraRotsText = extraRotsTextObj.GetComponentInChildren<IAnimatableIntegerText>();
		multiplierText = multiplierTextObj.GetComponentInChildren<IAnimatableIntegerText>();
		
		scoreText = scoreTextObj.GetComponentInChildren<IAnimatableIntegerText>();
		recordText = recordTextObj.GetComponentInChildren<IAnimatableIntegerText>();
		levelText = levelTextObj.GetComponentInChildren<IAnimatableIntegerText>();
		
		levelScoreStatus = levelScoreStatusObj.GetComponentInChildren<IStatusGraphics>();
		
		SetPopLives(LevelConfig.StartPopLives);
		SetRotLives(LevelConfig.StartRotLives);

		SetExtraRotLives(0);
		SetExtraPopLives(0);
		SetMultiplier(1);
		
		SetRecord(0);
		SetScore(0);
		SetLevel(1);
	}

}
