using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LEMapManager : MonoSingleton <LEMapManager> {

	public int width;
	public int height;

	public Text statusText;

	[Header("Hubbles")]
	[Range(0.5f,1.5f)]
	public float hubbleSize;
	public LEHubble LEHubblePrefab;

	[Space(5)]
	public int drawColor;
	public int drawType;

	private LEHubble[,] hubbles;
	public int [,] colorMap;
	public int [,] typeMap;

	private int[,] previousColorMap = new int[0, 0];
	private int[,] previousHTypeMap = new int[0, 0];

	private Button[] colorsButtons;
	private bool[] pressedColorButtons;
	private Button[] typesButtons;
	private bool[] pressedTypeButtons;

	[HideInInspector] public float offsetX;
	[HideInInspector] public float offsetY;

	[HideInInspector] public float normalScale;

	[HideInInspector] public Vector2[,] map;

	private LevelConfig currentLevelConfig;
	public bool isEnabled = false;
	public bool mapIsSaved = false;

	private bool ConfigIsNew {
		get {
			if (currentLevelConfig == null)
				return false;
			if (LevelConfigHandler.CurrentConfig == null)
				return false;

			return currentLevelConfig != LevelConfigHandler.CurrentConfig;
		}
	}

	private bool mapIsGenerated = false;

	void Awake () {
		isEnabled = true;
		UpdateStatus (true);
		GenerateMap ();
	}

	void Update () {
		if (LETouchManager.Instance.justTouched) {
			UpdatePreviousHTypesAndColors ();
		}

		if (LETouchManager.Instance.selectZoneWasUpdated) {
			SetHubbles (LETouchManager.Instance.firstHitCoord, LETouchManager.Instance.currentCoord);
		}
			
	}

	void OnDestroy () {
		isEnabled = false;
	}

	public void GenerateMap () {
		if (!isEnabled)
			return;

		currentLevelConfig = LevelConfigHandler.CurrentConfig;

		if (currentLevelConfig == null)
			return;
		if (currentLevelConfig.colors == null || currentLevelConfig.colors.Length == 0)
			return;
		if (HTypeList.Get () == null || HTypeList.Get ().Length == 0)
			return;

		if (hubbles != null) {
			for (int i = 0; i < hubbles.GetLength (0); i++) {
				for (int j = 0; j < hubbles.GetLength (1); j++) {
					if (!(j % 2 == 0 && i == width - 1)) {
						if (hubbles [i, j] != null) {
							Destroy (hubbles [i, j].gameObject);
						}
					}
				}
			}
		}
	
		width = currentLevelConfig.Width;
		height = currentLevelConfig.Height;

		normalScale = LEHubblePrefab.transform.localScale.x * (Mathf.Sqrt(3f) / 2f) * hubbleSize;

		offsetX = -(Coord.Step.x * (width - 1) / 2f);
		offsetY = -(Coord.Step.y * (height - 1) / 2f);

		hubbles = new LEHubble[width, height];
		map = new Vector2[width, height];

		colorMap = new int[width, height];
		typeMap = new int[width, height];
		previousColorMap = new int[width, height];
		previousHTypeMap = new int[width, height];

		for (int i = 0; i < width; i ++) {
			for (int j = 0; j < height; j ++) {
				map[i,j] = new Vector2 (i * Coord.Step.x, j * Coord.Step.y);
				if (j % 2 == 0) {
					map[i,j] += Vector2.right * Coord.Step.x / 2f;
					if (i == width - 1) {
						map[i,j] = new Vector2 (1000f, 1000f);
					}
				}
			}
		}

		for (int i = 0; i < width; i ++) {
			for (int j = 0; j <height; j ++) {
				if (!(j % 2 == 0 && i == width - 1)) {

					LEHubble newLEHubble = Instantiate (LEHubblePrefab, (Vector3)map [i, j], Quaternion.identity) as LEHubble;
					newLEHubble.transform.localScale = Vector3.one * normalScale;
					newLEHubble.transform.SetParent (this.transform);
					hubbles [i, j] = newLEHubble;

					int type = currentLevelConfig.TypeMap [i + j * width];
					int color = currentLevelConfig.ColorMap [i + j * width];

					hubbles [i, j].SetHubble (type, color);
					colorMap [i, j] = color;
					typeMap [i, j] = type;
					previousColorMap [i, j] = color;
					previousHTypeMap [i, j] = type;
				}

			}
		}

		Camera.main.ResizeInLevelEditor (this as LEMapManager);
		Coord.MapSize = new Coord (width, height);

		mapIsGenerated = true;
	}

	public void SetColorButtons(Button[] buttons) {
		colorsButtons = buttons;
		pressedColorButtons = new bool[buttons.Length];

		for (int i = 0; i < buttons.Length; i++) {
			colorsButtons [i].GetComponent <Outline> ().enabled = false;
			pressedColorButtons [i] = false;
		}
	}

	public void SetTypeButtons(Button[] buttons) {
		typesButtons = buttons;
		pressedTypeButtons = new bool[buttons.Length];

		for (int i = 0; i < buttons.Length; i++) {
			typesButtons [i].GetComponent <Outline> ().enabled = false;
			pressedTypeButtons [i] = false;
		}
	}

	public void PressCollorButton (int i) {
		pressedColorButtons [i] = !pressedColorButtons [i];
		colorsButtons [i].GetComponent <Outline> ().enabled = pressedColorButtons [i];

		drawColor = 0;

		int multiplayer = 1;
		for (int j = 0; j < pressedColorButtons.Length; j++) {
			if (pressedColorButtons [j]) {
				drawColor += (j + 1) * multiplayer;
				multiplayer *= 10;
			}
		}
	}

	public void PressTypeButton (int i) {
		for (int j = 0; j < pressedTypeButtons.Length; j++) {
			pressedTypeButtons [j] = false;
		}

		pressedTypeButtons [i] = true;

		for (int j = 0; j < pressedTypeButtons.Length; j++) {
			typesButtons [j].GetComponent <Outline> ().enabled = pressedTypeButtons [j];
		}

		drawType = HTypeList.Get ()[i].index;
	}

	void UnSelectAllColors () {
		for (int j = 0; j < pressedColorButtons.Length; j++) {
			pressedColorButtons [j] = false;
			colorsButtons [j].GetComponent <Outline> ().enabled = false;
		}
	}


	void UpdatePreviousHTypesAndColors () {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (!(j % 2 == 0 && i == width - 1)) {
					previousColorMap [i, j] = colorMap [i, j];
					previousHTypeMap [i, j] = typeMap [i, j];
				}
			}
		}
	}

	void SetHubbles (Coord start, Coord end) { 
		if (start.x < 0 || start.x >= width || start.y < 0 || start.y >= height)
			return;

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (!(j % 2 == 0 && i == width - 1)) {
					hubbles [i, j].SetHubble (previousHTypeMap [i, j], previousColorMap [i, j]);
					colorMap [i, j] = previousColorMap [i, j];
					typeMap [i, j] = previousHTypeMap [i, j];
				}
			}
		}

		int minX = Mathf.Clamp (Mathf.Min (start.x, end.x), 0, width - 1);
		int maxX = Mathf.Clamp (Mathf.Max (start.x, end.x), 0, width - 1);
		int minY = Mathf.Clamp (Mathf.Min (start.y, end.y), 0, height - 1);
		int maxY = Mathf.Clamp (Mathf.Max (start.y, end.y), 0, height - 1);

		for (int i = minX; i <= maxX; i++) {
			for (int j = minY; j <= maxY; j++) {
				if (!(j % 2 == 0 && i == width - 1)) {
					hubbles [i, j].SetHubble (drawType, drawColor);
					colorMap [i, j] = drawColor;
					typeMap [i, j] = drawType;
				}
			}
		}
			
		UpdateStatus (false);
	}

	public void UpdateHubbles () {
		if (!isEnabled)
			return;

		if (!mapIsGenerated || ConfigIsNew)
			GenerateMap ();
		else {

			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					if (!(j % 2 == 0 && i == width - 1)) {
						hubbles [i, j].UpdateHubble ();
					}
				}
			}

		}
	}

	public void UpdateStatus (bool status) {
		print ("update status");
		mapIsSaved = status;
		statusText.text = status ? "Saved" : "Not Saved";
		statusText.color = status ? Color.black : Color.red;
	}
}
