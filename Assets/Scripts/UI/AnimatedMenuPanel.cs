using UnityEngine;
using System.Collections;

/// <summary>
/// Menu panel that closes or opens with animations
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimatedMenuPanel  : MenuPanel
{
	
	private Animator animator;
	private float disappearAnimationLength = 0f;

	private Animator Animator
	{
		get
		{
			if (animator == null)
				animator = GetComponentInChildren<Animator>();
			return animator;
		}
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
		Animator.SetTrigger("Appear");
		onOpened.Invoke();
	}

	public override void ClosePanel () {
		Animator.SetTrigger("Disappear");
		onClosed.Invoke();
		
		if (deactivateOnClose)
			this.InvokeWithDelay(() =>
			{
				gameObject.SetActive(false);
			}, DisappearAnimationLength);
	}

	
}