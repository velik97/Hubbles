using UnityEngine;
using System.Collections;
using AnimationF;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class AnimationManager : MonoSingleton <AnimationManager> {

	[Header("HighLighting")]
	public float highLightTime;
	public float deltaHighLightTime;
	[Range(1,2)]public float scaleMultilpayer;
	public float startPitch;
	public float pitchStep;
	public float panelHighLightTime;
	public HighlightPanel highLightPanel;
	private Color[] panelColors;
	private Color BGColor;

	[Header("Rotation")]
	public float pullTime;
	public float rescaleTime;
	[Range(0,1)] public float rescalingPercent;
	public int easyRotationPow;
	public int easyRescalingPow;

	[Header("Deleting")]
	public float deleteTime;

	[Header("Sounds")]
	public AudioClip bigPop;
	public AudioClip rattle;

	[Header("Objects")]
	public GameObject pointsHolder;
	public GameObject pointsPrefab;

	[Header("UI")]
	public Text livesText;
	public float pickedIcnonsAppearenceTime;

	public Text scoreText;
	public StatusBar scoreStatus;
	public RectTransform firstStarMark;
	public RectTransform secondStarMark;
	public float starMarkWidth;

	private GameObject[] pointsTextsObj;
	private GameObject currentPointsTextObj;
	private bool pointsTextIsHighLighted;
	private bool pickedIconsAreUnHighLighting;
	
	public GameObject heartObj;
	private int healths;
	private bool heartIsHighLighted;

	public GameObject multiplayerObj;
	private int multiplayer;
	private bool multiplayerIsHighLighted;

	private int points;

	private float offsetAngle;
	private float currentAngle;
	[Space(10)]

	/*[HideInInspector] */public bool isAnimating;
	private bool panelIsUnHighLighting;
	

	private List<IEnumerator> highLightingCoroutines;
	private IEnumerator rotateIEnumerator;

	public void StartGame () {
		FindObjectsAndNullReferences();
	}

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
		if (heartObj.transform.localScale != Vector3.zero) {
			heartObj.GetComponent <Animator> ().SetBool ("Appear", false);
		}
		if (multiplayerObj.transform.localScale != Vector3.zero) {
			multiplayerObj.GetComponent <Animator> ().SetBool ("Appear", false);
		}
	}



	public void HighLightHubbles (List<Node> nodes) {
		IEnumerator coroutine = IHighLightHubbles(nodes);
		highLightingCoroutines.Add(coroutine);
		StartCoroutine(coroutine);
	}
	
	IEnumerator IHighLightHubbles (List<Node> nodes) {
		pointsTextIsHighLighted = false;
		heartIsHighLighted = false;
		multiplayerIsHighLighted = false;

		float pitch = Mathf.Exp(startPitch);
		points = 0;
		healths = 0;
		multiplayer = 1;
		currentPointsTextObj = pointsTextsObj[nodes[0].color - 1];

		IEnumerator coroutine;
		Text pointsText = currentPointsTextObj.GetComponentInChildren<Text>();
		Text heartText = heartObj.GetComponentInChildren<Text>();
		Text multiplayerText = multiplayerObj.GetComponentInChildren<Text>();

		foreach (Node node in nodes) {
			pitch += pitchStep;
			node.hubble.Highlight (pitch);

			yield return new WaitForSeconds (deltaHighLightTime);

			if (node.type == -1) {
				if (!pointsTextIsHighLighted) {
					currentPointsTextObj.GetComponent <Animator> ().SetBool ("Appear", true);
					pointsTextIsHighLighted = true;
				}
				points += node.points;
				pointsText.text = points.ToString();
			} else if (node.type == 1) {
				if (!heartIsHighLighted) {
					heartObj.GetComponent <Animator> ().SetBool ("Appear", true);
					heartIsHighLighted = true;
				}
				healths += 4;
				heartText.text = healths.ToString();
			} else if (node.type == 2) {
				if (!multiplayerIsHighLighted) {
					multiplayerObj.GetComponent <Animator> ().SetBool ("Appear", true);
					multiplayerIsHighLighted = true;
				}
				multiplayer *= 2;
				multiplayerText.text = "x" + multiplayer.ToString();
			}

		}
		if (HubblesManager.Instance.allAreOneColor) {
			highLightPanel.SetColor (CommonInfo.Instance.lightColors[nodes[0].color - 1], false);
			if (!multiplayerIsHighLighted) {
				multiplayerObj.GetComponent <Animator> ().SetBool ("Appear", true);
				multiplayerIsHighLighted = true;
			}
			multiplayer *= 2;
			multiplayerText.text = "x" + multiplayer.ToString();
		}
	}








	public void Rescale (Node node, bool makeSmaller) {
		StartCoroutine (IRescale(node, makeSmaller));
	}

	IEnumerator IRescale (Node node, bool makeSmaller) {
		isAnimating = true;
		float t = 0f;
		if (makeSmaller) {
			while (t < rescaleTime) {
				float scale = (1 + (rescalingPercent - 1) * Animf.EasyOut(t/rescaleTime, easyRescalingPow)) * CommonInfo.Instance.FitHubbleSize;
				node.hubble.transform.localScale = Vector3.one * scale;
				yield return null;
				t += Time.deltaTime;
			}
			node.hubble.transform.localScale = Vector3.one * CommonInfo.Instance.FitHubbleSize * rescalingPercent;
		} else {
			while (t < rescaleTime) {
				float scale = (rescalingPercent + (1 - rescalingPercent) * Animf.EasyOut(t/rescaleTime, easyRescalingPow)) * CommonInfo.Instance.FitHubbleSize;
				node.hubble.transform.localScale = Vector3.one * scale;
				yield return null;
				t += Time.deltaTime;
			}
			node.hubble.transform.localScale = Vector3.one * CommonInfo.Instance.FitHubbleSize;
		}
		isAnimating = false;
	}


	public void StartRotating (Node mainNode, List<Node> surroundingNodes) {
		rotateIEnumerator = IRotate (mainNode, surroundingNodes);
		StartCoroutine (rotateIEnumerator);
	}

	public void StopRotating () {
		if (rotateIEnumerator != null) {
			StopCoroutine (rotateIEnumerator);
		}
	}


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
	

	void Rotate (Node mainNode, List<Node> surroundingNodes, float angle) {
		currentAngle = (((int)angle) / 60) * 60f + Animf.EasyInOut((angle % 60) / 60 , easyRotationPow) * 60;
		mainNode.hubble.transform.rotation = Quaternion.Euler (new Vector3(0,0, - currentAngle));
		foreach (Node node in surroundingNodes) {
			node.hubble.content.transform.rotation = Quaternion.identity;
		}
		mainNode.hubble.content.transform.rotation = Quaternion.identity;
	}



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

		mainNode.hubble.transform.localScale = Vector3.one * CommonInfo.Instance.FitHubbleSize;

		foreach (Node node in surroundingNodes) {
			if (node != mainNode && node.hubble.transform != null) {
				node.hubble.transform.SetParent(this.transform);
				node.hubble.transform.rotation = Quaternion.identity;
				node.hubble.content.transform.rotation = Quaternion.identity;
				node.hubble.transform.localScale = Vector3.one * CommonInfo.Instance.FitHubbleSize;
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



	public void DeleteGroup (List<Node> nodes) {
		scoreStatus.SetPersentage (HubblesManager.Instance.totalScore, LevelConfigHandler.CurrentConfig.threeStarsScore);
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
 
	public void WinGame () {
		StartCoroutine (IWinGame ());
	}

	IEnumerator IWinGame () {
		while (HubblesManager.Instance.lives > 0) {
			
			HubblesManager.Instance.lives--;
			livesText.text = HubblesManager.Instance.lives.ToString ();

			HubblesManager.Instance.totalScore += 50;
			scoreText.text = HubblesManager.Instance.totalScore.ToString ();
			scoreStatus.SetPersentage (HubblesManager.Instance.totalScore, LevelConfigHandler.CurrentConfig.threeStarsScore);

			yield return new WaitForSeconds (.1f);
		}
		GameManager.Instance.Win ();
		yield return null;
	}

	void FindObjectsAndNullReferences () {

		if (pointsTextsObj != null) {
			foreach (GameObject uip in pointsTextsObj) {
				if (uip != null)
					Destroy (uip);
			}
		}

		pointsTextsObj = new GameObject[CommonInfo.Instance.lightColors.Length];
		for (int i = 0; i < pointsTextsObj.Length; i++) {
			pointsTextsObj [i] = Instantiate (pointsPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			pointsTextsObj [i].transform.SetParent (pointsHolder.transform, false);
			pointsTextsObj [i].transform.localScale = Vector3.zero;
			Image[] images = pointsTextsObj [i].GetComponentsInChildren <Image> ();
			images [0].color = CommonInfo.Instance.lightColors [i];
			images [1].color = CommonInfo.Instance.usualColors [i];
			pointsTextsObj [i].GetComponentInChildren <Text> ().color = CommonInfo.Instance.darkColors [i];
		}

		highLightingCoroutines = new List<IEnumerator>();

		panelIsUnHighLighting = false;
		pickedIconsAreUnHighLighting = false;

		offsetAngle = 0f;

		BGColor = Camera.main.backgroundColor;
		BGColor.a = 0f;

		heartObj.transform.localScale = Vector3.zero;
		multiplayerObj.transform.localScale = Vector3.zero;

		float firstStarMarkPos = ((float)LevelConfigHandler.CurrentConfig.oneStarScore) / ((float)LevelConfigHandler.CurrentConfig.threeStarsScore);
		float secondStarMarkPos = ((float)LevelConfigHandler.CurrentConfig.twoStarsScore) / ((float)LevelConfigHandler.CurrentConfig.threeStarsScore);

		firstStarMark.anchorMin = new Vector2 (firstStarMarkPos - starMarkWidth / 2f, 0f);
		firstStarMark.anchorMax = new Vector2 (firstStarMarkPos + starMarkWidth / 2f, 1f);
		firstStarMark.offsetMin = firstStarMark.offsetMin = Vector2.zero;

		secondStarMark.anchorMin = new Vector2 (secondStarMarkPos - starMarkWidth / 2f, 0f);
		secondStarMark.anchorMax = new Vector2 (secondStarMarkPos + starMarkWidth / 2f, 1f);
		secondStarMark.offsetMin = secondStarMark.offsetMin = Vector2.zero;

		scoreStatus.SetPersentage (0f);
		scoreText.text = 0.ToString ();
	}


}
