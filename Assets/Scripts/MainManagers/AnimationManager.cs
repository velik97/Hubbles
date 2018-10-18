using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/// <summary>
/// Handles animation for hubbles and UI.
/// </summary>
public class AnimationManager : MonoSingleton <AnimationManager> {

	/// <summary>
	/// Time of highlight for a hubble
	/// </summary>
	[Header("HighLighting")]
	public float highLightTime;
	/// <summary>
	/// Time between highlights of hubbles
	/// </summary>
	public float deltaHighLightTime;
	/// <summary>
	/// Pitch of pop for the first hubble
	/// </summary>
	public float startPitch;
	/// <summary>
	/// Difference between pitch of hubbles' pop
	/// </summary>
	public float pitchStep;
	/// <summary>
	/// Time of 'all of one color' panel highlight
	/// </summary>
	public float panelHighLightTime;
	/// <summary>
	/// Panel, that highlights, when all hubbles of certain color are collected
	/// </summary>
	public HighlightPanel highLightPanel;

	/// <summary>
	/// Time of pulling back after rotation
	/// </summary>
	[Header("Rotation")]
	public float pullTime;
	/// <summary>
	/// percent of rescaling when rotating
	/// </summary>
	[Range(0,1)] public float rescalingPercent;
	/// <summary>
	/// Pow of ease function for rotation
	/// </summary>
	public int easyRotationPow;
	/// <summary>
	/// Pow of ease function for rescaling when rotating
	/// </summary>
	public int easyRescalingPow;

	/// <summary>
	/// Time for deleting a group of hubbles
	/// </summary>
	[Header("Deleting")]
	public float deleteTime;

	/// <summary>
	/// Sound of group pop
	/// </summary>
	[Header("Sounds")]
	public AudioClip bigPop;
	/// <summary>
	/// Sound of rotating rattle
	/// </summary>
	public AudioClip rattle;

	/// <summary>
	/// Game object, that holds points' UI. Deprecated
	/// </summary>
	[Header("Objects")]
	public GameObject pointsHolder;
	/// <summary>
	/// Prefab of points UI
	/// </summary>
	public GameObject pointsPrefab;

	/// <summary>
	/// Text of popLives
	/// </summary>
	[Header("UI")]
	public Text popLivesText;
	/// <summary>
	/// Text of rotationLives
	/// </summary>
	public Text rotationLivesText;
	/// <summary>
	/// Text of total scores
	/// </summary>
	public Text scoreText;
	/// <summary>
	/// Text of next level scores
	/// </summary>
	public Text nextLevelScoreText;
	/// <summary>
	/// Text of total scores
	/// </summary>
	public Text levelText;
	/// <summary>
	/// Status bar for level
	/// </summary>
	public StatusBar scoreStatus;
	/// <summary>
	/// Deprecated
	/// </summary>

	/// <summary>
	/// UI objects showing points, highlighted in certain step
	/// </summary>
	private GameObject[] pointsTextsObj;
	/// <summary>
	/// UI objects showing points, highlighted in current step
	/// </summary>
	private GameObject currentPointsTextObj;
	/// <summary>
	/// Is points UI highlighted
	/// </summary>
	private bool pointsTextIsHighLighted;
	
	public GameObject additionalPopsObj;
	private bool additionalPopsIsHighLighted;
	
	public GameObject additionalRotationsObj;
	private bool additionalRotationsIsHighLighted;

	public GameObject multiplierObj;
	private bool multiplierIsHighLighted;

	private float offsetAngle;
	private float currentAngle;
	[Space(10)]
	
	/*[HideInInspector] */public bool isAnimating;

	private List<Coroutine> highLightingCoroutines;
	private List<Coroutine> rotateCoroutines;

	public void StartGame () {
		FindObjectsAndNullReferences();
	}

	/// <summary>
	/// Unhighlight hubbles and all UI
	/// </summary>
	public void UnHighLightEveryThing (bool unhighlightHubbles) {
		if (highLightingCoroutines.Count > 0) {
			foreach (Coroutine coroutine in highLightingCoroutines) {
				StopCoroutine (coroutine);
			}
		}
		highLightingCoroutines.Clear();

		if (unhighlightHubbles) {
			foreach (Node node in HubblesManager.Instance.oneColorGroup) {
				node.SetActive (false);
				node.hubble.UnHighlight ();
			}
		}
		highLightPanel.SetColor (new Color (1, 1, 1, 0), false);
		if (currentPointsTextObj!=null) {
			if (currentPointsTextObj.transform.localScale != Vector3.zero) {
				currentPointsTextObj.GetComponent <Animator> ().SetBool ("Appear", false);
			}
		}
		if (additionalPopsObj.transform.localScale != Vector3.zero) {
			additionalPopsObj.GetComponent <Animator> ().SetBool ("Appear", false);
		}
		if (additionalRotationsObj.transform.localScale != Vector3.zero) {
			additionalRotationsObj.GetComponent <Animator> ().SetBool ("Appear", false);
		}
		if (multiplierObj.transform.localScale != Vector3.zero) {
			multiplierObj.GetComponent <Animator> ().SetBool ("Appear", false);
		}
	}

	/// <summary>
	/// Highlight hubbles group
	/// </summary>
	/// <param name="nodes">nodes to be highlighted</param>
	public void HighLightHubbles (List<Node> nodes) {
		highLightingCoroutines.Add(StartCoroutine(IHighLightHubbles(nodes)));
	}
	
	IEnumerator IHighLightHubbles (List<Node> nodes) {
		pointsTextIsHighLighted = false;
		additionalPopsIsHighLighted = false;
		additionalRotationsIsHighLighted = false;
		multiplierIsHighLighted = false;

		float pitch = Mathf.Exp(startPitch);
		int points = 0;
		int popHeal = 0;
		int rotationHeal = 0;
		int multiplier = 1;
		currentPointsTextObj = pointsTextsObj[nodes[0].color];

		Text pointsText = currentPointsTextObj.GetComponentInChildren<Text>();
		Text additionalPopHealText = additionalPopsObj.GetComponentInChildren<Text>();
		Text additionalRotationHealText = additionalRotationsObj.GetComponentInChildren<Text>();
		Text multiplayerText = multiplierObj.GetComponentInChildren<Text>();

		foreach (Node node in nodes) {
			pitch += pitchStep;
			node.hubble.Highlight (pitch);

			yield return new WaitForSeconds (deltaHighLightTime);
			
			switch (node.type)
			{
				case HubbleType.Usual:
					if (!pointsTextIsHighLighted) {
						currentPointsTextObj.GetComponent <Animator> ().SetBool ("Appear", true);
						pointsTextIsHighLighted = true;
					}
					points += node.points;
					pointsText.text = points.ToString();
					break;
				
				case HubbleType.PopLive:
					if (!additionalPopsIsHighLighted) {
						additionalPopsObj.GetComponent <Animator> ().SetBool ("Appear", true);
						additionalPopsIsHighLighted = true;
					}
					popHeal += 1;
					additionalPopHealText.text = "+ " + (popHeal*multiplier);
					break;
				
				case HubbleType.RotationLive:
					if (!additionalRotationsIsHighLighted) {
						additionalRotationsObj.GetComponent <Animator> ().SetBool ("Appear", true);
						additionalRotationsIsHighLighted = true;
					}
					rotationHeal += 1;
					additionalRotationHealText.text = "+ " + (rotationHeal*multiplier);
					break;
				
				case HubbleType.Multiplier:
					if (!multiplierIsHighLighted) {
						multiplierObj.GetComponent <Animator> ().SetBool ("Appear", true);
						multiplierIsHighLighted = true;
					}
					multiplier *= 2;
					multiplayerText.text = "x" + multiplier;
					additionalPopHealText.text = "+ " + (popHeal*multiplier);
					additionalRotationHealText.text = "+ " + (rotationHeal*multiplier);
					break;
			}

		}
		if (HubblesManager.Instance.allAreOneColor) {
			highLightPanel.SetColor (HubblesAppearanceInfo.Instance.LightColors[nodes[0].color], false);
			if (!multiplierIsHighLighted) {
				multiplierObj.GetComponent <Animator> ().SetBool ("Appear", true);
				multiplierIsHighLighted = true;
			}
			multiplier *= 2;
			multiplayerText.text = "x" + multiplier;
			additionalPopHealText.text = "+ " + (popHeal*multiplier);
			additionalRotationHealText.text = "+ " + (rotationHeal*multiplier);
		}
	}


	/// <summary>
	/// Rescale hubble's neighbours before or after rotation
	/// </summary>
	/// <param name="node">central node</param>
	/// <param name="makeSmaller">make smaller or bigger</param>
	public void Rescale (Node node, bool makeSmaller) {
		rotateCoroutines.Add( StartCoroutine (IRescale(node, makeSmaller)));
	}

	IEnumerator IRescale (Node node, bool makeSmaller) {
		if (!makeSmaller)
			isAnimating = true;
		float t = 0f;
		if (makeSmaller) {
			while (t < pullTime) {
				float scale = (1 + (rescalingPercent - 1) * AnimationFunctions.EasyOut(t/pullTime, easyRescalingPow)) * HubblesAppearanceInfo.Instance.FitHubbleSize;
				node.hubble.transform.localScale = Vector3.one * scale;
				yield return null;
				t += Time.deltaTime;
			}
			node.hubble.transform.localScale = Vector3.one * HubblesAppearanceInfo.Instance.FitHubbleSize * rescalingPercent;
		} else {
			while (t < pullTime) {
				float scale = (rescalingPercent + (1 - rescalingPercent) * AnimationFunctions.EasyOut(t/pullTime, easyRescalingPow)) * HubblesAppearanceInfo.Instance.FitHubbleSize;
				node.hubble.transform.localScale = Vector3.one * scale;
				yield return null;
				t += Time.deltaTime;
			}
			node.hubble.transform.localScale = Vector3.one * HubblesAppearanceInfo.Instance.FitHubbleSize;
		}
		if (!makeSmaller)
			isAnimating = false;
	}

	/// <summary>
	/// Start rotating nodes
	/// </summary>
	/// <param name="mainNode">central node</param>
	/// <param name="surroundingNodes">surrounding neighbours</param>
	public void StartRotating (Node mainNode, List<Node> surroundingNodes) {
		rotateCoroutines.Add(StartCoroutine (IRotate (mainNode, surroundingNodes)));
	}

	/// <summary>
	/// Stop rotating
	/// </summary>
	public void StopRotating () {
		if (rotateCoroutines != null)
		{
			foreach (var coroutine in rotateCoroutines)
			{
				StopCoroutine(coroutine);
			}
			rotateCoroutines.Clear();
		}
	}

	/// <summary>
	/// Handle rotation
	/// </summary>
	/// <param name="mainNode">central node</param>
	/// <param name="surroundingNodes">surrounding neighbours</param>
	IEnumerator IRotate (Node mainNode, List<Node> surroundingNodes) {
		while (true) {
			Rotate (mainNode, surroundingNodes, TouchManager.Instance.angle);
			if (Mathf.Abs (offsetAngle - TouchManager.Instance.angle) > 30f) {
				if (TouchManager.Instance.angle > offsetAngle)
					offsetAngle += 60;
				else
					offsetAngle -= 60;
				SoundManager.Instance.Play(rattle);
			}
			yield return null;
		}
	}
	
	/// <summary>
	/// Set rotation
	/// </summary>
	/// <param name="mainNode">central node</param>
	/// <param name="surroundingNodes">surrounding neighbours</param>
	/// <param name="angle">angle of rotation</param>
	void Rotate (Node mainNode, List<Node> surroundingNodes, float angle) {
		currentAngle = (((int)angle) / 60) * 60f + AnimationFunctions.EasyInOut((angle % 60) / 60 , easyRotationPow) * 60;
		mainNode.hubble.transform.rotation = Quaternion.Euler (new Vector3(0,0, - currentAngle));
		foreach (Node node in surroundingNodes) {
			node.hubble.content.transform.rotation = Quaternion.identity;
		}
		mainNode.hubble.content.transform.rotation = Quaternion.identity;
	}


	/// <summary>
	/// Pull back to map after rotation
	/// </summary>
	/// <param name="mainNode">central node</param>
	/// <param name="surroundingNodes">surrounding neighbours</param>
	/// <param name="turns">the whole number of turns</param>
	/// <param name="originAngle">made number of turns (not whole)</param>
	public void PullToMap (Node mainNode, List<Node> surroundingNodes, int turns, float originAngle) {
		StartCoroutine (IPullToMap(mainNode, surroundingNodes, turns, originAngle));
		StartCoroutine (IRescale(mainNode, false));
	}

	IEnumerator IPullToMap (Node mainNode, List<Node> surroundingNodes, int turns ,float originAngle) {
		originAngle = originAngle % 360;
		float aimAngle;
		if (originAngle < 0)
			aimAngle = turns * 60f - 360;
		else 
			aimAngle = turns * 60;
		
		if (aimAngle == -360) {
			aimAngle = 0;
		}
		
		if (originAngle > 330 && originAngle <= 360 && aimAngle == 0)
			aimAngle = 360;
		
		if (originAngle < -330 && originAngle >= -360 && aimAngle == 0)
			aimAngle = -360;
		
		float t = 0f;
		float maxTime = pullTime * (Mathf.Abs(originAngle - aimAngle) / 30f);
		offsetAngle = originAngle;
		while (t < maxTime) {
			Rotate (mainNode, surroundingNodes, Mathf.Lerp (originAngle, aimAngle, t / maxTime));
			yield return null;
			t += Time.deltaTime;
		}
		mainNode.hubble.transform.rotation = Quaternion.Euler(0f, 0f, -aimAngle);

		yield return new WaitForSeconds (pullTime - maxTime);

		mainNode.hubble.transform.localScale = Vector3.one * HubblesAppearanceInfo.Instance.FitHubbleSize;

		yield return new WaitUntil(() => !isAnimating);
		foreach (Node node in surroundingNodes) {
			if (node != mainNode && node.hubble.transform != null) {
				node.hubble.transform.SetParent(MapGenerator.Instance.transform);
				node.hubble.transform.rotation = Quaternion.identity;
				node.hubble.content.transform.rotation = Quaternion.identity;
				node.hubble.transform.localScale = Vector3.one * HubblesAppearanceInfo.Instance.FitHubbleSize;
			}
		}
		mainNode.hubble.transform.rotation = Quaternion.identity;
		mainNode.hubble.content.transform.rotation = Quaternion.identity;
		surroundingNodes.Clear ();
		if (turns != 0) {
			HubblesManager.Instance.Turn (turns);
		}
		offsetAngle = 0f;
		rotationLivesText.text = HubblesManager.Instance.rotLives.ToString();
	}


	/// <summary>
	/// Delete group of nodes.
	/// </summary>
	/// <param name="nodes">nodes to be deleted</param>
	public void DeleteGroup (List<Node> nodes) {
		scoreText.text = HubblesManager.Instance.totalScore.ToString ();
		nextLevelScoreText.text = LevelConfig.LevelScores[HubblesManager.Instance.level].ToString();
		levelText.text = HubblesManager.Instance.level.ToString ();
		int maxScoreAchievableInThisLevel = LevelConfig.LevelScores[HubblesManager.Instance.level] -
		                                    LevelConfig.LevelScores[HubblesManager.Instance.level - 1];
		int scoreAchievedInThisLevel = HubblesManager.Instance.totalScore - 
		                               LevelConfig.LevelScores[HubblesManager.Instance.level - 1];
		scoreStatus.SetPercentage(scoreAchievedInThisLevel, maxScoreAchievableInThisLevel);
		StartCoroutine (IDeleteGroup(nodes));
	}

	IEnumerator IDeleteGroup (List<Node> nodes) {
		isAnimating = true;
		foreach (Node node in nodes) {
			node.SetActive (false);
			node.hubble.DisAppear ();
		}
		yield return new WaitForSeconds(deleteTime);
		StartCoroutine (MapGenerator.Instance.ReestablishMap (nodes));
		SoundManager.Instance.Play(bigPop);
		popLivesText.text = HubblesManager.Instance.popLives.ToString();
		rotationLivesText.text = HubblesManager.Instance.rotLives.ToString();
	}

	void FindObjectsAndNullReferences () {

		if (pointsTextsObj != null) {
			foreach (GameObject uip in pointsTextsObj) {
				if (uip != null)
					Destroy (uip);
			}
		}

		pointsTextsObj = new GameObject[HubblesAppearanceInfo.Instance.LightColors.Length];
		for (int i = 0; i < pointsTextsObj.Length; i++) {
			pointsTextsObj [i] = Instantiate (pointsPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			pointsTextsObj [i].transform.SetParent (pointsHolder.transform, false);
			pointsTextsObj [i].transform.localScale = Vector3.zero;
			Image[] images = pointsTextsObj [i].GetComponentsInChildren <Image> ();
			images [0].color = HubblesAppearanceInfo.Instance.LightColors [i];
			images [1].color = HubblesAppearanceInfo.Instance.UsualColors [i];
			pointsTextsObj [i].GetComponentInChildren <Text> ().color = HubblesAppearanceInfo.Instance.DarkColors [i];
		}

		highLightingCoroutines = new List<Coroutine>();
		rotateCoroutines = new List<Coroutine>(); 

		offsetAngle = 0f;
 
		additionalPopsObj.transform.localScale = Vector3.zero;
		additionalRotationsObj.transform.localScale = Vector3.zero;
		multiplierObj.transform.localScale = Vector3.zero;

		popLivesText.text = LevelConfig.StartPopLives.ToString();
		rotationLivesText.text = LevelConfig.StartRotationLives.ToString();

		scoreStatus.SetPercentage (0f);
		scoreText.text = 0.ToString ();
		nextLevelScoreText.text = LevelConfig.LevelScores[1].ToString();
		levelText.text = 1.ToString();
	}


}
