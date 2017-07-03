using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIAim : MonoBehaviour {

	[Header("Refs")]
	public RectTransform rectTransform;

	[Space(5)]
	public Image outsidePanel;
	public Image insidePanel;

	[Space(5)]
	public Text pointsText;

	[Header("Look")]
	public float highlightLightness;
	public float textDarkness;

	public void SetColor (int color) {
		color--;
		outsidePanel.color = CommonInfo.Instance.lightColors[color];
		insidePanel.color = CommonInfo.Instance.usualColors[color];
		pointsText.color = CommonInfo.Instance.darkColors[color];
	}

	public void SetPoints (int points) {
		if (points > 0) {
			pointsText.text = points.ToString ();
		} else {
			pointsText.text = "0";
		}
	}

}
