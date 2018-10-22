using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// Animates scene loading
/// </summary>
public class SceneLoader : AnimatedMenuPanel {

	public UnityEvent OnLoaded;

	public GameObject statusGraphicsObj;
	private IStatusGraphics statusGraphics;

	public float middleProgressPoint;
	private float startProgressPoint;
	private float endProgressPoint;

	public float timeToProccess;

	public IStatusGraphics StatusGraphics
	{
		get
		{
			if (statusGraphics == null)
				statusGraphics = statusGraphicsObj.GetComponent<IStatusGraphics>();
			return statusGraphics;
		}
	}
	
	protected void Awake () {
		EndLoadingScene ();
	}

	public void StartLoadingScene (int sceneIndex) {
		startProgressPoint = 0f;
		endProgressPoint = middleProgressPoint;
		StatusGraphics.SetStatus (startProgressPoint);

		onOpened.AddListener (delegate {
			StartCoroutine (LoadCoroutine ());	
		});
		OnLoaded.AddListener (delegate {
			SceneManager.LoadScene (sceneIndex);
		});
		OpenPanel ();
	}

	public void EndLoadingScene () {
		startProgressPoint = middleProgressPoint;
		endProgressPoint = 1f;
		StatusGraphics.SetStatus (startProgressPoint);
		gameObject.SetActive (true);

		StartCoroutine (LoadCoroutine ());
	}

	IEnumerator LoadCoroutine () {
		for (float timeOffset = 0f; timeOffset < timeToProccess; timeOffset += Time.deltaTime) {
			StatusGraphics.SetStatus (startProgressPoint + (endProgressPoint - startProgressPoint) * (timeOffset / timeToProccess));
			yield return null;
		}

		yield return null;

		OnLoaded.Invoke ();
		ClosePanel();
	}

}
