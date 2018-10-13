using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Visualization of actual hubble
/// </summary>
public class Hubble : MonoBehaviour{
	
	/// <summary>
	/// Circle in the middle
	/// </summary>
	public GameObject mainCircle;
	/// <summary>
	/// Highlight circle
	/// </summary>
	public GameObject highLight;
	/// <summary>
	/// Text in circle for points or bonuses
	/// </summary>
	public Text text;
	/// <summary>
	/// Sprite  in circle for bonuses
	/// </summary>
	public SpriteRenderer sr;
	/// <summary>
	/// Content in circle whether it's a text or a sprite
	/// </summary>
	public GameObject content;
	/// <summary>
	/// Animator of the hubble
	/// </summary>
	public Animator animator;
	/// <summary>
	/// Sound of highlight
	/// </summary>
	public AudioClip popSound;

	/// <summary>
	/// Set hubble visual according to params
	/// </summary>
	/// <param name="color">color of hubble</param>
	/// <param name="type">type of hubble</param>
	/// <param name="prevType">prev type of hubble</param>
	/// <param name="points">points of hubble</param>
	public void Set (int color, HubbleType type, HubbleType prevType, int points) {

		bool deleteContent;
		bool instantiateContent;

		if ((type == HubbleType.Usual && prevType == HubbleType.Usual) ||
		    (type != HubbleType.Usual && prevType != HubbleType.Usual)) {
			deleteContent = false;
			instantiateContent = false;
		} else {
			deleteContent = true;
			instantiateContent = true;
		}

		if (deleteContent)
			Destroy (content);

		if (instantiateContent) {
			if (type == HubbleType.Usual) {
				GameObject textPrefab = Instantiate (HubblesAppearanceInfo.Instance.textPrefab, transform.position, Quaternion.identity) as GameObject;
				textPrefab.transform.SetParent (mainCircle.transform, true);
				content = textPrefab;
				content.transform.localPosition = Vector3.zero;
				text = textPrefab.GetComponentInChildren <Text> ();
			} else {
				GameObject imagePrefab = Instantiate (HubblesAppearanceInfo.Instance.imagePrefab, transform.position, Quaternion.identity) as GameObject;
				imagePrefab.transform.SetParent (mainCircle.transform, true);
				content = imagePrefab;
				content.transform.localPosition = Vector3.zero;
				imagePrefab.transform.localScale = Vector3.one;
				sr = content.GetComponent <SpriteRenderer> ();
			}
		}

		mainCircle.GetComponent <SpriteRenderer> ().color = HubblesAppearanceInfo.Instance.usualColors [color];
		highLight.GetComponent <SpriteRenderer> ().color = HubblesAppearanceInfo.Instance.lightColors [color];

		transform.SetParent (MapGenerator.Instance.transform);

		StartCoroutine (AdjustColorAndTypeWithDelay (color, type, points));
	}
	
	public void Set(int color, HubbleType type, int points)
	{
		Set(color, type, HubbleType.Usual, points);
	}

	/// <summary>
	/// Set text and color delayed
	/// </summary>
	/// <param name="color">color of hubble</param>
	/// <param name="type">type of hubble</param>
	/// <param name="points">points of hubble</param>
	IEnumerator AdjustColorAndTypeWithDelay (int color, HubbleType type, int points) {
		yield return new WaitForSeconds (points == 1 ? .01f : .2f);
		if (type == HubbleType.Usual) {
			text.color = HubblesAppearanceInfo.Instance.darkColors [color];
				text.text = points.ToString ();
		} else {
			sr.color = HubblesAppearanceInfo.Instance.darkColors [color];
		}
	}

	/// <summary>
	/// Play animation for disappear
	/// </summary>
	public void DisAppear () {
		animator.SetBool ("Appear", false);
	}

	/// <summary>
	/// Play animation for appear
	/// </summary>
	public void Appear () {
		animator.SetBool ("Appear", true);
	}

	/// <summary>
	/// Play animation for highlight
	/// </summary>
	public void Highlight (float pitch) {
		animator.SetBool ("Highlight", true);
		SoundManager.Instance.Play(popSound, Mathf.Log(pitch), .1f);
	}

	/// <summary>
	/// Play animation for unhighlight
	/// </summary>
	public void UnHighlight () {
		animator.SetBool ("Highlight", false);
	}

	/// <summary>
	/// Play animation for change
	/// </summary>
	public void Change () {
		animator.SetTrigger ("Change");
	}

}
