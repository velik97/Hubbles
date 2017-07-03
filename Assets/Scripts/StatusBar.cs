using UnityEngine;
using System.Collections;

public class StatusBar : MonoBehaviour {

	public RectTransform bar;

	public void SetPersentage (int count, int max) {
		SetPersentage (((float)count) / ((float)max));
	}

	public void SetPersentage (float persantage) {
		if (persantage > 1f)
			persantage = 1f;
		bar.anchorMax = new Vector2 (persantage, 1f);
		bar.offsetMin = Vector2.zero;
		bar.offsetMax = Vector2.zero;
	}
}
