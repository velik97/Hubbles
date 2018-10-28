using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHintMenu : AnimatedMenuPanel
{
    [SerializeField] private List<GameObject> hints;
    [SerializeField] private Button nextHintButton;

    private int currentHintIndex = 0;

    private void Awake()
    {
        foreach (var hint in hints)
        {
            hint.SetActive(false);
        }
        nextHintButton.onClick.AddListener(MoveNext);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        hints[currentHintIndex].SetActive(true);
    }

    private void MoveNext()
    {
        if (currentHintIndex < hints.Count - 1)
        {
            hints[currentHintIndex++].SetActive(false);
            hints[currentHintIndex].SetActive(true);
        }
        else
        {
            MenuManager.Instance.CloseTopMenuPanel();
        }
    }
}
