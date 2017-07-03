using UnityEngine;
using System.Collections;

public class ValutaManager : MonoSingleton <ValutaManager> {

	private int valuta = -1;
	public int startValuta;

	public int Valuta {
		get {
			if (valuta < 0) {
				if (PlayerPrefs.HasKey ("Valuta")) {
					valuta = PlayerPrefs.GetInt ("Valuta");
				} else {
					PlayerPrefs.SetInt ("Valuta", startValuta);
					valuta = startValuta;
				}
			}
			return valuta;
		}
		set {
			PlayerPrefs.SetInt ("Valuta", value);
			valuta = value;
		}
	}

	public void TakeValuta (int cost) {
		Valuta -= cost;
		if (ValutaDiaplay.Instance)
			ValutaDiaplay.Instance.SetCount ();
	}

	public void AddValuta (int cost) {
		Valuta += cost;
		if (ValutaDiaplay.Instance)
			ValutaDiaplay.Instance.SetCount ();
	}

}
