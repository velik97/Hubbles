using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoMenuPanel : AnimatedMenuPanel {

	public Text levelNumText;
	public Text startLivesText;

	public UIAimsHolder aimsHolder;

	public override void OpenPanel () {
		base.OpenPanel ();
		levelNumText.text = LevelConfigHandler.CurrentIndex.ToString ();
		startLivesText.text = LevelConfigHandler.CurrentConfig.startLives.ToString ();
		aimsHolder.GenerateAims ();
	}
}
