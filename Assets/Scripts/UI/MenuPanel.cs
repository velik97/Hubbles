using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Menu panel, that can be opened or closed
/// </summary>
public abstract class MenuPanel : MonoBehaviour
{
	public bool deactivateOnClose = true;
	
	public UnityEvent onClosed;
	public UnityEvent onOpened;

	public abstract void OpenPanel ();

	public abstract void ClosePanel ();
}
