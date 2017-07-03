using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountablePaidStuff : PaidStuff {

	[Space(10)]
	public int countOfItems;
	public Text itemCountText;

	protected override void Awake () {
		base.Awake ();
		SetItemsCountText (countOfItems);
	}

	public void SetItemsCountText (int count) {
		itemCountText.text = count.ToString ();
	}
}
