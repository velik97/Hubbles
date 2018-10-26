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
	[SerializeField] private SpriteRenderer mainCircle;
	/// <summary>
	/// Highlight circle
	/// </summary>
	[SerializeField] private SpriteRenderer highlight;
	/// <summary>
	/// Text in circle for points or bonuses
	/// </summary>
	[SerializeField] private Text text;
	/// <summary>
	/// Text in circle
	/// </summary>
	[SerializeField] private GameObject textObject;
	/// <summary>
	/// Object of multiplier power up
	/// </summary>
	[SerializeField] private GameObject multiplierObj;
	/// <summary>
	/// Object of pop live power up
	/// </summary>
	[SerializeField] private GameObject popLiveObj;
	/// <summary>
	/// Object of rot live power up
	/// </summary>
	[SerializeField] private GameObject rotLiveObj;
	/// <summary>
	/// Content in circle whether it's a text or a sprite
	/// </summary>
	[HideInInspector]
	public GameObject content;
	/// <summary>
	/// GamePanelAnimator of the hubble
	/// </summary>
	[SerializeField] private Animator animator;
	/// <summary>
	/// Sound of highlight
	/// </summary>
	[SerializeField] private AudioClip popSound;

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
	public void Set (int color, HubbleType type, int points)
	{
		StartCoroutine (AdjustColorAndTypeWithDelay (color, type, points));
	}

	/// <summary>
	/// Set text and color delayed
	/// </summary>
	/// <param name="color">color of hubble</param>
	/// <param name="type">type of hubble</param>
	/// <param name="points">points of hubble</param>
	IEnumerator AdjustColorAndTypeWithDelay (int color, HubbleType type, int points) {
		yield return new WaitForSeconds (.01f);
		
		if (content != null)
			content.SetActive(false);
		switch (type)
		{
			case HubbleType.Usual:
				content = textObject;
				break;
			case HubbleType.PopLive:
				content = popLiveObj;
				break;
			case HubbleType.RotationLive:
				content = rotLiveObj;
				break;
			case HubbleType.Multiplier:
				content = multiplierObj;
				break;
		}
		if (content != null)
			content.SetActive(true);

		mainCircle.color = HubblesAppearanceInfo.Instance.UsualColors [color];
		highlight.color = HubblesAppearanceInfo.Instance.LightColors [color];

		if (type == HubbleType.Usual)
		{
			text.text = points.ToString();
			text.color = HubblesAppearanceInfo.Instance.DarkColors[color];
		}
		else
		{
			foreach (var sr in content.GetComponentsInChildren<SpriteRenderer>())
			{
				sr.color = HubblesAppearanceInfo.Instance.DarkColors[color];
			}
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
