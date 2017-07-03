using UnityEngine;
using System.Collections;

public class BonusManager : MonoBehaviour {

	public MenuPanel cancelPanel;

	public enum BonusType {
		Paint,
		Connect
	}

	BonusType currentType;

	public void ApplyBonus (BonusType type) {
		currentType = type;

		cancelPanel.OpenPanel ();

		TouchManager.Instance.FreeListeners ();
		TouchManager.Instance.OnTouchEnd.AddListener (delegate {
			switch (currentType) {
				case BonusType.Paint:
					PaintBonus.Instance.Apply (TouchManager.Instance.startTouchCoord);
					break;
				case BonusType.Connect:
					Debug.Log ("Connect Bonus isn't ready yet");
					break;
			}
			EndBonusActivity ();
		});
	}

	public void EndBonusActivity () {
		cancelPanel.ClosePanel ();
		HubblesManager.Instance.SubscribeOnTouchEvents ();
	}
}
