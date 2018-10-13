using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Panel, that highlights, when all hubbles of certain color are highlighted
/// </summary>
public class HighlightPanel : MonoBehaviour {

	public Canvas parentCanvas;
	public Image image;
	public Color defaultColor;

	public float setTime;

	private IEnumerator currentCoroutine;

	public void SetColor (Color color, bool betweenHubbles) {
		parentCanvas.sortingOrder = betweenHubbles ? 2 : -2;
		if (currentCoroutine != null)
			StopCoroutine (currentCoroutine);
		currentCoroutine = ISetColor (color);
		StartCoroutine (currentCoroutine);
	}

	IEnumerator ISetColor (Color color) {
		Color prevColor = image.color;

		for (float t = 0f; t < setTime; t += Time.deltaTime) {
			image.color = Color.Lerp (prevColor, color, AnimationFunctions.EasyOut (t / setTime, 3));
			yield return null;
		}

		image.color = color;
		yield return null;
	}
}
