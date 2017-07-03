using UnityEngine;
using System.Collections;

public class SimpleStarsHolder : StarsHolder {

	public GameObject star1;
	public GameObject star2;
	public GameObject star3;

	public override void SetStars (int count) {
		star1.SetActive (count > 0);
		star2.SetActive (count > 1);
		star3.SetActive (count > 2);
	}
}
