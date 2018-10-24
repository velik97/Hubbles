using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel, that highlights, when all hubbles of certain color are highlighted
/// </summary>
public class HighlightPanel : MonoSingleton<HighlightPanel> {

	[SerializeField] private Image frameImage;
	[SerializeField] private Image innerImage;

	[SerializeField] private float setTime;
	[SerializeField] private float innerLightness;

	private Coroutine currentCoroutine;

	private Color startInnerColor;
	private Color startFrameColor;
	private Color frameColor;
	private Color innerColor;

	private bool highLighted = false;

	public void Highlight(Color color) {
		highLighted = true;
		if (currentCoroutine != null)
			StopCoroutine (currentCoroutine);

		startFrameColor = new Color(color.r, color.g, color.b, 0f);
		startInnerColor = startFrameColor;
		
		frameColor = new Color(color.r, color.g, color.b, 1f);
		innerColor = new Color(color.r, color.g, color.b, innerLightness);

		currentCoroutine = this.PlayAnimation(SetColor, setTime);
	}

	public void Unhighlight()
	{
		if (!highLighted)
			return;
		highLighted = false;
		
		if (currentCoroutine != null)
			StopCoroutine (currentCoroutine);

		startFrameColor = frameImage.color;
		startInnerColor = innerImage.color;
		
		frameColor = new Color(startFrameColor.r, startFrameColor.g, startFrameColor.b, 0f);
		innerColor = frameColor;

		currentCoroutine = this.PlayAnimation(SetColor, setTime);
	}

	private void SetColor(float t)
	{
		frameImage.color = Color.Lerp(startFrameColor, frameColor, t);
		
		innerImage.color = Color.Lerp(startInnerColor, innerColor, t);
	}
}
