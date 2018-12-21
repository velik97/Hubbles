using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Coords for hexagonal field
/// </summary>
[System.Serializable]
public class Coord
{
	private static Coord mapSize;

	public int x;
	public int y;

	private static float hexStepX;
	private static float hexStepY;
	private static float offsetX;
	private static float offsetY;

	public static float mapScale = 1f;

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
			hexStepX = mapScale;
			hexStepY = mapScale * Mathf.Sqrt (3f) / 2f;

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

	public Vector2 ToVector2()
	{
		return Vector2FromCoord(this);
	}

	public Coord Neighbour(NeighbourType neighbourType)
	{
		switch (neighbourType)
		{
			case NeighbourType.Right:
				return new Coord(x + 1, y);
			case NeighbourType.TopRight:
				return new Coord(x + (y % 2 == 0 ? 1 : 0), y + 1);
			case NeighbourType.TopLeft:
				return new Coord(x - (y % 2 == 0 ? 0 : 1), y + 1);
			case NeighbourType.Left:
				return new Coord(x - 1, y);
			case NeighbourType.BottomLeft:
				return new Coord(x - (y % 2 == 0 ? 0 : 1), y - 1);
			case NeighbourType.BottomRight:
				return new Coord(x + (y % 2 == 0 ? 1 : 0), y - 1);
			default:
				return Bad;
		}
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

	public static Coord Random()
	{
		int y = UnityEngine.Random.Range(0, MapSize.y);
		int x = UnityEngine.Random.Range(0, y % 2 == 0 ? MapSize.x - 1 : MapSize.x);
		
		return new Coord(x, y);
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

	public static Coord RotationCoord(Vector2 startPoint, Vector2 endPoint, Vector2[] midPoints)
	{
		Coord startCoord = CoordFromVector2(startPoint);
		Vector2[] directions =
			Enumerable.Range(0,6)
				.Select(i => Vector2FromCoord(startCoord.Neighbour((NeighbourType)i)) - Vector2FromCoord(startCoord))
				.ToArray();

		Vector2 dir = endPoint - startPoint;
		NeighbourType dirNeighbourType = (NeighbourType)MyMath.ArgMax(directions, v => Vector2.Dot(dir, v));
		var sign = Mathf.RoundToInt(Mathf.Sign(
			midPoints
			.Select(v => v - startPoint)
			.Select(v => Vector3.Cross(v, dir).z)
			.Sum()
		));
		Coord rotationCoord = startCoord.Neighbour(
			(NeighbourType) (((int) dirNeighbourType + sign + 6) % 6)
		);
		if (!MapContains(rotationCoord))
		{
			rotationCoord = startCoord.Neighbour(
				(NeighbourType) (((int) dirNeighbourType - sign + 6) % 6)
			);
		}
		return rotationCoord;
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

	public override bool Equals(object obj)
	{
		Coord other = (Coord) obj;
		if (other == null)
			return false;
		return other == this;
	}

	public override string ToString () {
		if ((Coord)this == Bad)
			return "HexCoord.Bad";
		return "x: " + x + " ,y: " + y;
	}
}

public enum NeighbourType
{
	Right = 0,
	TopRight = 1,
	TopLeft = 2,
	Left = 3,
	BottomLeft = 4,
	BottomRight = 5
}
