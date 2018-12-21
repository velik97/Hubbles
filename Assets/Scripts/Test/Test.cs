using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{
	void Start ()
	{
		Coord.MapSize = new Coord(10,10);
		print(Coord.RotationCoord(
			Coord.Vector2FromCoord(new Coord(2, 2)),
			Coord.Vector2FromCoord(new Coord(3, 1)),
			new Vector2[0]));
	}
}
