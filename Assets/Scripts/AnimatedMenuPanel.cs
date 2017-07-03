using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class AnimatedMenuPanel  : MenuPanel {

	protected Animator animator;

	protected virtual void Awake () {
		GetAnimator ();
	}

	protected void GetAnimator () {
		if (!animator)
			animator = GetComponent <Animator> ();
	}

	public override void OpenPanel () {
		gameObject.SetActive (true);
		GetAnimator ();
		animator.SetBool ("Appear", true);
	}

	public override void ClosePanel () {
		animator.SetBool ("Appear", false);
		OnClosed.AddListener (delegate {
			gameObject.SetActive (false);	
			OnClosed.RemoveAllListeners ();
		});
	}

	public void OnClosedInvoke () {
		OnClosed.Invoke ();
	}

	public void OnOpenedInvoke () {
		OnOpened.Invoke ();
	}
}