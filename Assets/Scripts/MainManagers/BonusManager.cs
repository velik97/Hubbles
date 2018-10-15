using UnityEngine;
using System.Collections;

/// <summary>
/// Manages bonus use
/// </summary>
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

		TouchManager.Instance.RemoveAllListeners ();
		TouchManager.Instance.touchState.AddListener(SubscribeOnTouch);

	}
	
	private void SubscribeOnTouch (TouchState touchState)
	{
		if (touchState != TouchState.EndedTouching && touchState != TouchState.EndedRotating)
			return;
		switch (currentType)
		{
			case BonusType.Paint:
				PaintBonus.Instance.Apply(TouchManager.Instance.startTouchCoord);
				break;
			case BonusType.Connect:
				Debug.Log("Connect Bonus isn't ready yet");
				break;
		}

		EndBonusActivity();
	}

	public void EndBonusActivity () {
		cancelPanel.ClosePanel ();
		HubblesManager.Instance.SubscribeOnTouchEvents ();
	}
}
