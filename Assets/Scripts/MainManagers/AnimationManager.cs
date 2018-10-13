using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/// <summary>
/// Handles animation for hubbles and UI. Need change (separate game field animation and UI)
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
	/// Time of rescaling of group of hubbles before and after rotaion
	/// </summary>
	public float rescaleTime;
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
	/// Text of popLives. Need change
	/// </summary>
	[Header("UI")]
	public Text livesText;
	/// <summary>
	/// Text of total scores
	/// </summary>
	public Text scoreText;
	/// <summary>
	/// Status bar for level
	/// </summary>
	public StatusBar scoreStatus;
	/// <summary>
	/// Deprecated
	/// </summary>
	public RectTransform firstStarMark;
	/// <summary>
	/// Deprecated
	/// </summary>
	public RectTransform secondStarMark;
	/// <summary>
	/// Deprecated
	/// </summary>
	public float starMarkWidth;

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

	private List<IEnumerator> highLightingCoroutines;
	private IEnumerator rotateIEnumerator;

	public void StartGame () {
		FindObjectsAndNullReferences();
	}

	/// <summary>
	/// Unhighlight hubbles and all UI
	/// </summary>
	public void UnHighLightEveryThing (bool unhighlightHubbles) {
		if (highLightingCoroutines.Count > 0) {
			foreach (IEnumerator coroutine in highLightingCoroutines) {
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
		if (multiplierObj.transform.localScale != Vector3.zero) {
			multiplierObj.GetComponent <Animator> ().SetBool ("Appear", false);
		}
	}

	/// <summary>
	/// Highlight hubbles group
	/// </summary>
	/// <param name="nodes">nodes to be highlighted</param>
	public void HighLightHubbles (List<Node> nodes) {
		IEnumerator coroutine = IHighLightHubbles(nodes);
		highLightingCoroutines.Add(coroutine);
		StartCoroutine(coroutine);
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
		Text popHealText = additionalPopsObj.GetComponentInChildren<Text>();
		Text rotationHealText = additionalRotationsObj.GetComponentInChildren<Text>();
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
					popHealText.text = popHeal.ToString();
					break;
				
				case HubbleType.RotationLive:
					if (!additionalRotationsIsHighLighted) {
						additionalRotationsObj.GetComponent <Animator> ().SetBool ("Appear", true);
						additionalRotationsIsHighLighted = true;
					}
					rotationHeal += 1;
					rotationHealText.text = rotationHeal.ToString();
					break;
				
				case HubbleType.Multiplier:
					if (!multiplierIsHighLighted) {
						multiplierObj.GetComponent <Animator> ().SetBool ("Appear", true);
						multiplierIsHighLighted = true;
					}
					multiplier *= 2;
					multiplayerText.text = "x" + multiplier.ToString();
					break;
			}

		}
		if (HubblesManager.Instance.allAreOneColor) {
			highLightPanel.SetColor (HubblesAppearanceInfo.Instance.lightColors[nodes[0].color], false);
			if (!multiplierIsHighLighted) {
				multiplierObj.GetComponent <Animator> ().SetBool ("Appear", true);
				multiplierIsHighLighted = true;
			}
			multiplier *= 2;
			multiplayerText.text = "x" + multiplier.ToString();
		}
	}


	/// <summary>
	/// Rescale hubble's neighbours before or after rotation
	/// </summary>
	/// <param name="node">central node</param>
	/// <param name="makeSmaller">make smaller or bigger</param>
	public void Rescale (Node node, bool makeSmaller) {
		StartCoroutine (IRescale(node, makeSmaller));
	}

	IEnumerator IRescale (Node node, bool makeSmaller) {
		isAnimating = true;
		float t = 0f;
		if (makeSmaller) {
			while (t < rescaleTime) {
				float scale = (1 + (rescalingPercent - 1) * AnimationFunctions.EasyOut(t/rescaleTime, easyRescalingPow)) * HubblesAppearanceInfo.Instance.FitHubbleSize;
				node.hubble.transform.localScale = Vector3.one * scale;
				yield return null;
				t += Time.deltaTime;
			}
			node.hubble.transform.localScale = Vector3.one * HubblesAppearanceInfo.Instance.FitHubbleSize * rescalingPercent;
		} else {
			while (t < rescaleTime) {
				float scale = (rescalingPercent + (1 - rescalingPercent) * AnimationFunctions.EasyOut(t/rescaleTime, easyRescalingPow)) * HubblesAppearanceInfo.Instance.FitHubbleSize;
				node.hubble.transform.localScale = Vector3.one * scale;
				yield return null;
				t += Time.deltaTime;
			}
			node.hubble.transform.localScale = Vector3.one * HubblesAppearanceInfo.Instance.FitHubbleSize;
		}
		isAnimating = false;
	}

	/// <summary>
	/// Start rotating nodes
	/// </summary>
	/// <param name="mainNode">central node</param>
	/// <param name="surroundingNodes">surrounding neighbours</param>
	public void StartRotating (Node mainNode, List<Node> surroundingNodes) {
		rotateIEnumerator = IRotate (mainNode, surroundingNodes);
		StartCoroutine (rotateIEnumerator);
	}

	/// <summary>
	/// Stop rotating
	/// </summary>
	public void StopRotating () {
		if (rotateIEnumerator != null) {
			StopCoroutine (rotateIEnumerator);
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
				GetComponent<AudioSource> ().clip = rattle;
				GetComponent<AudioSource> ().Play ();
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
	}

	IEnumerator IPullToMap (Node mainNode, List<Node> surroundingNodes, int turns ,float originAngle) {
		isAnimating = true;
		originAngle = originAngle % 360;
		float aimAngle;
		if (originAngle < 0)
			aimAngle = turns * 60f - 360;
		else 
			aimAngle = turns * 60;
		
		if (aimAngle == -360) {
			aimAngle = 0;
		}
		
		if (originAngle > 330 && originAngle <=360 && aimAngle == 0)
			aimAngle = 360;
		
		if (originAngle < -330 && originAngle >= -360 && aimAngle == 0)
			aimAngle = -360;
		
		float t = 0f;
		float maxTime = pullTime * (Mathf.Abs(originAngle - aimAngle) / 30f);
		StartCoroutine (IRescale(mainNode, false));
		offsetAngle = originAngle;
		while (t < maxTime) {
			Rotate (mainNode, surroundingNodes, Mathf.Lerp (originAngle, aimAngle, t / maxTime));
			yield return null;
			t += Time.deltaTime;
		}
		mainNode.hubble.transform.rotation = Quaternion.Euler(0f, 0f, -aimAngle);

		yield return new WaitForSeconds (pullTime - maxTime);

		mainNode.hubble.transform.localScale = Vector3.one * HubblesAppearanceInfo.Instance.FitHubbleSize;

		foreach (Node node in surroundingNodes) {
			if (node != mainNode && node.hubble.transform != null) {
				node.hubble.transform.SetParent(this.transform);
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
		isAnimating = false;
	}


	/// <summary>
	/// Delete group of nodes. Need Change
	/// </summary>
	/// <param name="nodes">nodes to be deleted</param>
	public void DeleteGroup (List<Node> nodes) {
		//scoreStatus.SetPercentage (HubblesManager.Instance.totalScore, LevelConfigHandler.CurrentConfig.threeStarsScore);
		scoreText.text = HubblesManager.Instance.totalScore.ToString ();
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
		GetComponent<AudioSource>().clip = bigPop;
		GetComponent<AudioSource>().Play();
	}

	void FindObjectsAndNullReferences () {

		if (pointsTextsObj != null) {
			foreach (GameObject uip in pointsTextsObj) {
				if (uip != null)
					Destroy (uip);
			}
		}

		pointsTextsObj = new GameObject[HubblesAppearanceInfo.Instance.lightColors.Length];
		for (int i = 0; i < pointsTextsObj.Length; i++) {
			pointsTextsObj [i] = Instantiate (pointsPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			pointsTextsObj [i].transform.SetParent (pointsHolder.transform, false);
			pointsTextsObj [i].transform.localScale = Vector3.zero;
			Image[] images = pointsTextsObj [i].GetComponentsInChildren <Image> ();
			images [0].color = HubblesAppearanceInfo.Instance.lightColors [i];
			images [1].color = HubblesAppearanceInfo.Instance.usualColors [i];
			pointsTextsObj [i].GetComponentInChildren <Text> ().color = HubblesAppearanceInfo.Instance.darkColors [i];
		}

		highLightingCoroutines = new List<IEnumerator>();

		offsetAngle = 0f;

		additionalPopsObj.transform.localScale = Vector3.zero;
		multiplierObj.transform.localScale = Vector3.zero;

		scoreStatus.SetPercentage (0f);
		scoreText.text = 0.ToString ();
	}


}
