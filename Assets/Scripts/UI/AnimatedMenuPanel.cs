using UnityEngine;
using System.Collections;

/// <summary>
/// Menu panel that closes or opens with animations
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimatedMenuPanel  : MenuPanel {

	protected Animator animator;
	private float disappearAnimationLength = 0f;

	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
	}

	private float DisappearAnimationLength
	{
		get
		{
			if (disappearAnimationLength == 0f)
			{
				disappearAnimationLength = animator.AnimationDuration("Disappear");
			}

			return disappearAnimationLength;
		}
	}

	public override void OpenPanel () {
		gameObject.SetActive (true);
		animator.SetTrigger("Appear");
		onOpened.Invoke();
	}

	public override void ClosePanel () {
		animator.SetTrigger("Disappear");
		onClosed.Invoke();
		
		if (deactivateOnClose)
			this.InvokeWithDelay(() =>
			{
				gameObject.SetActive(false);
			}, DisappearAnimationLength);
	}

	
}