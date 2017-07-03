using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HubblesManager : MonoSingleton <HubblesManager> {


	private Node currentNode;
	private Node previousNode;
	[HideInInspector] public List<Node> oneColorGroup;
	private List<Node> surroundingNodes;

	private bool canRotate;
	[HideInInspector] public bool allAreOneColor;
	private bool turnedPreviously;

	public int lives;
	public int totalScore;

	public UIAimsHolder aimsHolder;

	public Aim[] aims;

	public bool gameEnded;

	public void StartGame () {
		lives = LevelConfigHandler.CurrentConfig.startLives;
		FindObjectsAndNullReferences ();
//		ActivatePaintBonus <PaintBonus> ();
		SubscribeOnTouchEvents ();
	}

	public void ActivatePaintBonus <T> () where T : Bonus <T> {
		TouchManager.Instance.OnTouchEnd.AddListener (delegate {
			PaintBonus.Instance.Apply (TouchManager.Instance.startTouchCoord);
//			T.Instance.Apply (TouchManager.Instance.startTouchCoord);
		});
	}

	public void SubscribeOnTouchEvents () {
		TouchManager.Instance.OnTouchStart.AddListener (delegate {
			FindCurrentNode ();			
			if (!oneColorGroup.Contains (currentNode) && oneColorGroup.Count > 0) {
				AnimationManager.Instance.UnHighLightEveryThing (true);
				oneColorGroup.Clear ();
			}
		});

		TouchManager.Instance.OnTouchSurrounding.AddListener (delegate {
			if (oneColorGroup.Count > 0) {
				AnimationManager.Instance.UnHighLightEveryThing(true);
				oneColorGroup.Clear();
			}
		});

		TouchManager.Instance.OnRotatingStart.AddListener (delegate {
			AnimationManager.Instance.UnHighLightEveryThing(true);																									
			surroundingNodes = Map.NodesFromCoords (Map.NearCoords (TouchManager.Instance.startTouchCoord, out canRotate, Map.AreRotable));														
			if (!canRotate) {
				//animManager.RotatingIsUnabled ();																														// Don't Forget This!
			} else {
				foreach (Node node in surroundingNodes) {
					node.hubble.transform.SetParent (currentNode.hubble.transform);
				}
				AnimationManager.Instance.Rescale (currentNode, true);
				AnimationManager.Instance.StartRotating (currentNode, surroundingNodes);
			}
			if (oneColorGroup.Count > 0) {
				oneColorGroup.Clear();
			}
		});

		TouchManager.Instance.OnRotatingEnd.AddListener (delegate {
			if (canRotate) {
				int turns = (Mathf.RoundToInt (TouchManager.Instance.angle / 60f) % 360 + 360) % 6;
				AnimationManager.Instance.StopRotating ();
				AnimationManager.Instance.PullToMap (currentNode, surroundingNodes, turns, TouchManager.Instance.angle);
				if (turns != 0) {
					if (previousNode!=null) {
						if (currentNode != previousNode || !turnedPreviously) {
							lives -= 1;
						}
					} else {
						lives -= 1;
						if (lives < 0)
							lives = 0;
					}
					turnedPreviously = true;
					previousNode = currentNode;
				} else {
					turnedPreviously = false;
				}
				AnimationManager.Instance.livesText.text = lives.ToString();

				CheckLivesAndAims ();
			}
		});

		TouchManager.Instance.OnTouchEnd.AddListener (delegate {
			if (!TouchManager.Instance.wasRotating) {
				AnimationManager.Instance.UnHighLightEveryThing(false);
				if (oneColorGroup.Count > 0) {
					DeleteGroup (currentNode.color);
					AnimationManager.Instance.DeleteGroup (oneColorGroup);
					previousNode = null;
					turnedPreviously = false;
					lives -= 2;
					if (lives < 0)
						lives = 0;
					AnimationManager.Instance.livesText.text = lives.ToString();

					CheckLivesAndAims ();
				} else {
					FindGroup ();
					AnimationManager.Instance.HighLightHubbles (oneColorGroup);
				}
			}
		});
	}

	void CheckLivesAndAims () {
		bool livesAreEmpty = lives == 0;
		bool allAimsAreDone = true;
		foreach (Aim aim in aims) {
			if (!aim.isDone ()) {
				allAimsAreDone = false;
				break;
			}
		}

		if (allAimsAreDone) {
			gameEnded = true;
			AnimationManager.Instance.WinGame ();
		} else if (livesAreEmpty) {
			gameEnded = true;
			GameManager.Instance.Lose ();
		}
	}

	public void ContinueWithNewSteps (int steps) {
		lives += steps;
		gameEnded = false;
		AnimationManager.Instance.livesText.text = lives.ToString();
		AnimationManager.Instance.isAnimating = false;
	}

	void FindCurrentNode () {
		currentNode = Map.nodeMap[TouchManager.Instance.startTouchCoord.x, TouchManager.Instance.startTouchCoord.y];
	} 

	void DeleteGroup (int color) {
		int points = 0;
		int multiplayer = 1;
		int health = 0; 

		foreach (Node node in oneColorGroup) {
			if (node.type == -1)
				points += node.points;
			else if (node.type == 1)
				health += 4;
			else if (node.type == 2)
				multiplayer *= 2;
		}

		if (allAreOneColor) {
			multiplayer *= 2;
		}
			
		Aim currentAim = null;
		foreach (Aim aim in aims) {
			if (aim.color == color) {
				currentAim = aim;
				break;
			}
		}

		if (currentAim != null) {
			currentAim.count -= points * multiplayer;
			aimsHolder.SetCParticularAimCount (color, currentAim.count);
		}

		totalScore += points * multiplayer;
		lives += health * multiplayer;
	}

	void FindGroup () {
		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (TouchManager.Instance.startTouchCoord);
		oneColorGroup.Add (currentNode);
		Coord particularCoord;
		Map.nodeMap [TouchManager.Instance.startTouchCoord.x, TouchManager.Instance.startTouchCoord.y].SetActive (true);
		
		while (queue.Count != 0) {
			particularCoord = queue.Dequeue();
			foreach (Coord coord in Map.NearCoords(particularCoord, Map.AreTheSamecolor)) {
				Map.nodeMap [coord.x, coord.y].SetActive (true);
				oneColorGroup.Add (Map.nodeMap[coord.x, coord.y]);
				queue.Enqueue (coord);
			}
		}
			
		if (oneColorGroup.Count == MapGenerator.Instance.thisColorNodes[currentNode.color - 1].Count) {
			allAreOneColor = true;
		} else {
			allAreOneColor = false;
		}
		
	}

	public void Turn (int turns) {

		Node temporaryNode;

		int x = TouchManager.Instance.startTouchCoord.x;
		int y = TouchManager.Instance.startTouchCoord.y;

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
		
	void FindObjectsAndNullReferences () {
		oneColorGroup = new List<Node>();
		surroundingNodes = new List<Node>();
		canRotate = false;
		allAreOneColor = false;
		turnedPreviously = false;
		gameEnded = false;

		totalScore = 0;

		aims = new Aim[LevelConfigHandler.CurrentConfig.aims.Length];

		for (int i = 0; i < aims.Length; i++) {
			aims [i] = LevelConfigHandler.CurrentConfig.aims [i].Copy ();
		}

		aimsHolder.GenerateAims ();
		AnimationManager.Instance.livesText.text = lives.ToString();
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
