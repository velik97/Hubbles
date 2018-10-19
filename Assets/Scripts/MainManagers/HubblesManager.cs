using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Handles hubbles choose, rotation, deletion and reestablishing  
/// </summary>
public class HubblesManager : MonoSingleton <HubblesManager> {

	/// <summary>
	/// Node, chosen on current step
	/// </summary>
	private Node currentNode;
	/// <summary>
	/// Node, chosen on previous step
	/// </summary>
	private Node previousNode;
	/// <summary>
	/// List of nodes, chosen on previous step with one color (if it was chosen)
	/// </summary>
	[HideInInspector] public List<Node> oneColorGroup;
	/// <summary>
	/// List of surrounding nodes
	/// </summary>
	private List<Node> surroundingNodes;

	/// <summary>
	/// Is chosen for rotation node actually could be rotated 
	/// </summary>
	private bool canRotate;
	
	/// <summary>
	/// Does chosen color group contains all hubbles of such color
	/// </summary>
	[HideInInspector] public bool allAreOneColor;
	/// <summary>
	/// Was on previous step
	/// </summary>
	private bool turnedPreviously;

	/// <summary>
	/// Remained popLives
	/// </summary>
	[HideInInspector] public int popLives;
	/// <summary>
	/// Remained rotLives
	/// </summary>
	[HideInInspector] public int rotLives;
	/// <summary>
	/// Total achieved score in this session
	/// </summary>
	[HideInInspector] public int totalScore;
	/// <summary>
	/// Total achieved score in this session
	/// <summary>
	/// Level in the game
	/// </summary>
	[HideInInspector] public int level;

	/// <summary>
	/// Is game ended
	/// </summary>
	public bool gameEnded;

	public bool debugCoords;

	[HideInInspector] public UnityEvent onHighlighted;
	[HideInInspector] public UnityEvent onPoped;
	[HideInInspector] public IntEvent onRotated = new IntEvent();

	/// <summary>
	/// Coord on start of rotating
	/// </summary>
	private Coord startRotatingCoord;

	private int totalPopsPerLevel;
	private int totalOneColorGroupPopPerLevel;
	private int totalRotsPerLevel;

	public void StartGame () {
		popLives = LevelConfig.StartPopLives;
		rotLives = LevelConfig.StartRotationLives;
		FindObjectsAndNullReferences ();
		SubscribeOnTouchEvents ();

		totalPopsPerLevel = 0;
		totalRotsPerLevel = 0;
		totalOneColorGroupPopPerLevel = 0;

		if (debugCoords)
			StartCoroutine(DebugCoords());
	}

	/// <summary>
	/// Subscribe on all touch events.
	/// </summary>
	public void SubscribeOnTouchEvents () {
		TouchManager.Instance.touchState.AddListener(delegate(TouchState touchState)
		{
			switch (touchState)
			{
				case TouchState.StartedTouching:
					StartTouch();
					break;
				case TouchState.EndedTouching:
					EndTouch();
					break;
				case TouchState.StartedRotating:
					StartRotating(() => TouchManager.Instance.angle);
					break;
				case TouchState.EndedRotating:
					EndRotating(() => TouchManager.Instance.angle);
					break;
				case TouchState.StartedTouchingSurrounding:
					TouchSurrounding();
					break;
			}
		});
		
	}

	private void StartTouch()
	{
		FindCurrentNode ();			
		if (!oneColorGroup.Contains (currentNode) && oneColorGroup.Count > 0) {
			AnimationManager.Instance.UnHighLightEveryThing (true);
			oneColorGroup.Clear ();
		}
	}
	
	private void TouchSurrounding()
	{
		if (oneColorGroup.Count > 0) {
			AnimationManager.Instance.UnHighLightEveryThing(true);
			oneColorGroup.Clear();
		}
	}
	
	public void StartRotating(Func<float> getAngle)
	{
		AnimationManager.Instance.UnHighLightEveryThing(true);
		startRotatingCoord = TouchManager.Instance.startTouchCoord;
		surroundingNodes = Map.NodesFromCoords
			(Map.NearCoords (TouchManager.Instance.startTouchCoord, out canRotate, Map.AreRotable));
		bool haveLives = rotLives > 0 ||
		                 (turnedPreviously && previousNode != null && previousNode == currentNode);
		canRotate &= haveLives;
		if (!canRotate) {
			//animManager.RotatingIsUnabled ();		// Don't Forget This!
		} else {
			foreach (Node node in surroundingNodes) {
				node.hubble.transform.SetParent (currentNode.hubble.transform);
			}
			AnimationManager.Instance.Rescale (currentNode, true);
			AnimationManager.Instance.StartRotating (currentNode, surroundingNodes, getAngle);
		}
		if (oneColorGroup.Count > 0) {
			oneColorGroup.Clear();
		}
	}
	
	public void EndRotating(Func<float> getAngle)
	{
		if (canRotate) {
			int turns = (Mathf.RoundToInt (getAngle() / 60f) % 360 + 360) % 6;
			AnimationManager.Instance.StopRotating ();
			AnimationManager.Instance.PullToMap (currentNode, surroundingNodes, turns, getAngle());
			AnimationManager.Instance.rotationLivesText.text = rotLives.ToString();
			if (turns != 0) {
				if (previousNode!=null) {
					if (currentNode != previousNode || !turnedPreviously) {
						rotLives -= 1;
						if (rotLives < 0)
							rotLives = 0;
						totalRotsPerLevel++;
					}
				} else
				{
					
					rotLives -= 1;
					if (rotLives < 0)
						rotLives = 0;
				}
				turnedPreviously = true;
				previousNode = currentNode;
			} else {
				turnedPreviously = false;
			}
			onRotated.Invoke(turns);
		}
	}
	
	private void EndTouch()
	{
		AnimationManager.Instance.UnHighLightEveryThing(false);
		if (oneColorGroup.Count > 0) {
			DeleteGroup (currentNode.color);
			popLives -= 1;
			totalPopsPerLevel++;
			if (popLives < 0)
				popLives = 0;
			MapGenerator.Instance.hubbleGenerator.LoadStepData(totalScore, popLives, rotLives);
			CheckLivesAndAims ();
			AnimationManager.Instance.DeleteGroup (oneColorGroup);
			previousNode = null;
			turnedPreviously = false;
			onPoped.Invoke();
		} else {
			FindGroup ();
			AnimationManager.Instance.HighLightHubbles (oneColorGroup);
			onHighlighted.Invoke();
		}
	}

	/// <summary>
	/// Check if popLives are out
	/// </summary>
	void CheckLivesAndAims () {
		while (totalScore > LevelConfig.LevelScores[level])
		{
			GameManager.Instance.LevelDone(level, popLives, rotLives, totalPopsPerLevel, totalRotsPerLevel,
				totalOneColorGroupPopPerLevel);
			level++;

			totalPopsPerLevel = 0;
			totalRotsPerLevel = 0;
			totalOneColorGroupPopPerLevel = 0;
		}
		if (popLives == 0) {
			gameEnded = true;
			GameManager.Instance.Lose ();
		}
	}

	/// <summary>
	/// Continue game with added steps. Need change
	/// </summary>
	/// <param name="steps">steps for continue</param>
	public void ContinueWithNewSteps (int steps) {
		popLives += steps;
		gameEnded = false;
		AnimationManager.Instance.popLivesText.text = popLives.ToString();		
		AnimationManager.Instance.isAnimating = false;
	}

	/// <summary>
	/// Finds current node
	/// </summary>
	void FindCurrentNode () {
		currentNode = Map.NodeFromCoord(TouchManager.Instance.startTouchCoord);
	} 

	/// <summary>
	/// Deletes one color group
	/// </summary>
	/// <param name="color">color of the group</param>
	void DeleteGroup (int color) {
		int points = 0;
		int multiplier = 1;
		int popHeal = 0; 
		int rotHeal = 0; 

		foreach (Node node in oneColorGroup)
		{
			switch (node.type)
			{
				case HubbleType.Usual:
					points += node.points;
					break;
				case HubbleType.PopLive:
					popHeal += 1;
					break;
				case HubbleType.RotationLive:
					rotHeal += 1;
					break;
				case HubbleType.Multiplier:
					multiplier *= 2;
					break;
			}
		}

		if (allAreOneColor) {
			multiplier *= 2;
			totalOneColorGroupPopPerLevel++;
		}

		totalScore += points * multiplier;
		popLives += popHeal * multiplier;
		rotLives += rotHeal * multiplier;
	}

	/// <summary>
	/// Find one color group for current node
	/// </summary>
	void FindGroup () {
		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (TouchManager.Instance.startTouchCoord);
		oneColorGroup.Add (currentNode);
		Coord particularCoord;
		Map.nodeMap [TouchManager.Instance.startTouchCoord.x, TouchManager.Instance.startTouchCoord.y].SetActive (true);
		
		while (queue.Count != 0) {
			particularCoord = queue.Dequeue();
			foreach (Coord coord in Map.NearCoords(particularCoord, Map.AreTheSameСolor)) {
				Map.nodeMap [coord.x, coord.y].SetActive (true);
				oneColorGroup.Add (Map.nodeMap[coord.x, coord.y]);
				queue.Enqueue (coord);
			}
		}
			
		if (oneColorGroup.Count == MapGenerator.Instance.thisColorNodes[currentNode.color].Count) {
			allAreOneColor = true;
		} else {
			allAreOneColor = false;
		}
		
	}

	/// <summary>
	/// Rotate current hubble and it's neighbours
	/// </summary>
	/// <param name="turns">number of turns</param>
	public void Turn (int turns) {

		Node temporaryNode;

		int x = startRotatingCoord.x;
		int y = startRotatingCoord.y;

		for (int i = 0; i < turns; i ++) {
			if (y % 2 == 1) {
				temporaryNode = Map.nodeMap[x-1, y];

				Map.nodeMap [x - 1, y] = Map.nodeMap [x - 1, y - 1];
				Map.nodeMap [x - 1, y - 1] = Map.nodeMap [x, y - 1];
				Map.nodeMap [x, y - 1] = Map.nodeMap [x + 1, y];
				Map.nodeMap [x + 1, y] = Map.nodeMap [x, y + 1];
				Map.nodeMap [x, y + 1] = Map.nodeMap [x - 1, y + 1];
				Map.nodeMap [x - 1, y + 1] = temporaryNode;

			} else {
				temporaryNode = Map.nodeMap[x-1, y];

				Map.nodeMap [x - 1, y] = Map.nodeMap [x, y - 1];
				Map.nodeMap [x, y - 1] = Map.nodeMap [x + 1, y - 1];
				Map.nodeMap [x + 1, y - 1] = Map.nodeMap [x + 1, y];
				Map.nodeMap [x + 1, y] = Map.nodeMap [x + 1, y + 1];
				Map.nodeMap [x + 1, y + 1] = Map.nodeMap [x, y + 1];
				Map.nodeMap [x, y + 1] = temporaryNode;
			
			}
		}

	}
	
	/// <summary>
	/// Finds all dependencies and initializes all fields 
	/// </summary>
	void FindObjectsAndNullReferences () {
		oneColorGroup = new List<Node>();
		surroundingNodes = new List<Node>();
		canRotate = false;
		allAreOneColor = false;
		turnedPreviously = false;
		gameEnded = false;

		totalScore = 0;
		level = 1;
	}

	private IEnumerator DebugCoords()
	{
		while (true)
		{
			for (var i = 0; i < Map.nodeMap.GetLength(0); i++)
			{
				for (var j = 0; j < Map.nodeMap.GetLength(1); j++)
				{
					if (Map.nodeMap[i, j] != null && Map.nodeMap[i, j].hubble != null && Map.nodeMap[i, j].hubble.text != null)
					{
						Map.nodeMap[i, j].hubble.text.text = i + ", " + j;
					}
				}	
			}
			yield return null;
		}
	}
//	void OnDrawGizmos () {
//		if (currentNode != null) {
//			if (currentNode.hubble != null) {
//				Gizmos.color = Color.red;
//				Gizmos.DrawWireSphere (currentNode.hubble.transform.position, 0.55f);
//			}
//		}
//		if (previousNode != null) {
//			if (previousNode.hubble != null) {
//				Gizmos.color = Color.blue;
//				Gizmos.DrawWireSphere (previousNode.hubble.transform.position, 0.6f);
//			}
//		}
//		if (oneColorGroup != null) {
//			if (oneColorGroup.Count > 0) {
//				foreach (Node node in oneColorGroup) {
//					if (node != null) {
//						Gizmos.color = Color.red;
//						Gizmos.DrawWireSphere (node.hubble.transform.position, 0.5f);
//					}
//				}
//			}
//		}
//	}

}

public class IntEvent : UnityEvent<int> {}
