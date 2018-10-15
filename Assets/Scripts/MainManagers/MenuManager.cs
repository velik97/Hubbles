﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages menus to be opened and closed in right order
/// </summary>
public class MenuManager : MonoSingleton <MenuManager> {

	public Stack <MenuPanel> menuPanelStack;
	
	private float delayAfterClosingMenu = 0.1f;
	private bool waitedForDelayAfterMenuClose = true;

	/// <summary>
	/// Button for taping out of menu
	/// </summary>
	public AnimatedMenuPanel shadowPanel;

	public bool MenuIsOpened {
		get {
			return (menuPanelStack != null && menuPanelStack.Count > 0) || !waitedForDelayAfterMenuClose;
		}
	}

	void Awake () {
		RefreshStack ();
	}

	public void RefreshStack () {
		menuPanelStack = new Stack<MenuPanel> ();
	}

	public void OpenMenuPanel (MenuPanel panel) {
		if (MenuIsOpened) {
			MenuPanel previous = menuPanelStack.Peek ();
			previous.OnClosed.AddListener (delegate {
				panel.OpenPanel ();	
				menuPanelStack.Push (panel);
			});
			previous.ClosePanel ();
		} else {
			shadowPanel.OpenPanel ();
			panel.OpenPanel ();	
			menuPanelStack.Push (panel);
		}
	}

	public void CloseTopMenuPanel () {
		if (MenuIsOpened) {
			MenuPanel top = menuPanelStack.Pop ();
			if (MenuIsOpened) {
				MenuPanel previous = menuPanelStack.Peek ();
				top.OnClosed.AddListener (delegate {
					previous.OpenPanel ();
				});
			} else {
				shadowPanel.ClosePanel ();
			}
			top.ClosePanel ();
			if (menuPanelStack.Count == 0)
			{
				waitedForDelayAfterMenuClose = false;
				StartCoroutine(WaitForLastMenuClosing());
			}
		}
	}

	private IEnumerator WaitForLastMenuClosing()
	{
		yield return new WaitForSeconds(delayAfterClosingMenu);
		waitedForDelayAfterMenuClose = true;
	}
}
