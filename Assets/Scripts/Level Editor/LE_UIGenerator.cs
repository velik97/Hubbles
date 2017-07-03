using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LE_UIGenerator : UIGenerator <Button> {
	
	[Header("Prefabs")]
	[Header("Extra")]
	public Text bigTextPrefab;
	public Button buttonWithTextPrefab;

	[Header("Sizes")]
	public float bigTextWidth;
	public float bigTextHeight;

	[Space(5)]
	public float spaceToNextGroup;

	private Button[] typeButtons;
	private Button[] colorButtons;
	private Text typeText;
	private Text colorText;

	private bool isEnabled;

	void Start () {
		isEnabled = true;
		Generate ();
	}

	void OnDestroy () {
		isEnabled = false;
	}

	public override void Generate () {
		if (!isEnabled)
			return;
		if (LevelConfigHandler.CurrentConfig == null)
			return;

		if (typeButtons != null) {
			foreach (Button b in typeButtons) {
				Destroy (b.gameObject);
			}
		}

		if (colorButtons != null) {
			foreach (Button b in colorButtons) {
				Destroy (b.gameObject);
			}
		}

		if (typeText != null) {
			Destroy (typeText.gameObject);
		}

		if (colorText != null) {
			Destroy (colorText.gameObject);
		}

		float offsetX = startX;
		float offsetY = startY;

		minX = offsetX;
		offsetX += bigTextWidth;
		maxX = offsetX;

		maxY = offsetY;
		offsetY -= bigTextHeight;
		minY = offsetY;

		typeText = Instantiate (bigTextPrefab) as Text;
		currentRectTransform = typeText.GetComponent <RectTransform> ();
		currentRectTransform.SetParent (this.transform, true);
		SetCurrentTransform ();
		typeText.text = "Types";

		int buttonNum = 0;

		typeButtons = new Button[HTypeList.Get ().Length];

		while (buttonNum < typeButtons.Length) {
			offsetX = startX;
			int buttonInRow = 0;

			minX = offsetX;
			offsetX += buttonWidth;
			maxX = offsetX;

			maxY = offsetY;
			offsetY -= buttonHeight;
			minY = offsetY;

			while (buttonNum < typeButtons.Length && buttonInRow < 3) {
				Button typeButton = Instantiate (buttonWithTextPrefab) as Button;
				currentRectTransform = typeButton.GetComponent <RectTransform> ();
				SetCurrentTransform ();
				typeButton.GetComponentInChildren <Text> ().text = HTypeList.Get ()[buttonNum].name;
				int delegateButtonNum = buttonNum;
				typeButton.onClick.AddListener (delegate {
					LEMapManager.Instance.PressTypeButton (delegateButtonNum);
				});
				typeButtons [buttonNum] = typeButton;

				offsetX += spaceBetweenButtonsX;
				minX = offsetX;
				offsetX += buttonWidth;
				maxX = offsetX;

				buttonNum++;
				buttonInRow++;
			}

			offsetY -= spaceBetweenButtonsY;
		}
		LEMapManager.Instance.SetTypeButtons (typeButtons);
			
		offsetY += spaceBetweenButtonsY;
		offsetY -= spaceToNextGroup;
		offsetX = startX;

		minX = offsetX;
		offsetX += bigTextWidth;
		maxX = offsetX;

		maxY = offsetY;
		offsetY -= bigTextHeight;
		minY = offsetY;

		colorText = Instantiate (bigTextPrefab) as Text;
		currentRectTransform = colorText.GetComponent <RectTransform> ();
		currentRectTransform.SetParent (this.transform, true);
		SetCurrentTransform ();
		colorText.text = "Colors";

		buttonNum = 0;

		colorButtons = new Button[LevelConfigHandler.CurrentConfig.colors.Length];

		while (buttonNum < colorButtons.Length) {
			offsetX = startX;
			int buttonInRow = 0;

			minX = offsetX;
			offsetX += buttonWidth;
			maxX = offsetX;

			maxY = offsetY;
			offsetY -= buttonHeight;
			minY = offsetY;

			while (buttonNum < colorButtons.Length && buttonInRow < 3) {
				Button colorButton = Instantiate (spawningObjectPrefab) as Button;
				currentRectTransform = colorButton.GetComponent <RectTransform> ();
				SetCurrentTransform ();
				colorButton.GetComponentInChildren <Image> ().color = LevelConfigHandler.CurrentConfig.colors[buttonNum];
				int delegateButtonNum = buttonNum;
				colorButton.onClick.AddListener (delegate {
					LEMapManager.Instance.PressCollorButton (delegateButtonNum);
				});
				colorButtons [buttonNum] = colorButton;

				offsetX += spaceBetweenButtonsX;
				minX = offsetX;
				offsetX += buttonWidth;
				maxX = offsetX;

				buttonNum++;
				buttonInRow++;
			}

			offsetY -= spaceBetweenButtonsY;
		}

		LEMapManager.Instance.SetColorButtons (colorButtons);
	}

}
