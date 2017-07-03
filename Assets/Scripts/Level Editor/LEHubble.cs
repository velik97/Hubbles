using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LEHubble : MonoBehaviour {
	public int color;
	public int type;

	public SpriteRenderer[] circleRenderers;
	public Text typeText;

	public void SetHubble (int type, int color) {
		if (!(this.type == type && this.color == color)) {

			this.type = type;

			if (type == 0 || type == 4) { // Empty or Gear
				this.color = 0;
			} else {
				this.color = color;
			}

			UpdateHubble ();
		}
	}

	public void UpdateHubble () {
		if (this.type == 0) { // Empty
			for (int i = 0; i < circleRenderers.Length; i++) {
				circleRenderers [i].color = new Color (1, 1, 1, 0);
			}
			typeText.text = "";
		} else if (this.type == 4 || this.type == 5) { // Gear or Versicolor
			for (int i = 1; i < circleRenderers.Length; i++) {
				circleRenderers [i].color = Color.white;
			}
			circleRenderers [0].color = Color.black;

			typeText.color = Color.black;
			typeText.text = HTypeList.GetWithIndex (type).shortCut;
		} else if (!(color == 0)) {
			int tempColor = color;
			int index = 0;
			while (tempColor > 0) {
				tempColor /= 10;
				index++;
			}

			int[] colorsToApply = new int[index];

			tempColor = color;
			index = 0;

			while (tempColor > 0) {
				colorsToApply [index] = tempColor % 10;
				colorsToApply [index]--;
				tempColor /= 10;
				index++;
			}

			for (int i = 0; i < circleRenderers.Length; i++) {
				circleRenderers [i].color = LevelConfigHandler.CurrentConfig.colors [Mathf.Clamp (colorsToApply [Mathf.Clamp (i, 0, colorsToApply.Length - 1)], 0, LevelConfigHandler.CurrentConfig.colors.Length - 1)];
			}

			typeText.color = Color.white;
			typeText.text = HTypeList.GetWithIndex (type).shortCut;
		}
	}

	// ========================

	// // Types

	//-2 -- none
	//-1 -- usual
	// 0 -- empty
	// 1 -- health
	// 2 -- multiplayer
	// 3 -- ice
	// 4 -- gear
	// 5 -- versicolor

	// // Colors

	// 0 -- none
	// 1... -- other colors

	// Example: 
	// 123 is 1, 2 or 3 color

	// ========================
}
