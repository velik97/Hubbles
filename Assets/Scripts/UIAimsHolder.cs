using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIAimsHolder : MonoBehaviour {

	public UIAim UIAimPrefab;
	public float width;
	public float space;

	Dictionary <int, UIAim> aimDictionary;

	private RectTransform currentRectTransform;

	private float minX, maxX;
	private float minY = 0f, maxY = 1f;

	public void GenerateAims () {
		LevelConfig config = LevelConfigHandler.CurrentConfig;

		int length = config.aims.Length;

		if (length > 4) {
			Debug.LogError ("There can't be more then 4 aims");
			return;
		} else if (length <= 0){
			Debug.LogError ("There can't be less then 1 aim");
			return;
		}

		minX = .5f-((width * length) + (space * (length - 1)))/2f;
		maxX = minX + width;

		DestroyAims ();
		aimDictionary = new Dictionary<int, UIAim> ();
		CommonInfo.Instance.UpdateColors ();
		for (int i = 0; i < length; i++) {
			UIAim newUIAim = Instantiate (UIAimPrefab) as UIAim;
			currentRectTransform = newUIAim.rectTransform;
			SetCurrentTransform ();
			newUIAim.SetColor (config.aims[i].color);
			newUIAim.SetPoints (config.aims[i].count);
			minX += width + space;
			maxX += width + space;
			aimDictionary.Add (config.aims[i].color, newUIAim);
		}
			
	}

	void DestroyAims () {
		if (aimDictionary != null) {
			foreach (UIAim uiaim in aimDictionary.Values) {
				if (aimDictionary != null) {
					Destroy (uiaim.gameObject);
				}
			}
			aimDictionary.Clear ();
		}
	}

	public void SetCParticularAimCount (int color, int points) {
		aimDictionary [color].SetPoints (points);
	}

	void SetCurrentTransform () {
		currentRectTransform.SetParent (this.transform, false);
		currentRectTransform.anchorMin = new Vector2 (minX, minY);
		currentRectTransform.anchorMax = new Vector2 (maxX, maxY);
		currentRectTransform.offsetMax = Vector2.zero;
		currentRectTransform.offsetMin = Vector2.zero;
	}
}
