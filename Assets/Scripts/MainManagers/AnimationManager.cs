using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

/// <summary>
/// Handles animation for hubbles and UI.
/// </summary>
public class AnimationManager : MonoSingleton <AnimationManager> {

	/// <summary>
	/// Time between highlights of hubbles
	/// </summary>
	[Header("HighLighting")]
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
	public void UnHighLightEverything (bool unhighlightHubbles) {
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
		HighlightPanel.Instance.Unhighlight();
		
		InGameUIManager.Instance.SetMultiplier(1);
		InGameUIManager.Instance.SetExtraPopLives(0);
		InGameUIManager.Instance.SetExtraRotLives(0);
	}

	/// <summary>
	/// Highlight hubbles group
	/// </summary>
	/// <param name="nodes">nodes to be highlighted</param>
	public void HighLightHubbles (List<Node> nodes) {
		highLightingCoroutines.Add(StartCoroutine(IHighLightHubbles(nodes)));
	}
	
	IEnumerator IHighLightHubbles (List<Node> nodes) {
		float pitch = Mathf.Exp(startPitch);
		int popHeal = 0;
		int rotationHeal = 0;
		int multiplier = 1;

		foreach (Node node in nodes) {
			pitch += pitchStep;
			node.hubble.Highlight (pitch);

			yield return new WaitForSeconds (deltaHighLightTime);
			
			switch (node.type)
			{
				case HubbleType.Usual:
					break;
				
				case HubbleType.PopLive:
					popHeal += 1;
					InGameUIManager.Instance.SetExtraPopLives(popHeal*multiplier);
					break;
				
				case HubbleType.RotationLive:
					rotationHeal += 1;
					InGameUIManager.Instance.SetExtraRotLives(rotationHeal*multiplier);
					break;
				
				case HubbleType.Multiplier:
					multiplier *= 2;
					InGameUIManager.Instance.SetMultiplier(multiplier);
					InGameUIManager.Instance.SetExtraPopLives(popHeal*multiplier);
					InGameUIManager.Instance.SetExtraRotLives(rotationHeal*multiplier);
					break;
			}

		}
		if (HubblesManager.Instance.allAreOneColor) {
			HighlightPanel.Instance.Highlight (HubblesAppearanceInfo.Instance.UsualColors[nodes[0].color]);
			multiplier *= 2;
			InGameUIManager.Instance.SetMultiplier(multiplier);
			InGameUIManager.Instance.SetExtraPopLives(popHeal*multiplier);
			InGameUIManager.Instance.SetExtraRotLives(rotationHeal*multiplier);
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
				float scale = (1 + (rescalingPercent - 1) * AnimationF.EasyOut(t/pullTime, easyRescalingPow)) * HubblesAppearanceInfo.Instance.FitHubbleSize;
				node.hubble.transform.localScale = Vector3.one * scale;
				yield return null;
				t += Time.deltaTime;
			}
			node.hubble.transform.localScale = Vector3.one * HubblesAppearanceInfo.Instance.FitHubbleSize * rescalingPercent;
		} else {
			while (t < pullTime) {
				float scale = (rescalingPercent + (1 - rescalingPercent) * AnimationF.EasyOut(t/pullTime, easyRescalingPow)) * HubblesAppearanceInfo.Instance.FitHubbleSize;
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
	/// <param name="getAngle">function that returns current angle</param>
	public void StartRotating (Node mainNode, List<Node> surroundingNodes, Func<float> getAngle) {
		rotateCoroutines.Add(StartCoroutine (IRotate (mainNode, surroundingNodes, getAngle)));
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
	/// <param name="getAngle">function that returns current angle</param>
	IEnumerator IRotate (Node mainNode, List<Node> surroundingNodes, Func<float> getAngle) {
		while (true) {
			Rotate (mainNode, surroundingNodes, getAngle());
			if (Mathf.Abs (offsetAngle - getAngle()) > 30f) {
				if (getAngle() > offsetAngle)
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
	public void Rotate (Node mainNode, List<Node> surroundingNodes, float angle) {
		currentAngle = (((int)angle) / 60) * 60f + AnimationF.EasyInOut((angle % 60) / 60 , easyRotationPow) * 60;
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
		InGameUIManager.Instance.SetRotLives(HubblesManager.Instance.rotLives);
	}


	/// <summary>
	/// Delete group of nodes.
	/// </summary>
	/// <param name="nodes">nodes to be deleted</param>
	public void DeleteGroup (List<Node> nodes) {
		InGameUIManager.Instance.SetScore(HubblesManager.Instance.totalScore);
		InGameUIManager.Instance.SetLevel(HubblesManager.Instance.level);
		StartCoroutine (IDeleteGroup(nodes));
	}

	IEnumerator IDeleteGroup (List<Node> nodes) {
		isAnimating = true;
		InGameUIManager.Instance.SetPopLives(HubblesManager.Instance.popLives);
		InGameUIManager.Instance.SetRotLives(HubblesManager.Instance.rotLives);
		foreach (Node node in nodes) {
			node.SetActive(false);
			node.hubble.DisAppear();
		}
		yield return new WaitForSeconds(deleteTime);
		SoundManager.Instance.Play(bigPop);
		StartCoroutine(MapGenerator.Instance.ReestablishMap(nodes));
	}

	void FindObjectsAndNullReferences ()
	{
		highLightingCoroutines = new List<Coroutine>();
		rotateCoroutines = new List<Coroutine>(); 

		offsetAngle = 0f;
	}

	private static string FormatNumber(int number)
	{
		var f = new NumberFormatInfo {NumberGroupSeparator = " "};
		return number.ToString("#,0", f);
	} 
}
