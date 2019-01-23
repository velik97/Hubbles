using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Manages menus to be opened and closed in right order
/// </summary>
public class MenuManager : MonoSingleton <MenuManager>
{
	[SerializeField] private MenuPanel pauseMenu;
	[SerializeField] private MenuPanel loseMenu;
	[SerializeField] private Animator blackFadeAnimator;
	
	[Space(5)]
	[SerializeField] private Button pauseButton;
	
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

	public bool MenuIsOpened {
		get { return menuPanelStack != null && menuPanelStack.Count > 0; }
	}

	void Awake () {
		if (pauseButton != null)
			pauseButton.onClick.AddListener(OpenPauseMenu);
		if (blackFadeAnimator != null)
			blackFadeAnimator.gameObject.SetActive(false);
	}

	public void OpenMenuPanel (MenuPanel panel) {
		if (MenuIsOpened) {
			MenuPanel previous = MenuPanelStack.Peek();
			previous.onClosed.AddListener(delegate {
				panel.OpenPanel();	
				MenuPanelStack.Push(panel);
			});
			previous.ClosePanel();
		} else {
			panel.OpenPanel();	
			MenuPanelStack.Push(panel);
		}
		blackFadeAnimator.gameObject.SetActive(true);
		blackFadeAnimator.SetTrigger("Appear");
	}

	public void CloseTopMenuPanel () {
		if (MenuIsOpened) {
			MenuPanel top = MenuPanelStack.Pop();
			if (MenuIsOpened) {
				MenuPanel previous = MenuPanelStack.Peek();
				top.onClosed.AddListener(delegate {
					previous.OpenPanel();
				});
			}
			top.ClosePanel();
			if (!MenuIsOpened)
			{
				blackFadeAnimator.SetTrigger("Disappear");
				this.InvokeWithDelay(() => blackFadeAnimator.gameObject.SetActive(false), 0.25f);
			}
		}
	}

	public void OpenPauseMenu()
	{
		pauseButton.onClick.RemoveListener(OpenPauseMenu);
		OpenMenuPanel(pauseMenu);
	}

	public void ClosePauseMenu()
	{
		pauseButton.onClick.AddListener(OpenPauseMenu);
		CloseTopMenuPanel();
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
