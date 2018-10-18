using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Resizes camera for game field size
/// </summary>
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

	private static float DefaultRation
	{
		get { return 1.7778f; } // 16:9
	}

	private static float DefaultCameraSize
	{
		get { return 9.6f; }
	} 

	public static void ResizeInGame (this Camera cam)
	{
		float ratio = ScreenHeight / ScreenWidth;
		if (ratio < DefaultRation)
			cam.orthographicSize = DefaultCameraSize;
		else
			cam.orthographicSize = DefaultCameraSize * ratio / DefaultRation;
	}
}
