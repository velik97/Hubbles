using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// Animates scene loading
/// </summary>
public class SceneLoader : AnimatedMenuPanel {

	public UnityEvent OnLoaded;

	public StatusBar loadProgressBar;

	public float middleProgressPoint;
	private float startProgressPoint;
	private float endProgressPoint;

	public float timeToProccess;

	protected override void Awake () {
		base.Awake ();
		EndLoadingScene ();
	}

	public override void OpenPanel () {
		base.OpenPanel ();
		animator.SetBool ("StartAnimation", true);
	}

	public override void ClosePanel () {
		base.ClosePanel ();
		animator.SetBool ("StartAnimation", true);
	}

	public void StartLoadingScene (int sceneIndex) {
		startProgressPoint = 0f;
		endProgressPoint = middleProgressPoint;
		loadProgressBar.SetPercentage (startProgressPoint);

		OnOpened.AddListener (delegate {
			StartCoroutine (ILoad ());	
		});
		OnLoaded.AddListener (delegate {
			SceneManager.LoadScene (sceneIndex);
		});
		OpenPanel ();
	}

	public void EndLoadingScene () {
		startProgressPoint = middleProgressPoint;
		endProgressPoint = 1f;
		loadProgressBar.SetPercentage (startProgressPoint);
		gameObject.SetActive (true);

		OnLoaded.AddListener (ClosePanel);
		StartCoroutine (ILoad ());
	}

	IEnumerator ILoad () {
		for (float timeOfsset = 0f; timeOfsset < timeToProccess; timeOfsset += Time.deltaTime) {
			loadProgressBar.SetPercentage (startProgressPoint + (endProgressPoint - startProgressPoint) * (timeOfsset / timeToProccess));
			yield return null;
		}

		yield return null;

		OnLoaded.Invoke ();
	}

}
