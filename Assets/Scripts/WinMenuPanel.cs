using UnityEngine;
using System.Collections;

public class WinMenuPanel : AnimatedMenuPanel {

	public StarsHolder starsHolder;

	public override void OpenPanel () {
		base.OpenPanel ();
		starsHolder.SetStars (LevelConfigHandler.GetStarsCount (LevelConfigHandler.CurrentIndex));
	}

}
