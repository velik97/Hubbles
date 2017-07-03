using UnityEngine;
using System.Collections;

public class FieldResizer : MonoBehaviour {
	
	public void Resize (int width, int height) {
		float x = width * Coord.Step.x;
		float y = height * Coord.Step.y;

		transform.localScale = new Vector3 (x, y, 1f);
	}
}
