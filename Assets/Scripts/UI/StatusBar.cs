using UnityEngine;
using System.Collections;

public class StatusBar : MonoBehaviour {

	public RectTransform bar;

	public void SetPercentage (int count, int max) {
		SetPercentage (((float)count) / ((float)max));
	}

	public void SetPercentage (float percentage) {
		if (percentage > 1f)
			percentage = 1f;
		bar.anchorMax = new Vector2 (percentage, 1f);
		bar.offsetMin = Vector2.zero;
		bar.offsetMax = Vector2.zero;
	}
}
