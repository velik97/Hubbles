using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : MonoSingleton <MenuManager> {

	public Stack <MenuPanel> menuPanelStack;

	public AnimatedMenuPanel shadowPanel;

	public bool MenuIsOpened {
		get {
			return menuPanelStack != null && menuPanelStack.Count > 0;
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
		}
	}
}
