using System;
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
	/// Text in circle
	/// </summary>
	public GameObject textObject;
	/// <summary>
	/// Sprite Renderer in circle
	/// </summary>
	public GameObject srObject;
	/// <summary>
	/// Content in circle whether it's a text or a sprite
	/// </summary>
	public GameObject content;
	/// <summary>
	/// GamePanelAnimator of the hubble
	/// </summary>
	public Animator animator;
	/// <summary>
	/// Sound of highlight
	/// </summary>
	public AudioClip popSound;

	private void Awake()
	{
		transform.SetParent (MapGenerator.Instance.transform);
	}

	/// <summary>
	/// Set hubble visual according to params
	/// </summary>
	/// <param name="color">color of hubble</param>
	/// <param name="type">type of hubble</param>
	/// <param name="prevType">prev type of hubble</param>
	/// <param name="points">points of hubble</param>
	public void Set (int color, HubbleType type, HubbleType prevType, int points)
	{
		if (type == HubbleType.Usual ^ prevType == HubbleType.Usual)
		{
			if (content != null)
				content.SetActive(false);
			content = type == HubbleType.Usual ? textObject : srObject;
			if (content != null)
				content.SetActive(true);
		}
			
		bool contentWasInstantiated = false;
		if (type == HubbleType.Usual && text == null)
		{
			textObject = Instantiate
				(HubblesAppearanceInfo.Instance.textPrefab, transform.position, Quaternion.identity);
			content = textObject;
			text = content.GetComponentInChildren <Text> ();
			contentWasInstantiated = true;
		}
		else if (type != HubbleType.Usual && sr == null)
		{
			srObject = Instantiate
				(HubblesAppearanceInfo.Instance.imagePrefab, transform.position, Quaternion.identity);
			content = srObject;
			sr = content.GetComponentInChildren <SpriteRenderer> ();
			contentWasInstantiated = true;
		}

		if (contentWasInstantiated)
		{
			content.transform.SetParent (mainCircle.transform, true);
			content.transform.localPosition = Vector3.zero;
			content.transform.localScale = Vector3.one;
		}

		mainCircle.GetComponent <SpriteRenderer> ().color = HubblesAppearanceInfo.Instance.UsualColors [color];
		highLight.GetComponent <SpriteRenderer> ().color = HubblesAppearanceInfo.Instance.LightColors [color];

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
			text.text = points.ToString ();
			text.color = HubblesAppearanceInfo.Instance.DarkColors [color];
		} else {
			switch (type)
			{
				case HubbleType.PopLive:
					sr.sprite = HubblesAppearanceInfo.Instance.popLiveSprite;
					break;
				case HubbleType.RotationLive:
					sr.sprite = HubblesAppearanceInfo.Instance.rotationLiveSprite;
					break;
				case HubbleType.Multiplier:
					sr.sprite = HubblesAppearanceInfo.Instance.multiplierSprite;
					break;
			}
			sr.color = HubblesAppearanceInfo.Instance.DarkColors [color];
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
