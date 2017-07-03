using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUIGenerator : UIGenerator <MenuButton> {

	public InfoMenuPanel infoPanel;

	void Awake () {
		Generate ();
	}

	public override void Generate () {
		float offsetX = startX;
		float offsetY = startY;

		int buttonNum = 0;

		while (buttonNum < LevelConfigHandler.ConfigsCount) {
			int buttonInRow = 0;

			offsetX = startX;

			minX = offsetX;
			offsetX += buttonWidth;
			maxX = offsetX;

			minY = offsetY;
			offsetY += buttonHeight;
			maxY = offsetY;

			while (buttonNum < LevelConfigHandler.ConfigsCount && buttonInRow < 5) {
				MenuButton newButton = Instantiate (spawningObjectPrefab) as MenuButton;
				currentRectTransform = newButton.rectTransform;
				SetCurrentTransform ();
				int delegateButtonNum = buttonNum + 1;
				newButton.button.onClick.AddListener (delegate {
					ActivateButton (delegateButtonNum);
				});

				newButton.starsHolder.SetStars (LevelConfigHandler.GetStarsCount (delegateButtonNum));
				newButton.text.text = delegateButtonNum.ToString ();

				newButton.button.interactable = buttonNum <= LevelConfigHandler.LastDoneLevel;

				offsetX += spaceBetweenButtonsX;

				minX = offsetX;
				offsetX += buttonWidth;
				maxX = offsetX;

				buttonNum++;
				buttonInRow++;
			}

			offsetY += spaceBetweenButtonsY;
		}


	}

	void ActivateButton (int i) {
		LevelConfigHandler.CurrentIndex = i; 
		MenuManager.Instance.OpenMenuPanel (infoPanel);
	}

}
