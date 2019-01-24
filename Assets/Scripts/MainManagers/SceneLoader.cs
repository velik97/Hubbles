using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// Animates scene loading
/// </summary>
public class SceneLoader : AnimatedMenuPanel {

	public UnityEvent onLoaded;

	private bool isOpened = true;

	private IStatusGraphics statusGraphics;

	private IStatusGraphics StatusGraphics
	{
		get
		{
			if (statusGraphics == null)
				statusGraphics = GetComponentInChildren<IStatusGraphics>();
			return statusGraphics;
		}
	}

	public float middleProgressPoint;
	private float startProgressPoint;
	private float endProgressPoint;

	public float timeToProcess;

	public void StartLoadingScene (string sceneName) {
		startProgressPoint = 0f;
		endProgressPoint = middleProgressPoint;
		StatusGraphics.SetStatus(startProgressPoint);

		onLoaded.AddListener(() =>
		{
			SceneManager.LoadScene(sceneName);
		});
		if (isOpened)
		{
			gameObject.SetActive(true);
			StartCoroutine(LoadCoroutine());
		}
		else
		{
			onOpened.AddListener(() =>
			{
				isOpened = true;
				gameObject.SetActive(true);
				StartCoroutine(LoadCoroutine());	
			});
			OpenPanel();	
		}
	}

	public void EndLoadingScene() {
		startProgressPoint = middleProgressPoint;
		endProgressPoint = 1f;
		StatusGraphics.SetStatus(startProgressPoint);
		gameObject.SetActive(true);

		StartCoroutine(LoadCoroutine());
		onClosed.AddListener(() => isOpened = false);
	}

	private IEnumerator LoadCoroutine() {
		for (float timeOffset = 0f; timeOffset < timeToProcess; timeOffset += Time.deltaTime) {
			StatusGraphics.SetStatus(startProgressPoint + (endProgressPoint - startProgressPoint) * (timeOffset / timeToProcess));
			yield return null;
		}

		yield return null;
		
		onLoaded.Invoke();
		if (endProgressPoint < 1f)
			yield break;
		ClosePanel();
	}

}
