using System;
using System.Collections;
using UnityEngine;

public static class Extensions {

	public static void SetStatus(this IStatusGraphics graphics, int count, int max) {
		graphics.SetStatus(((float)count) / ((float)max));
	}

	public static void InvokeWithDelay(this MonoBehaviour monoBehaviour, Action action, float delay)
	{
		monoBehaviour.StartCoroutine(InvokeWithDelayCoroutine(action, delay));
	}

	private static IEnumerator InvokeWithDelayCoroutine(Action action, float delay)
	{
		yield return new WaitForSeconds(delay);
		action.Invoke();
	}
}
