using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Contains all metadata and link to hubble for certain node on a map
/// </summary>
[System.Serializable]
public class Node {

	public int color;
	public int type;
	public int points;
	public bool isActive;
	public Hubble hubble;

	public Node (int color, int type, int points, Hubble hubble) {
		this.color = color;
		this.type = type;
		this.points = points;
		this.isActive = false;
		this.hubble = hubble;
	}

	public Node (int color, int type, Hubble hubble) {
		this.color = color;
		this.type = type;
		this.points = 1;
		this.isActive = false;
		this.hubble = hubble;
	}

	public Node () {}

	/// <summary>
	/// Finds new params for deleted hubble
	/// </summary>
	public void Reestablish () {
		// Choose new color, that is not equal to previous
		int[] colorsToApply = new int [LevelConfigHandler.CurrentConfig.colors.Length - 1];

		int i = 0;
		int c = 1;

		while (i < colorsToApply.Length) {
			if (color != c) {
				colorsToApply [i] = c;
				i++;
			}
			c++;
		}

		color = colorsToApply [Random.Range (0, colorsToApply.Length)];

		// Choose new type
		int newType;
		int chance = Random.Range (0, 1000);
		if (chance < LevelConfigHandler.CurrentConfig.heartChance)
			newType = 1;
		else if (chance < LevelConfigHandler.CurrentConfig.multiplayerChance + LevelConfigHandler.CurrentConfig.heartChance)
			newType = 2;
		else
			newType = -1;

		// Sets new points
		points++;

		hubble.Set (color - 1, newType, type, points);
		type = newType;
		hubble.Appear ();
		hubble.UnHighlight ();
	}

	/// <summary>
	/// Set node activity 
	/// </summary>
	/// <param name="active">is active?</param>
	public void SetActive (bool active) {
		if (type == 5 && !active) {
			color = 0;
		}
		isActive = active;
	}

	/// <summary>
	/// Change node params without deleting
	/// </summary>
	/// <param name="type">new type</param>
	/// <param name="color">new color</param>
	/// <param name="pointsDif">added points</param>
	public void ChangeParams (int type, int color, int pointsDif) {
		MapGenerator.Instance.thisColorNodes [this.color - 1].Remove (this as Node);
		this.color = color;
		this.points += pointsDif;
		hubble.Change ();
		hubble.Set (color - 1, type, this.type, this.points);
		this.type = type;
		MapGenerator.Instance.thisColorNodes [this.color - 1].Add (this as Node);
	}
		
	// ========================

	// // Types

	//-2 -- none
	//-1 -- usual
	// 0 -- empty
	// 1 -- health
	// 2 -- multiplayer
	// 3 -- ice
	// 4 -- gear
	// 5 -- versicolor

	// // Colors

	// 0 -- none
	// 1... -- other colors

	// Example: 
	// 123 is 1, 2 or 3 color

	// ========================
}
	
/// <summary>
/// Handles random map generation 
/// </summary>
public class Map : ScriptableObject {

	/// <summary>
	/// Array of all nodes
	/// </summary>
	public static Node[,] nodeMap;

	/// <summary>
	/// Array of lists of nodes for certain colors
	/// </summary>
	public static List <Node>[] oneColorNodes;

	/// <summary>
	/// Creates new node
	/// </summary>
	/// <param name="color">color for new node</param>
	/// <param name="type">type for new node</param>
	/// <param name="coord">coord for new node</param>
	/// <returns>created node</returns>
	public static Node SetNode (int color, int type, Coord coord) {
		Hubble hubble = null;
		if (type != 0) {
			hubble = Instantiate (CommonInfo.Instance.hubblePrefab, Coord.Vector2FromCoord (coord), Quaternion.identity) as Hubble;
			hubble.transform.localScale *= CommonInfo.Instance.FitHubbleSize;
		}
		
		return SetNode (color, type, 1, hubble, coord);
	}

	/// <summary>
	/// Creates new node with existing hubble
	/// </summary>
	/// <param name="color">color for new node</param>
	/// <param name="type">type for new node</param>
	/// <param name="points">points for new node</param>
	/// <param name="hubble">hubble for new nodw</param>
	/// <param name="coord">coord for new node</param>
	/// <returns>created node</returns>
	public static Node SetNode (int color, int type, int points, Hubble hubble, Coord coord) {

		Node resault = new Node (color, type, points, hubble);

		if (type == 0) { // Empty
			if (hubble != null)
				Destroy (resault.hubble.gameObject);
		} else if (type == 3 || type == 4 || type == 5) { // Gear, Ice or Versicolor
			Debug.LogError ("Gear, Ice and Versicolor functionality are not ready");
		} else if (color != 0) {
			int tempColor = color;
			int index = 0;
			while (tempColor > 0) {
				tempColor /= 10;
				index++;
			}

			int[] colorsToApply = new int[index];
			int randomIndex = Random.Range (0, index);

			tempColor = color;
			index = 0;

			while (tempColor > 0) {
				colorsToApply [index] = tempColor % 10;
				tempColor /= 10;
				index++;
			}

			resault.color = colorsToApply [randomIndex];

			if (type == -1) {
				int chance = Random.Range (0, 1000);
				if (chance < LevelConfigHandler.CurrentConfig.heartChance)
					resault.type = 1;
				else if (chance < LevelConfigHandler.CurrentConfig.multiplayerChance + LevelConfigHandler.CurrentConfig.heartChance)
					type = 2;
			} else if (type == -2) {
				resault.type = -1;
			}
				
			hubble.Set (resault.color - 1, resault.type, 0, resault.points);
		}

		nodeMap [coord.x, coord.y] = resault;
		return resault;
	}

	/// <summary>
	/// Bool delegate that check if node is similar to current
	/// </summary>
	/// <param name="x">x coord of node to compare</param>
	/// <param name="y">y coord of node to compare</param>
	/// <param name="currentNode">node to be compared</param>
	public delegate bool LookForNearNodes (int x, int y, Node currentNode);

	/// <summary>
	/// Returns list of coords, that are near and similar to current
	/// </summary>
	/// <param name="coord">coord of current node</param>
	/// <param name="Suits">boolean delegate to compare to nodes on similarity</param>
	/// <returns>list of similar neighbours</returns>
	public static List <Coord> NearCoords (Coord coord, LookForNearNodes Suits) {
		int x = coord.x;
		int y = coord.y;
		Node currentNode = nodeMap [x, y];
		List <Coord> nearCoords = new List<Coord> ();

		if (Suits (x, y, currentNode))
			nearCoords.Add (new Coord(x, y));

		if (Suits (x + 1, y, currentNode))
			nearCoords.Add (new Coord(x + 1, y));
		if (Suits (x - 1, y, currentNode))
			nearCoords.Add (new Coord(x - 1, y));
		if (Suits (x, y + 1, currentNode))
			nearCoords.Add (new Coord(x, y + 1));
		if (Suits (x, y - 1, currentNode))
			nearCoords.Add (new Coord(x, y - 1));

		if (y % 2 == 1) {

			if (Suits (x - 1, y + 1, currentNode))
				nearCoords.Add (new Coord(x - 1, y + 1));
			if (Suits (x - 1, y - 1, currentNode))
				nearCoords.Add (new Coord(x - 1, y - 1));

		} else {

			if (Suits (x + 1, y + 1, currentNode))
				nearCoords.Add (new Coord(x + 1, y + 1));
			if (Suits (x + 1, y - 1, currentNode))
				nearCoords.Add (new Coord(x + 1, y - 1));

		}

		return nearCoords;
	}

	/// <summary>
	/// Returns list of coords, that are near and similar to current
	/// </summary>
	/// <param name="coord">coord of current node</param>
	/// <param name="canRotate">is current node able to be rotated?</param>
	/// <param name="Suits">boolean delegate to compare to nodes on similarity</param>
	/// <returns>list of similar neighbours</returns>
	public static List <Coord> NearCoords (Coord coord, out bool canRotate, LookForNearNodes Suits) {
		List <Coord> nearCoords = NearCoords (coord, Suits);
		canRotate = nearCoords.Count == 6;
		return nearCoords;
	}

	/// <summary>
	/// List of nodes for given list of coords
	/// </summary>
	/// <param name="coords">list of coords</param>
	/// <returns>list of corresponding nodes</returns>
	public static List <Node> NodesFromCoords (List <Coord> coords) {
		List <Node> nodes = new List<Node> ();
		foreach (Coord coord in coords) {
			nodes.Add (nodeMap [coord.x, coord.y]);
		}
		return nodes;
	}

	/// <summary>
	/// Is node on x,y coord can be rotated
	/// </summary>
	/// <param name="x">x coord of node to compare</param>
	/// <param name="y">y coord of node to compare</param>
	/// <param name="currentNode">node to be compared</param>
	/// <returns>is node on x,y coord can be rotated</returns>
	public static bool AreRotable (int x, int y, Node currentNode) {
		if (!Coord.MapContains (new Coord (x, y)))
			return false;
		if (currentNode == nodeMap [x, y])
			return false;
		if (currentNode.type == 0 || currentNode.type == 3)
			return false;
		int t = nodeMap [x, y].type;
		return Coord.MapContains (new Coord (x, y)) && (t == -1 || t == 1 || t == 2 || t == 5);
	}

	/// <summary>
	/// Is node on x,y coord has the same color as current
	/// </summary>
	/// <param name="x">x coord of node to compare</param>
	/// <param name="y">y coord of node to compare</param>
	/// <param name="currentNode">node to be compared</param>
	/// <returns>is node on x,y coord has the same color as current</returns>
	public static bool AreTheSameСolor (int x, int y, Node currentNode) {
		if (!Coord.MapContains (new Coord (x, y)))
			return false;
		if (currentNode == nodeMap [x, y])
			return false;
		if (currentNode.type == 0 || currentNode.type == 3 || currentNode.type == 4)
			return false;
		if (currentNode.type == 5 && currentNode.color == 0)
			return false;
		if (nodeMap [x, y].isActive)
			return false;
		int t = nodeMap [x, y].type;
		if (t == 0 || t == 4)
			return false;
		if (t == 5) {
			nodeMap [x, y].color = currentNode.color;
			return true;
		}
		return nodeMap [x, y].color == currentNode.color;
	}

	/// <summary>
	/// Is node on x,y coord can be painted
	/// </summary>
	/// <param name="x">x coord of node to compare</param>
	/// <param name="y">y coord of node to compare</param>
	/// <param name="currentNode">node to be compared</param>
	/// <returns>is node on x,y coord can be painted</returns>
	public static bool Paintable (int x, int y, Node currentNode) {
		if (!Coord.MapContains (new Coord (x, y)))
			return false;
		if (currentNode.type == 0 || currentNode.type == 4)
			return false;
		int t = nodeMap [x, y].type;
		if (t == 0 || t == 4 || t == 5) 
			return false;
		return true;
	}

}
