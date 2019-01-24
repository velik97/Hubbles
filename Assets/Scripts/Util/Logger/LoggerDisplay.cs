using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LoggerDisplay : MonoBehaviour
{
    private void Awake()
    {
        Text text = GetComponent<Text>();
        Logger.Subscribe(s => text.text = s);
    }
}
