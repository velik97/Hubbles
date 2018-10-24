using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Manages menus to be opened and closed in right order
/// </summary>
public class MenuManager : MonoSingleton <MenuManager>
{

	[SerializeField]
	private MenuPanel pauseMenu;
	[SerializeField]
	private MenuPanel loseMenu;
	[SerializeField]
	private MenuPanel gamePanel;

	[Space(5)]
	[SerializeField]
	private Button pauseButton;
	
	private Stack <MenuPanel> menuPanelStack;

	public Stack<MenuPanel> MenuPanelStack
	{
		get
		{
			if (menuPanelStack == null)
				menuPanelStack = new Stack<MenuPanel>();
			return menuPanelStack;
		}
	}

	private float delayAfterClosingMenu = 0.1f;

	public bool MenuIsOpened {
		get { return menuPanelStack != null && menuPanelStack.Count > 0; }
	}

	void Awake () {
		pauseButton.onClick.AddListener(OpenPauseMenu);
	}

	private void OpenMenuPanel (MenuPanel panel) {
		if (MenuIsOpened) {
			MenuPanel previous = MenuPanelStack.Peek ();
			previous.onClosed.AddListener (delegate {
				panel.OpenPanel ();	
				MenuPanelStack.Push (panel);
			});
			previous.ClosePanel ();
		} else {
			panel.OpenPanel ();	
			MenuPanelStack.Push (panel);
		}
	}

	private void CloseTopMenuPanel () {
		if (MenuIsOpened) {
			MenuPanel top = MenuPanelStack.Pop ();
			if (MenuIsOpened) {
				MenuPanel previous = MenuPanelStack.Peek ();
				top.onClosed.AddListener (delegate {
					previous.OpenPanel ();
				});
			}
			top.ClosePanel ();
		}
	}

	public void OpenPauseMenu()
	{
		HubblesManager.Instance.ClearHighlightedGroup();
		pauseButton.onClick.RemoveListener(OpenPauseMenu);
		pauseButton.onClick.AddListener(ClosePauseMenu);
		OpenMenuPanel(pauseMenu);
		gamePanel.ClosePanel();
	}

	public void ClosePauseMenu()
	{
		pauseButton.onClick.RemoveListener(ClosePauseMenu);
		pauseButton.onClick.AddListener(OpenPauseMenu);
		CloseTopMenuPanel();
		gamePanel.OpenPanel();
	}

	public void OpenLoseMenu()
	{
		pauseButton.onClick.RemoveListener(OpenPauseMenu);
		OpenMenuPanel(loseMenu);
	}

	public void CloseLoseMenu()
	{
		pauseButton.onClick.AddListener(OpenPauseMenu);
		CloseTopMenuPanel();
	}
}
