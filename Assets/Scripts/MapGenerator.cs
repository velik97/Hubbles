using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapGenerator : MonoSingleton <MapGenerator> {

	[HideInInspector] public int width;
	[HideInInspector] public int height;

	private float multiplayerChance;
	private float heartChance;

	public FieldResizer fRes;

	private int colorsCount;

//	[HideInInspector] public int[] thisColorNodesCount;
	[HideInInspector] public List <Node>[] thisColorNodes;

	[HideInInspector] public float offsetX;
	[HideInInspector] public float offsetY;

	public void StartGame () {
		AssembleConfig (LevelConfigHandler.CurrentConfig);
		GenerateMap ();

		Camera.main.ResizeInGame ();
		StartCoroutine(GenerateMap ());
		fRes.Resize (width, height);
		Coord.MapSize = new Coord (width, height);
	}
		
	void AssembleConfig (LevelConfig config) {
		CommonInfo.Instance.UpdateColors ();
		Coord.MapSize = new Coord (config.Width, config.Height);
		width = config.width;
		height = config.height;
		colorsCount = config.colors.Length;
	}

//	void Update () {
//		for (int i = 0; i < width; i ++) {
//			for (int j = 0; j < height; j ++) {
//				if (coords[i,j] != new HexCoord (-1, -1) && hubbles[i,j] != null) {
//					hubbles [i, j].content.GetComponentInChildren <Text> ().text = hubbles [i, j].coord.x + "," + hubbles [i, j].coord.y;
//				}
//			}
//		}
//	}


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
					Node newNode = Map.SetNode (LevelConfigHandler.CurrentConfig.colorMap [i + j * width], LevelConfigHandler.CurrentConfig.typeMap [i + j * width], newCoord);
					if (newNode.type != 0) {
						thisColorNodes [newNode.color - 1].Add (newNode);
						yield return new WaitForSeconds (.01f);
					}
				}
			}
		}

		AnimationManager.Instance.isAnimating = false;
		yield return null;
		HubblesManager.Instance.oneColorGroup.Clear ();
	}

	public IEnumerator ReestablishMap (List<Node> nodesToReestablish) {
		int color = nodesToReestablish[0].color;

		foreach (Node node in nodesToReestablish) {
			thisColorNodes [color - 1].Remove (node);
		}

		AnimationManager.Instance.isAnimating = true;

		foreach (Node node in nodesToReestablish) {
			node.Reestablish ();
			yield return new WaitForSeconds (0.01f);
			thisColorNodes [node.color - 1].Add (node);
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
