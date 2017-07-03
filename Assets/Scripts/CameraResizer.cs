using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class CameraResizer {

	private static float screenWidth = 0;
	private static float screenHeight = 0;

	private static float ScreenWidth {
		get {
			if (screenWidth == 0) {
				screenWidth = (float)Screen.width;
			}
			return screenWidth;
		}
	}
	private static float ScreenHeight {
		get {
			if (screenHeight == 0) {
				screenHeight = (float)Screen.height;
			}
			return screenHeight;
		}
	}

	public static void ResizeInGame (this Camera cam) {
		float maxX, maxY;
		float maxSize;

//		maxY = 11 * Coord.Step.y * 0.5f;
//		maxX = 7 * Coord.Step.x * ((float)Screen.height / (float)Screen.width) * 0.5f;

		maxY = MapGenerator.Instance.height * Coord.Step.y * 0.5f;
		maxX = MapGenerator.Instance.width * Coord.Step.x * ((float)Screen.height / (float)Screen.width) * 0.5f;

		maxSize = (maxX * 1.2f > maxY * 1.3f) ? (maxX * 1.2f) : (maxY * 1.4f);

		cam.orthographicSize = maxSize;
	}

	public static void ResizeInLevelEditor (this Camera cam, LEMapManager map) {
		float maxX, maxY;
		float maxSize;
		maxY = map.height * Coord.Step.y * 0.5f;
		maxX = map.width * Coord.Step.x * ((float)ScreenHeight / (float)ScreenWidth) * 0.5f * 10f / 6f;

		maxSize = Mathf.Max (maxX, maxY);
		maxSize *= 1.1f;

		cam.orthographicSize = maxSize;
		cam.transform.position = new Vector3 (-maxSize * .4f * (float)ScreenWidth / ((float)ScreenHeight), 0, -1);
		cam.transform.position -= new Vector3 (map.offsetX, map.offsetY);
	}
}
