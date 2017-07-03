using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class PaidStuff : MonoBehaviour {

	public enum ValutaType {
		LocalValuta,
		Dollars
	}

	public UnityEvent OnBuySuccess;
	public UnityEvent OnBuyNotSuccess;

	public ValutaType valutaType;
	public float cost;
	public Text costText;

	protected virtual void Awake () {
		SetCostText (cost);
	}

	public void SetCostText (float cost) {
		if (costText)
			costText.text = cost.ToString ();
	}

	public void TryBuy () {
		if (valutaType == ValutaType.LocalValuta) {
			if (ValutaManager.Instance.Valuta >= Mathf.RoundToInt (cost)) {
				ValutaManager.Instance.Valuta -= Mathf.RoundToInt (cost);
				if (ValutaDiaplay.Instance)
					ValutaDiaplay.Instance.SetCount ();
				OnBuySuccess.Invoke ();
			} else {
				OnBuyNotSuccess.Invoke ();
			}
		} else {
			OnBuySuccess.Invoke ();
			Debug.Log ("real monetization isn't set yet");
		}
	}

}
