using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class LivesDisplay : MonoSingleton <LivesDisplay> {

	public Text livesCountText;
	public Text timeToNextLiveAddText;

	public Button button;
	public MenuPanel livePackPanel;

	[Space(10)]
	public StatusBar statusBar;

	void Awake () {
		button.onClick.AddListener (delegate {
			if (!LivesManager.Instance.LivesAreFull)
				MenuManager.Instance.OpenMenuPanel (livePackPanel);
		});

//		LivesManager.Instance.TakeLive ();
//		ValutaManager.Instance.Valuta = 0;

		UpdateDisplay();
		InvokeRepeating ("UpdateDisplay", 0f, 1f);
	}

	public void UpdateDisplay () {
		livesCountText.text = LivesManager.Instance.CurrentLivesCount.ToString ();

		if (!LivesManager.Instance.LivesAreFull) {
			timeToNextLiveAddText.text = LivesManager.Instance.TimeToNextLiveAdd.MinutesToString ();
			statusBar.SetPersentage (LivesManager.Instance.TimeScinceLastLiveAdd.ToInt (), LivesManager.Instance.minutesBetweenLivesAdding * 60);
		} else {
			timeToNextLiveAddText.text = "";
			statusBar.SetPersentage (1);
		}
	}
}
