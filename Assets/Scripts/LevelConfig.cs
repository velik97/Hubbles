using UnityEngine;
using System.Collections;

/// <summary>
/// Level Configuration. Need change
/// </summary>
[System.Serializable]
public class LevelConfig {

	public int width, height;
	public int[] typeMap;
	public int[] colorMap;

	public int startLives;
	
	/// <summary>
	/// Deprecated
	/// </summary>
	public Aim[] aims;

	/// <summary>
	/// Deprecated
	/// </summary>
	public int oneStarScore;
	/// <summary>
	/// Deprecated
	/// </summary>
	public int twoStarsScore;
	/// <summary>
	/// Deprecated
	/// </summary>
	public int threeStarsScore;

	/// <summary>
	/// Need Change
	/// </summary>
	public float heartChance;
	public float multiplayerChance;

	public Color[] colors;

	public LevelConfig () {}

	public int[] TypeMap {
		get {
			if (typeMap == null || typeMap.Length == 0) {
				typeMap = new int[width * height];

				int type = 0;

				if (HTypeList.Get () != null && HTypeList.Get ().Length != 0)
					type = HTypeList.Get () [0].index;

				for (int i = 0; i < width; i++) {
					for (int j = 0; j < height; j++) {
						if (!(j % 2 == 0 && i == width - 1)) {
							typeMap [i + j * width] = type;
						}

					}
				}
			}

			return typeMap;
		}
		set {
			typeMap = value;
		}
	}

	public int[] ColorMap {
		get {
			if (colorMap == null || colorMap.Length == 0) {
				colorMap = new int[width * height];

				for (int i = 0; i < width; i++) {
					for (int j = 0; j < height; j++) {
						if (!(j % 2 == 0 && i == width - 1)) {
							colorMap [i + j * width] = 1;
						}

					}
				}
			}

			return colorMap;
		}
		set {
			colorMap = value;
		}
	}

	public int Width {
		get {
			return width;
		}
		set {
			int color = 1;
			int type = HTypeList.DefaultIndex;

			int[] newColroMar = new int[value * height];
			int[] newTypeMap = new int[value * height];

			bool mapIsNull = colorMap == null || colorMap.Length == 0 || typeMap == null || typeMap.Length == 0;

			for (int i = 0; i < Mathf.Max (width, value); i++) {
				for (int j = 0; j < height; j++) {
					bool nowContains = (j < height && i < value && !(j % 2 == 0 && i == value - 1));
					bool prevContains = (j < height && i < width && !(j % 2 == 0 && i == width - 1));

					if (nowContains && !mapIsNull) {
						if (prevContains) {
							newColroMar [i + j * value] = colorMap [i + j * width];
							newTypeMap [i + j * value] = typeMap [i + j * width];
						} else {
							newColroMar [i + j * value] = color;
							newTypeMap [i + j * value] = type;
						}
					}

				}
			}

			colorMap = newColroMar;
			typeMap = newTypeMap;
			width = value;
		}
	}

	public int Height {
		get {
			return height;
		}
		set {
			int color = 1;
			int type = HTypeList.DefaultIndex;

			int[] newColroMar = new int[width * value];
			int[] newTypeMap = new int[width * value];

			for (int i = 0; i < width; i++) {
				for (int j = 0; j < Mathf.Max (height, value); j++) {
					bool nowContains = (j < value && i < width && !(j % 2 == 0 && i == width - 1));
					bool prevContains = (j < height && i < width && !(j % 2 == 0 && i == width - 1));

					if (nowContains && !prevContains) {
						newColroMar [i + j * width] = color;
						newTypeMap [i + j * width] = type;
					} else if (nowContains && prevContains) {
						newColroMar [i + j * width] = colorMap [i + j * width];
						newTypeMap [i + j * width] = typeMap [i + j * width];
					}

				}
			}

			colorMap = newColroMar;
			typeMap = newTypeMap;
			height = value;
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

	// ========================
}
