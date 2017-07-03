using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public abstract class MenuPanel : MonoBehaviour {

	public UnityEvent OnClosed;
	public UnityEvent OnOpened;

	public abstract void OpenPanel ();

	public abstract void ClosePanel ();
}
