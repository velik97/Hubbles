using UnityEngine;
using System.Collections;

public class SimpleMenuPanel : MenuPanel {

	public override void OpenPanel () {
		gameObject.SetActive (true);
		OnOpened.Invoke ();
		OnOpened.RemoveAllListeners ();
	}

	public override void ClosePanel () {
		gameObject.SetActive (false);
		OnClosed.Invoke ();
		OnClosed.RemoveAllListeners ();
	}
}
