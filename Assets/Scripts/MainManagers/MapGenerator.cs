using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Randomly generates map
/// </summary>
public class MapGenerator : MonoSingleton <MapGenerator> {

	/// <summary>
	/// Map width
	/// </summary>
	[HideInInspector] public int width;
	/// <summary>
	/// Map height
	/// </summary>
	[HideInInspector] public int height;

	/// <summary>
	/// Probability of falling multiplier node
	/// </summary>
	private float multiplierChance;
	/// <summary>
	/// Probability of falling heart node. Need change
	/// </summary>
	private float heartChance;

	public FieldResizer fRes;

	/// <summary>
	/// How many colors will be on the map
	/// </summary>
	private int colorsCount;

	/// <summary>
	/// Array of lists of nodes for certain colors
	/// </summary>
	[HideInInspector] public List <Node>[] thisColorNodes;
	
	public void StartGame () {
		AssembleConfig ();

		Camera.main.ResizeInGame ();
		StartCoroutine(GenerateMap ());
		fRes.Resize (width, height);
		Coord.MapSize = new Coord (width, height);
	}
		
	/// <summary>
	/// Apply config params to map
	/// </summary>
	void AssembleConfig () {
		HubblesAppearanceInfo.Instance.UpdateColors ();
		Coord.MapSize = new Coord (LevelConfig.Width, LevelConfig.Height);
		width = LevelConfig.Width;
		height = LevelConfig.Height;
		colorsCount = LevelConfig.Colors.Length;
	}

	/// <summary>
	/// Generates new map with some duration 
	/// </summary>
	IEnumerator GenerateMap () {
		Map.nodeMap = new Node[Coord.MapSize.x, Coord.MapSize.y];
		thisColorNodes = new List<Node>[colorsCount];

		for (int i = 0; i < thisColorNodes.Length; i++) {
			thisColorNodes[i] = new List<Node>();
		}
	
		AnimationManager.Instance.isAnimating = true;
		Hubble newHubble;
		for (int i = 0; i < width; i ++) {
			for (int j = 0; j < height; j ++) {
				Coord newCoord = new Coord (i, j);
				if (Coord.MapContains (newCoord)) {
					Node newNode = Map.SetNode (RandomGenerator.RandomColor(), RandomGenerator.RandomType(), newCoord);
					if (newNode.type != 0) {
						thisColorNodes [newNode.color].Add (newNode);
						yield return new WaitForSeconds (.01f);
					}
				}
			}
		}

		AnimationManager.Instance.isAnimating = false;
		yield return null;
		HubblesManager.Instance.oneColorGroup.Clear ();
	}

	/// <summary>
	/// Reestablish deleted nodes
	/// </summary>
	/// <param name="nodesToReestablish">Nodes deleted on previous step, that are needed to be reestablished</param>
	public IEnumerator ReestablishMap (List<Node> nodesToReestablish) {
		int color = nodesToReestablish[0].color;

		foreach (Node node in nodesToReestablish) {
			thisColorNodes [color].Remove (node);
		}

		AnimationManager.Instance.isAnimating = true;

		foreach (Node node in nodesToReestablish) {
			node.Reestablish ();
			yield return new WaitForSeconds (0.01f);
			thisColorNodes [node.color].Add (node);
		}
		if (!HubblesManager.Instance.gameEnded)
			AnimationManager.Instance.isAnimating = false;
		HubblesManager.Instance.oneColorGroup.Clear ();
	
	}


//	public void ClearMap () {
//		if (hubbleMap != null) {
//			foreach (Hubble hubble in hubbleMap) {
//				if (hubble != null) {
//					Destroy (hubble.gameObject);
//				}
//			}
//		}
//	}


	



}
