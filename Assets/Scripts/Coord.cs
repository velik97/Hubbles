using UnityEngine;

/// <summary>
/// Coords for hexagonal field
/// </summary>
[System.Serializable]
public class Coord {

	private static Coord mapSize;

	public int x;
	public int y;

	private static float hexStepX;
	private static float hexStepY;
	private static float offsetX;
	private static float offsetY;

	public static Coord MapSize {
		get {
			return mapSize;
		}
		set {
			offsetX = -(Coord.Step.x * (value.x - 1) / 2f);
			offsetY = -(Coord.Step.y * (value.y - 1) / 2f);
			mapSize = value;
		}
	}

	public static Vector2 Step {
		get {
			if (hexStepX == 0f)
				hexStepX = 1f;
			if (hexStepY == 0f)
				hexStepY = Mathf.Sqrt (3f) / 2f;

			return new Vector2 (hexStepX, hexStepY);

		}
	}

	public Coord () {
		x = 0;
		y = 0;
	}

	public Coord (int x, int y) {
		this.x = x;
		this.y = y;
	}

	public static Coord Bad {
		get {
			return new Coord (-1, -1);
		}
	}

	public static Coord Zero {
		get {
			return new Coord (0, 0);
		}
	}

	public static Coord Top {
		get {
			if (mapSize == null) {
				Debug.LogError ("static field 'mapSize' of 'HexCoord' class wasn't set!");
				return Bad;
			}

			return new Coord (mapSize.x - 1, mapSize.y - 1);
		}
	}

	public static bool MapContains (Coord coord) {
		if (coord.y >= MapSize.y || coord.y < 0 || coord.x < 0)
			return false;

		if (coord.y % 2 == 0)
			return coord.x < MapSize.x - 1;
		return coord.x < MapSize.x;
	}

	public static Coord CoordFromVector2 (Vector2 v) {
		if (mapSize == null) {
			Debug.LogError ("static field 'mapSize' of 'HexCoord' class wasn't set!");
			return Bad;
		}

		int closestX;
		int closestY;

		closestY = Mathf.RoundToInt ((v.y - offsetY) / Step.y);
		closestX = Mathf.RoundToInt ((v.x - offsetX + ((closestY % 2 == 0) ? (-Step.x / 2f) : (0f))) / Step.x);

		return new Coord (closestX, closestY);
	}

	public static Vector2 Vector2FromCoord (Coord coord) {
		Vector2 resault = new Vector2 (offsetX + coord.x * Step.x, offsetY + coord.y * Step.y);
		if (coord.y % 2 == 0)
			resault += Vector2.right * Coord.Step.x * .5f;
		return resault;
	}

	public static Coord LECoordFromVector2 (Vector2 v) {
		if (mapSize == null) {
			Debug.LogError ("static field 'mapSize' of 'HexCoord' class wasn't set!");
			return Bad;
		}

		int closestX;
		int closestY;

		closestY = Mathf.RoundToInt ((v.y) / Step.y);
		closestX = Mathf.RoundToInt ((v.x + ((closestY % 2 == 0) ? (-Step.x / 2f) : (0f))) / Step.x);

		return new Coord (closestX, closestY);
	}

	public static bool operator == (Coord a, Coord b) {
		System.Object aObj = a as System.Object;
		System.Object bObj = b as System.Object;

		if (aObj == null ^ bObj == null) {
			return false;
		} else if (aObj == null && bObj == null) {
			return true;
		}

		return (a.x == b.x && a.y == b.y);
	}

	public static bool operator != (Coord a, Coord b) {
		return !(a == b);
	}

	public override string ToString () {
		if ((Coord)this == Bad)
			return "HexCoord.Bad";
		return "x: " + x + " ,y: " + y;
	}
}
