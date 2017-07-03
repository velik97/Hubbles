using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InMenuInfoPanel : InfoMenuPanel {

	public Button startButton;

	public MenuPanel loaderPanel;


	void Awake () {
		startButton.onClick.AddListener (delegate {
			loaderPanel.OpenPanel ();
		});
	}



}
