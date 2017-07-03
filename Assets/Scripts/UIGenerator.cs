using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class UIGenerator <T> : MonoSingleton <UIGenerator<T>> {

	[Header("Prefabs")]
	public T spawningObjectPrefab;

	[Header("Sizes")]
	public float startX;
	public float startY;

	[Space(5)]
	public float buttonWidth;
	public float buttonHeight;

	[Space(5)]
	public float spaceBetweenButtonsX;
	public float spaceBetweenButtonsY;

	protected RectTransform currentRectTransform;

	protected float minX, maxX;
	protected float minY, maxY;

	public abstract void Generate ();

	protected void SetCurrentTransform () {
		currentRectTransform.SetParent (this.transform, false);
		currentRectTransform.anchorMin = new Vector2 (minX, minY);
		currentRectTransform.anchorMax = new Vector2 (maxX, maxY);
		currentRectTransform.offsetMax = Vector2.zero;
		currentRectTransform.offsetMin = Vector2.zero;
	}
		
}
