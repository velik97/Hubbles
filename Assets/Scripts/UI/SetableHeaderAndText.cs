using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetableHeaderAndText : MonoBehaviour {

	public Text header;
	public Text message;

	public void SetHeader (string text) {
		header.text = text;
	}

	public void SetMessage (string text) {
		message.text = text;
	}
}
