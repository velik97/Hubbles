using UnityEngine;
using System.Collections;

/// <summary>
/// Resizes game field for given map parameters
/// </summary>
public class FieldResizer : MonoBehaviour {
	
	/// <summary>
	/// Resize game field for given params
	/// </summary>
	/// <param name="width">map width</param>
	/// <param name="height">map height</param>
	public void Resize (int width, int height) {
		float x = width * Coord.Step.x;
		float y = height * Coord.Step.y;

		transform.localScale = new Vector3 (x, y, 1f);
	}
}
