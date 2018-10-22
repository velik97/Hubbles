using UnityEngine;
using System.Collections;

public class StatusBar : MonoBehaviour, IStatusGraphics {

	public RectTransform bar;
	
	public Vector2 minOffset = Vector2.zero;
	public Vector2 maxOffset = Vector2.zero;

	public void SetStatus (int count, int max) {
		SetStatus (((float)count) / ((float)max));
	}

	public void SetStatus (float percentage) {
		if (percentage > 1f)
			percentage = 1f;
		bar.anchorMax = new Vector2 (percentage, 1f);
		bar.offsetMin = minOffset;
		bar.offsetMax = maxOffset;
	}
}
