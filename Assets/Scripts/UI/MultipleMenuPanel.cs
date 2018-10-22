using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleMenuPanel : MenuPanel
{
	[SerializeField]
	private MenuPanel[] menuPanels;
	
	public override void OpenPanel()
	{
		foreach (var menuPanel in menuPanels)
		{
			menuPanel.OpenPanel();
		}
	}

	public override void ClosePanel()
	{
		foreach (var menuPanel in menuPanels)
		{
			menuPanel.ClosePanel();
		}
	}
}
