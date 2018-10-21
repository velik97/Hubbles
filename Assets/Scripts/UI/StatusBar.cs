using UnityEngine;
using System.Collections;

public class StatusBar : MonoBehaviour {

	public RectTransform bar;
	
	public Vector2 minOffset = Vector2.zero;
	public Vector2 maxOffset = Vector2.zero;

	public void SetPercentage (int count, int max) {
		SetPercentage (((float)count) / ((float)max));
	}

	public void SetPercentage (float percentage) {
		if (percentage > 1f)
			percentage = 1f;
		bar.anchorMax = new Vector2 (percentage, 1f);
		bar.offsetMin = minOffset;
		bar.offsetMax = maxOffset;
	}
}
