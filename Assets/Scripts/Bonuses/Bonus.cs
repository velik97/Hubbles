using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Base class for bonus
/// </summary>
/// <typeparam name="T">Type of bonus to follow singleton pattern</typeparam>
public abstract class Bonus <T> : MonoSingleton <T> where T : MonoBehaviour
{

	private int count = -1;

	public Text countText;
	public GameObject blockImage;

	void Awake () {
		blockImage.SetActive (Available);
		countText.text = Available ? Count.ToString () : "";
	}

	public int Count {
		get {
			if (count == -1) {
				if (PlayerPrefs.HasKey (typeof(T).ToString () + " count")) {
					count = PlayerPrefs.GetInt (typeof(T).ToString () + " count");
				} else {
					PlayerPrefs.SetInt (typeof(T).ToString () + " count", 0);
					count = 0;
				}
			}

			return count;
		}
		set {
			count = value;
			PlayerPrefs.SetInt (typeof(T).ToString () + " Count", value);
		}
	}

	public bool Available {
		get {
			bool available = false;
			if (PlayerPrefs.HasKey (typeof(T).ToString () + " is available")) {
				available = PlayerPrefs.GetInt (typeof(T).ToString () + " is available") == 1;
			} else {
				PlayerPrefs.SetInt (typeof(T).ToString () + " is available", 0);
			}
			return available;
		}

		set {
			PlayerPrefs.SetInt (typeof(T).ToString () + " is available", value ? 1 : 0);
		}
	}

	public int coast;

	public virtual void Apply (Coord coordToApply) {
		count--;
	}

}
