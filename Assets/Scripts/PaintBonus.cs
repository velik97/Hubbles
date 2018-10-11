using UnityEngine;
using System.Collections;

/// <summary>
/// Bonus, that paints surrounding nodes
/// </summary>
public class PaintBonus : Bonus <PaintBonus> {

	public int color;

	public override void Apply (Coord coordToApply) {
		base.Apply (coordToApply);

		foreach (Coord coord in Map.NearCoords (coordToApply, Map.Paintable)) {
			Node node = Map.nodeMap [coord.x, coord.y];
			node.ChangeParams (node.type, color, 0);	
		}
			
	}

}
