using UnityEngine;
using System.Collections;

public class SimpleMenuPanel : MenuPanel {

	public override void OpenPanel () {
		gameObject.SetActive (true);
		onOpened.Invoke ();
		onOpened.RemoveAllListeners ();
	}

	public override void ClosePanel () {
		gameObject.SetActive (false);
		onClosed.Invoke ();
		onClosed.RemoveAllListeners ();
	}
}
