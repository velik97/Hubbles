using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class LevelEditorWindow : EditorWindow  {

	[MenuItem ("Window/Level Editor")]
	public static void ShowLevelEditorWindow () {
		var window = GetWindow <LevelEditorWindow> ();
		window.titleContent.text = "Level Editor";
	}
		
	private LevelConfig currentLevelConfig;
	public int viewIndex;

	public HType[] htypes;
	private bool editingNewHTypeIndex;
	private int newIndex;

	private bool[] pressedColorButtons;
	private bool[] pressedTypeButtons;

	private Vector2 scrollPosition;

	public void OnEnable () {
		if (LevelConfigHandler.Instance == null)
			LevelConfigHandler.Instance = CreateScriptableObject <LevelConfigHandler> ();
		if (HTypeList.Instance == null)
			HTypeList.Instance = CreateScriptableObject <HTypeList> ();

		viewIndex = 1;
		editingNewHTypeIndex = false;
		htypes = HTypeList.Get ();

		if (LevelConfigHandler.ConfigsCount > 0) {
			LevelConfigHandler.CurrentIndex = LevelConfigHandler.ConfigsCount;
			currentLevelConfig = LevelConfigHandler.CurrentConfig;
		}
	}

	public void OnGUI () {
		if (LEMapManager.Instance == null) {
			EditorGUILayout.LabelField ("This window is enabled only in 'Level Editor' scene");
			return;
		}

		DoMenu ();
		GUILayout.Box ("", new GUILayoutOption[] {GUILayout.ExpandWidth (true), GUILayout.Height (1)});

		scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true));
		{

			if (LevelConfigHandler.ConfigsCount > 0) {
				EditorGUI.BeginChangeCheck ();
				DoEditorSettings ();
				if (EditorGUI.EndChangeCheck ()) {
					LEMapManager.Instance.UpdateStatus (false);
				}
					
				DoHTypeEdit ();
			}
		}
		GUILayout.EndScrollView ();

	}

	void DoMenu () {
		GUILayout.BeginHorizontal ();
		{
			if (GUILayout.Button ("Add Level", GUILayout.ExpandWidth (false))) {
				AddLevel ();
			}
			if (LevelConfigHandler.ConfigsCount > 0 && GUILayout.Button ("Delete Level", GUILayout.ExpandWidth (false))) {
				DeleteLevel ();
			}

			if (LevelConfigHandler.ConfigsCount > 0 && !LEMapManager.Instance.mapIsSaved) {
				EditorGUILayout.LabelField ("", GUILayout.Width (20));
				if (GUILayout.Button ("Save Config", GUILayout.ExpandWidth (false)))
					SaveMap ();
			}
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		{
			if (LevelConfigHandler.ConfigsCount > 0) {
				int i = Mathf.Clamp (EditorGUILayout.IntField (viewIndex, GUILayout.Width (50)), 1, LevelConfigHandler.ConfigsCount);
				if (i != viewIndex) {
					ChangeCurrentLevel (i);
				}
				EditorGUILayout.LabelField ("of " + LevelConfigHandler.ConfigsCount + " levels", GUILayout.ExpandWidth (false));
			}
		}
		GUILayout.EndHorizontal ();
	}

	void DoEditorSettings () {
		EditorGUI.BeginChangeCheck ();

		GUILayout.BeginHorizontal ();
		{
			EditorGUILayout.LabelField ("Width", GUILayout.Width (75));

			if (currentLevelConfig.Width > 3 ) {
				if (GUILayout.Button ("-", GUILayout.Width (20))) {
					currentLevelConfig.Width--;
				}
			} else {
				EditorGUILayout.LabelField ("", GUILayout.Width (20));
			}

			EditorGUILayout.LabelField (currentLevelConfig.Width.ToString (), GUILayout.Width (20));

			if (currentLevelConfig.Width < 7 ) {
				if (GUILayout.Button ("+", GUILayout.Width (20))) {
					currentLevelConfig.Width++;
				}
			} else {
				EditorGUILayout.LabelField ("", GUILayout.Width (20));
			}
				
		}
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		{
			EditorGUILayout.LabelField ("Height", GUILayout.Width (75));

			if (currentLevelConfig.Height > 3 ) {
				if (GUILayout.Button ("-", GUILayout.Width (20))) {
					currentLevelConfig.Height--;
				}
			} else {
				EditorGUILayout.LabelField ("", GUILayout.Width (20));
			}

			EditorGUILayout.LabelField (currentLevelConfig.Height.ToString (), GUILayout.Width (20));

			if (currentLevelConfig.Height < 11 ) {
				if (GUILayout.Button ("+", GUILayout.Width (20))) {
					currentLevelConfig.Height++;
				}
			} else {
				EditorGUILayout.LabelField ("", GUILayout.Width (20));
			}
		}
		GUILayout.EndHorizontal ();

		if (EditorGUI.EndChangeCheck ()) {
			LEMapManager.Instance.GenerateMap ();
		}
			
		EditorGUILayout.Space ();

		currentLevelConfig.multiplayerChance = EditorGUILayout.Slider ("Multiplayer Chance", currentLevelConfig.multiplayerChance, 0, 1000, GUILayout.Width (400));
		currentLevelConfig.heartChance = EditorGUILayout.Slider ("Heart Chance", currentLevelConfig.heartChance, 0, 1000, GUILayout.Width (400));

		GUILayout.BeginHorizontal ();
		{
			EditorGUILayout.LabelField ("Start Lives", GUILayout.Width (150));
			currentLevelConfig.startLives = EditorGUILayout.IntField (currentLevelConfig.startLives, GUILayout.Width (75));
		}
		GUILayout.EndHorizontal ();
		EditorGUILayout.Space ();
		DoAimEdit ();
		EditorGUILayout.Space ();
		DoStarsEdit ();
		GUILayout.Box ("", new GUILayoutOption[] {GUILayout.ExpandWidth (true), GUILayout.Height (1)});
		DoColorEdit ();
		EditorGUILayout.Space ();
	}

	void DoColorEdit () {
		EditorGUILayout.LabelField ("Colors", GUILayout.Width (75));

		EditorGUI.BeginChangeCheck ();
		if (currentLevelConfig.colors != null) {
			for (int i = 0; i < currentLevelConfig.colors.Length; i++) {
				currentLevelConfig.colors[i] = EditorGUILayout.ColorField (currentLevelConfig.colors[i], GUILayout.Width (100));
			}
		} else {
			currentLevelConfig.colors = new Color[0];
		}
		if (EditorGUI.EndChangeCheck ()) {
			LE_UIGenerator.Instance.Generate ();
			LEMapManager.Instance.UpdateHubbles ();
		}

		GUILayout.BeginHorizontal ();
		{
			if (GUILayout.Button ("Add", GUILayout.ExpandWidth (false))) {
				AddColor ();
			}
			if (currentLevelConfig.colors.Length > 0 && GUILayout.Button ("Del", GUILayout.ExpandWidth (false))) {
				DelColor ();
			}
		}
		GUILayout.EndHorizontal ();
	}

	void DoAimEdit () {
		EditorGUILayout.LabelField ("Aims", GUILayout.Width (75));
		GUILayout.BeginHorizontal ();
		{
			EditorGUILayout.LabelField ("", GUILayout.Width (75));
			EditorGUILayout.LabelField ("Color", GUILayout.Width (75));
			EditorGUILayout.LabelField ("Type", GUILayout.Width (75));
			EditorGUILayout.LabelField ("Count", GUILayout.Width (75));
		}
		GUILayout.EndHorizontal ();

		if (currentLevelConfig.aims != null) {
			for (int i = 0; i < currentLevelConfig.aims.Length; i++) {
				GUILayout.BeginHorizontal ();
				{
					EditorGUILayout.LabelField ("Aim " + (i + 1).ToString (), GUILayout.Width (75));
					currentLevelConfig.aims [i].color = EditorGUILayout.IntField (currentLevelConfig.aims [i].color, GUILayout.Width (75));
					currentLevelConfig.aims [i].type = EditorGUILayout.IntField (currentLevelConfig.aims [i].type, GUILayout.Width (75));
					currentLevelConfig.aims [i].count = EditorGUILayout.IntField (currentLevelConfig.aims [i].count, GUILayout.Width (75));
				}
				GUILayout.EndHorizontal ();
			}
		} else {
			currentLevelConfig.aims = new Aim[0];
		}

		GUILayout.BeginHorizontal ();
		{
			if (GUILayout.Button ("Add", GUILayout.ExpandWidth (false))) {
				Addaim ();
			}
			if (currentLevelConfig.aims.Length > 0 && GUILayout.Button ("Del", GUILayout.ExpandWidth (false))) {
				DelAim ();
			}
		}
		GUILayout.EndHorizontal ();
	}

	void DoStarsEdit () {
		EditorGUILayout.LabelField ("Stars", GUILayout.Width (75));

		GUILayout.BeginHorizontal ();
		{
			EditorGUILayout.LabelField ("1", GUILayout.Width (75));
			int oneStarAim = 0;
			if (currentLevelConfig.aims != null) {
				foreach (Aim aim in currentLevelConfig.aims) {
					oneStarAim += aim.count;
				}
			}
			currentLevelConfig.oneStarScore = oneStarAim;
			EditorGUILayout.LabelField (oneStarAim.ToString (), GUILayout.Width (75));
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		{
			EditorGUILayout.LabelField ("2", GUILayout.Width (75));
			currentLevelConfig.twoStarsScore = EditorGUILayout.IntField (currentLevelConfig.twoStarsScore, GUILayout.Width (75));
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		{
			EditorGUILayout.LabelField ("3", GUILayout.Width (75));
			currentLevelConfig.threeStarsScore = EditorGUILayout.IntField (currentLevelConfig.threeStarsScore, GUILayout.Width (75));
		}
		GUILayout.EndHorizontal ();

	}

	void DoHTypeEdit () {
		EditorGUILayout.LabelField ("Hubble Types", GUILayout.Width (75));
		GUILayout.BeginHorizontal ();
		{
			EditorGUILayout.LabelField ("", GUILayout.Width (75));
			EditorGUILayout.LabelField ("Index", GUILayout.Width (50));
			EditorGUILayout.LabelField ("Name", GUILayout.Width (150));
			EditorGUILayout.LabelField ("ShortCut", GUILayout.Width (75));
		}
		GUILayout.EndHorizontal ();

		if (htypes != null) {
			for (int i = 0; i < htypes.Length; i++) {
				GUILayout.BeginHorizontal ();
				{
					EditorGUILayout.LabelField ("Htype " + (i + 1).ToString (), GUILayout.Width (75));

					EditorGUI.BeginChangeCheck ();

					EditorGUILayout.LabelField (htypes [i].index.ToString (), GUILayout.Width (50));
					htypes [i].name = EditorGUILayout.TextField (htypes [i].name, GUILayout.Width (150));
					htypes [i].shortCut = EditorGUILayout.TextField (htypes [i].shortCut, GUILayout.Width (50));

					if (EditorGUI.EndChangeCheck ()) {
						HTypeList.UpdateList (htypes [i]);
						LE_UIGenerator.Instance.Generate ();
						LEMapManager.Instance.UpdateHubbles ();
					}

				}
				GUILayout.EndHorizontal ();
			}
		}

		GUILayout.BeginHorizontal ();
		if (editingNewHTypeIndex) {
			AddHType ();
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		{
			if (GUILayout.Button ("Add", GUILayout.ExpandWidth (false)) && !editingNewHTypeIndex) {
				editingNewHTypeIndex = true;
				newIndex = 0;
			}

			if (htypes.Length > 0 && GUILayout.Button ("Del", GUILayout.ExpandWidth (false))) {
				DelHType ();
			}
		}
		GUILayout.EndHorizontal ();
	}

	void AddLevel () {
		Color[] prevColors = new Color[0];

		if (currentLevelConfig != null && currentLevelConfig.colors != null) {
			prevColors = currentLevelConfig.colors;
		}

		LevelConfig newLevelConfig;

		newLevelConfig = LevelConfigHandler.CreateConfig (LevelConfigHandler.ConfigsCount);
		newLevelConfig.colors = prevColors;
		newLevelConfig.Width = 3;
		newLevelConfig.Height = 3;

		if (LevelConfigHandler.ConfigsCount == 1) {
			currentLevelConfig = newLevelConfig;
			viewIndex = 1;
		}
	}

	void ChangeCurrentLevel (int i) {
		if (!LEMapManager.Instance.mapIsSaved && viewIndex != 0) {
			switch (EditorUtility.DisplayDialogComplex ("'Level " + viewIndex.ToString () + "' Was Modified",
				"Do you want to save 'Level " + viewIndex.ToString () + "' before going to 'Level " + i.ToString () + "'?", "Save", "Don't Save", "Cancel")) {

			case 0:
				SaveMap ();
				break;

			case 1:
				break;

			case 2:
				return;
			}
		}

		viewIndex = i;
		LevelConfigHandler.CurrentIndex = i;
		currentLevelConfig = LevelConfigHandler.CurrentConfig;
		LEMapManager.Instance.UpdateStatus (true);
		LEMapManager.Instance.GenerateMap ();
		LE_UIGenerator.Instance.Generate ();
	}

	void DeleteLevel () {
		if (LevelConfigHandler.ConfigsCount > 0 && EditorUtility.DisplayDialog ("Really?", "Do you want to delete 'Levev " + LevelConfigHandler.ConfigsCount.ToString () + "'?", "Yes", "No")) {
			LevelConfigHandler.DeleteLastConfig ();
			if (viewIndex == LevelConfigHandler.ConfigsCount + 1) {
				viewIndex--;
				LEMapManager.Instance.GenerateMap ();
				LE_UIGenerator.Instance.Generate ();
			}
		}
	}

	void AddColor () {
		Color[] tempColors = currentLevelConfig.colors;
		currentLevelConfig.colors = new Color[currentLevelConfig.colors.Length + 1];

		for (int i = 0; i < tempColors.Length; i++) {
			currentLevelConfig.colors [i] = tempColors [i];
		}
		LE_UIGenerator.Instance.Generate ();
		LEMapManager.Instance.UpdateHubbles ();
	}

	void DelColor () {
		if (currentLevelConfig.colors.Length > 0) {
			Color[] tempColors = currentLevelConfig.colors;
			currentLevelConfig.colors = new Color[currentLevelConfig.colors.Length - 1];

			for (int i = 0; i < currentLevelConfig.colors.Length; i++) {
				currentLevelConfig.colors [i] = tempColors [i];
			}
			LE_UIGenerator.Instance.Generate ();
			LEMapManager.Instance.UpdateHubbles ();
		}
	}

	void Addaim () {
		Aim[] tempAims = currentLevelConfig.aims;
		currentLevelConfig.aims = new Aim[currentLevelConfig.aims.Length + 1];

		for (int i = 0; i < tempAims.Length; i++) {
			currentLevelConfig.aims [i] = tempAims [i];
		}
		currentLevelConfig.aims[currentLevelConfig.aims.Length - 1] = new Aim ();
	}

	void DelAim () {
		if (currentLevelConfig.aims.Length > 0) {
			Aim[] tempAims = currentLevelConfig.aims;
			currentLevelConfig.aims = new Aim[currentLevelConfig.aims.Length - 1];

			for (int i = 0; i < currentLevelConfig.aims.Length; i++) {
				currentLevelConfig.aims [i] = tempAims [i];
			}
		}
	}

	void AddHType () {
		if (GUILayout.Button ("Ready", GUILayout.Width (75))) {
			if (!HTypeList.Contains (newIndex)) {
				HTypeList.UpdateList (new HType (newIndex, "", ""));
				htypes = HTypeList.Get ();
				LE_UIGenerator.Instance.Generate ();
				LEMapManager.Instance.UpdateHubbles ();
			} else {
				EditorUtility.DisplayDialog ("You can't do this", "HType with index '" + newIndex + "' allready exists", "OK");
			}
			editingNewHTypeIndex = false;
		}

		newIndex = EditorGUILayout.IntField (newIndex, GUILayout.Width (50));
		if (GUILayout.Button ("Cancel", GUILayout.Width (75))) {
			editingNewHTypeIndex = false;
		}
	}

	void DelHType () {
		if (HTypeList.DelHType (htypes [htypes.Length - 1])) {
			htypes = HTypeList.Get ();
			LE_UIGenerator.Instance.Generate ();
			LEMapManager.Instance.UpdateHubbles ();
		}

	}

	void SaveMap () {
		if (LEMapManager.Instance.isEnabled) {

			int width = currentLevelConfig.width;
			int height = currentLevelConfig.height;

			int[] newColorMap = new int[width * height];
			int[] newTypeMap = new int[width * height];

			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					if (!(j % 2 == 0 && i == width - 1)) {
						newColorMap [i + j * width] = LEMapManager.Instance.colorMap [i, j];
						newTypeMap [i + j * width] = LEMapManager.Instance.typeMap [i, j];
					}
				}
			}

			currentLevelConfig.colorMap = newColorMap;
			currentLevelConfig.typeMap = newTypeMap;	
		}

		Debug.Log ("save");
		SaveAsset <LevelConfigHandler> (LevelConfigHandler.Instance);
		SaveAsset <HTypeList> (HTypeList.Instance);
		LEMapManager.Instance.UpdateStatus (true);

	}

	T CreateScriptableObject <T> () where T : ScriptableObject
	{
		Debug.Log ("creating" + typeof(T).ToString ()); 
		T asset;

		if (!AssetDatabase.IsValidFolder ("Assets/Info")) {
			AssetDatabase.CreateFolder ("Assets", "Info");
			asset = CreateAsset <T> ();
		} else {
			asset = AssetDatabase.LoadAssetAtPath <T> ("Assets/Info/" + typeof(T).ToString() + ".asset");

			if (asset == null) {
				asset = CreateAsset <T> ();
			}
		}

		return asset;
	}

	public T CreateAsset <T> () where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance <T> ();
		AssetDatabase.CreateAsset (asset, "Assets/Info/" + typeof(T).ToString() + ".asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}

	public void SaveAsset <T> (T asset) where T : ScriptableObject
	{
		AssetDatabase.Refresh ();
		EditorUtility.SetDirty (asset);
		AssetDatabase.SaveAssets ();
	}

}
