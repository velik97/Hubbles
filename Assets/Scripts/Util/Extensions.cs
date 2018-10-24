using System;
using System.Collections;
using UnityEngine;

public static class Extensions
{
	/// <summary>
	/// Sets status of <c>IStatusGraphics</c> object with specific integer value
	/// </summary>
	/// <param name="graphics">graphics to be set</param>
	/// <param name="count">integer value</param>
	/// <param name="max">max integer value</param>
	public static void SetStatus(this IStatusGraphics graphics, int count, int max) {
		graphics.SetStatus(((float)count) / ((float)max));
	}

	/// <summary>
	/// Invokes some method with given delay
	/// </summary>
	/// <param name="monoBehaviour">method will be invoked on this</param>
	/// <param name="action">method to be invoked</param>
	/// <param name="delay">delay before method will be invoked</param>
	public static Coroutine InvokeWithDelay(this MonoBehaviour monoBehaviour, Action action, float delay)
	{
		return monoBehaviour.StartCoroutine(InvokeWithDelayCoroutine(action, delay));
	}


	/// <summary>
	/// Invoke action straight after condition in triggerFunc is met
	/// </summary>
	/// <param name="monoBehaviour">method will be invoked on this</param>
	/// <param name="action">action to be invoked after condition is met</param>
	/// <param name="triggerFunc">condition is met when trigger func returns true</param>
	public static Coroutine InvokeAfter(this MonoBehaviour monoBehaviour, Action action, Func<bool> triggerFunc)
	{
		return monoBehaviour.StartCoroutine(InvokeAfterCoroutine(action, triggerFunc));
	} 
	
	/// <summary>
	/// Plays animation in a coroutine giving animationAction params from 0 to 1
	/// </summary>
	/// <param name="monoBehaviour">method will be invoked on this</param>
	/// <param name="animationAction">action to be done each frame</param>
	/// <param name="duration">duration of action</param>
	public static Coroutine PlayAnimation(this MonoBehaviour monoBehaviour, Action<float> animationAction, float duration)
	{
		return monoBehaviour.StartCoroutine(PlayAnimationCoroutine(animationAction, duration,
			par => par));
	}
	
	/// <summary>
	/// Plays animation in a coroutine giving animationAction params from 0 to 1 with specific ease func
	/// </summary>
	/// <param name="monoBehaviour">method will be invoked on this</param>
	/// <param name="animationAction">action to be done each frame</param>
	/// <param name="duration">duration of action</param>
	/// <param name="easeCurve">ease function</param>
	public static Coroutine PlayAnimation(this MonoBehaviour monoBehaviour, Action<float> animationAction, float duration,
		AnimationF.GeneralEaseFunc easeCurve)
	{
		return monoBehaviour.StartCoroutine(PlayAnimationCoroutine(animationAction, duration, easeCurve));
	}

	/// <summary>
	/// Plays animation in a coroutine giving animationAction params from 0 to 1 with specific ease func
	/// </summary>
	/// <param name="monoBehaviour">method will be invoked on this</param>
	/// <param name="animationAction">action to be done each frame</param>
	/// <param name="duration">duration of action</param>
	/// <param name="easeCurve">ease function</param>
	/// <param name="pow">pow of ease function</param>
	public static Coroutine PlayAnimation(this MonoBehaviour monoBehaviour, Action<float> animationAction, float duration,
		AnimationF.InOutEaseFunc easeCurve, int pow = 2)
	{
		return monoBehaviour.StartCoroutine(PlayAnimationCoroutine(animationAction, duration,
			par => easeCurve(par, pow)));
	}

	/// <summary>
	/// Plays animation in a coroutine giving animationAction params from 0 to 1 with specific ease func
	/// </summary>
	/// <param name="monoBehaviour">method will be invoked on this</param>
	/// <param name="animationAction">action to be done each frame</param>
	/// <param name="duration">duration of action</param>
	/// <param name="easeCurve">ease function</param>
	/// <param name="percentage">percent of ease function</param>
	/// <param name="pow">pow of ease function</param>
	public static Coroutine PlayAnimation(this MonoBehaviour monoBehaviour, Action<float> animationAction, float duration,
		AnimationF.BetweenEaseFunc easeCurve, float percentage, int pow = 2)
	{
		return monoBehaviour.StartCoroutine(PlayAnimationCoroutine(animationAction, duration,
			par => easeCurve(par, percentage, pow)));
	}
	
	/// <summary>
	/// Plays animation in a coroutine giving animationAction params from 0 to 1 with specific ease func
	/// </summary>
	/// <param name="monoBehaviour">method will be invoked on this</param>
	/// <param name="animationAction">action to be done each frame</param>
	/// <param name="duration">duration of action</param>
	/// <param name="easeCurve">ease function</param>
	public static Coroutine PlayAnimation(this MonoBehaviour monoBehaviour, Action<float> animationAction, float duration,
		AnimationCurve easeCurve)
	{
		return monoBehaviour.StartCoroutine(PlayAnimationCoroutine(animationAction, duration,
			easeCurve.Evaluate));
	}

	/// <summary>
	/// Returns duration of animation with given name of given animator   
	/// </summary>
	/// <param name="animator">animator, that has animation on it</param>
	/// <param name="animationName">name of required animation</param>
	public static float AnimationDuration(this Animator animator, string animationName)
	{
		float duration = 0f;
		RuntimeAnimatorController ac = animator.runtimeAnimatorController;
		foreach (var clip in ac.animationClips)
		{
			if (clip.name.Contains(animationName))
			{
				duration = clip.length;
				break;
			}
		}
		return duration;
	} 
	
	private static IEnumerator InvokeAfterCoroutine(Action action, Func<bool> triggerFunc)
	{
		yield return new WaitUntil(triggerFunc);
		action.Invoke();
	}
	
	private static IEnumerator InvokeWithDelayCoroutine(Action action, float delay)
	{
		yield return new WaitForSeconds(delay);
		action.Invoke();
	}

	private static IEnumerator PlayAnimationCoroutine(Action<float> animationAction, float duration,
		AnimationF.GeneralEaseFunc easeFunc)
	{
		float t = 0f;
		float startTime = Time.time;

		while ((t = (Time.time - startTime)/duration) < 1f)
		{
			animationAction.Invoke(easeFunc(t));
			yield return null;
		}
		
		animationAction.Invoke(easeFunc(1f));
		yield return null;
	}
}
