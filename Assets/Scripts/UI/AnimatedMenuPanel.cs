using UnityEngine;
using System.Collections;

/// <summary>
/// Menu panel that closes or opens with animations
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimatedMenuPanel  : MenuPanel {

	protected Animator anim;
	private float disappearAnimationLength = 0f;
	private float DisappearAnimationLength
	{
		get
		{
			if (disappearAnimationLength == 0f)
			{
				RuntimeAnimatorController ac = Anim.runtimeAnimatorController;
				foreach (var clip in ac.animationClips)
				{
					if (clip.name.Contains("Disappear"))
					{
						disappearAnimationLength = clip.length;
						break;
					}
				}
			}

			return disappearAnimationLength;
		}
	}

	public Animator Anim
	{
		get
		{
			if (anim == null)
				anim = GetComponent <Animator> ();
			return anim;
		}
	}

	public override void OpenPanel () {
		gameObject.SetActive (true);
		Anim.SetTrigger("Appear");
		onOpened.Invoke();
	}

	public override void ClosePanel () {
		Anim.SetTrigger("Disappear");
		onClosed.Invoke();
		
		if (deactivateOnClose)
			this.InvokeWithDelay(() =>
			{
				gameObject.SetActive(false);
			}, DisappearAnimationLength);
	}

	
}