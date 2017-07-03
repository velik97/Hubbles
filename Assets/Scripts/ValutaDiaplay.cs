using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ValutaDiaplay : MonoSingleton <ValutaDiaplay> {

	public Text coinsCount;

	void Start () {
//		ValutaManager.Instance.Valuta = 0;
		SetCount ();
	}

	public void SetCount () {
		coinsCount.text = ValutaManager.Instance.Valuta.ToString ();
	}

}
