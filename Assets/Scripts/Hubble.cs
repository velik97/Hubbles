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
	public Text textRef;
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
	/// Audio source for sounds. Need change to audio manager
	/// </summary>
	public AudioSource sound;

	/// <summary>
	/// Set hubble visual according to params
	/// </summary>
	/// <param name="color">color of hubble</param>
	/// <param name="type">type of hubble</param>
	/// <param name="prevType">prev type of hubble</param>
	/// <param name="points">points of hubble</param>
	public void Set (int color, int type, int prevType, int points) {

		bool deleteContent;
		bool instantiateContent;

		if (prevType == 0) {
			deleteContent = false;
			instantiateContent = true;
		} else if ((type == 1 && prevType == 1) || ((type == -1 || type == 2) && (prevType == -1 || prevType == 2))) {
			deleteContent = false;
			instantiateContent = false;
		} else {
			deleteContent = true;
			instantiateContent = true;
		}

		if (deleteContent)
			Destroy (content);

		if (instantiateContent) {
			if (type == 1) {
				GameObject heart = Instantiate (CommonInfo.Instance.heartPrefab, transform.position, Quaternion.identity) as GameObject;
				heart.transform.SetParent (mainCircle.transform, true);
				content = heart;
				content.transform.localPosition = Vector3.zero;
				heart.transform.localScale = Vector3.one;
				sr = content.GetComponent <SpriteRenderer> ();
			} else if (type == -1 || type == 2) {
				GameObject text = Instantiate (CommonInfo.Instance.textPrefab, transform.position, Quaternion.identity) as GameObject;
				text.transform.SetParent (mainCircle.transform, true);
				content = text;
				content.transform.localPosition = Vector3.zero;
				text.transform.localScale = Vector3.one * .007f;
				textRef = text.GetComponentInChildren <Text> ();
			}
		}

		mainCircle.GetComponent <SpriteRenderer> ().color = CommonInfo.Instance.usualColors [color];
		highLight.GetComponent <SpriteRenderer> ().color = CommonInfo.Instance.lightColors [color];

		transform.SetParent (MapGenerator.Instance.transform);

		StartCoroutine (AdjustColorAndTypeWithDelay (color, type, points));
	}

	/// <summary>
	/// Set text and color delayed
	/// </summary>
	/// <param name="color">color of hubble</param>
	/// <param name="type">type of hubble</param>
	/// <param name="points">points of hubble</param>
	IEnumerator AdjustColorAndTypeWithDelay (int color, int type, int points) {
		yield return new WaitForSeconds (points == 1 ? .01f : .2f);
		if (type == -1 || type == 2) {
			textRef.color = CommonInfo.Instance.darkColors [color];
			if (type == -1)
				textRef.text = points.ToString ();
			else 
				textRef.text = "x2";
		} else if (type == 1) {
			sr.color = CommonInfo.Instance.darkColors [color];
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
		sound.pitch = Mathf.Log(pitch);
		sound.PlayDelayed (.1f);
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
