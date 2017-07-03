using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Hubble : MonoBehaviour{
	public GameObject mainCircle;
	public GameObject highLight;
	public Text textRef;
	public SpriteRenderer sr;
	public GameObject content;
	public Animator animator;
	public AudioSource sound;

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

	public void DisAppear () {
		animator.SetBool ("Appear", false);
	}

	public void Appear () {
		animator.SetBool ("Appear", true);
	}

	public void Highlight (float pitch) {
		animator.SetBool ("Highlight", true);
		sound.pitch = Mathf.Log(pitch);
		sound.PlayDelayed (.1f);
	}

	public void UnHighlight () {
		animator.SetBool ("Highlight", false);
	}

	public void Change () {
		animator.SetTrigger ("Change");
	}

}
