using UnityEngine;
using System.Collections;

public class LETouchManager : MonoSingleton <LETouchManager> {

	private Vector2 firstHitPosition;
	private Vector2 currentPosition;

	public Coord firstHitCoord;
	public Coord currentCoord;
	private Coord lastCoord;

	public LayerMask touchFieldsLayer;

	private bool isTouching;
	public bool justTouched;
	public bool justLifted;

	public bool selectZoneWasUpdated;

	void Start () {
		FindObjectsAndNullReferences ();
	}

	public void foo (int i) {
		print (i);
	}

	void Update () {
		DetermineTouches ();

//		CheckReferneces ();
	}

	public void DetermineTouches () { 
		justTouched = false;
		justLifted = false;
		selectZoneWasUpdated = false;

		if (Input.GetMouseButton (0)) {
			Vector3 inputPosition = Input.mousePosition;

			Ray camRay = Camera.main.ScreenPointToRay (inputPosition);
			RaycastHit hit;

			if (Physics.Raycast (camRay, out hit, 2f, touchFieldsLayer)) {
				Coord hitCoord = Coord.LECoordFromVector2 ((Vector2)hit.point);

				if (!isTouching) {
					isTouching = true;
					justTouched = true;
				}

				if (justTouched) {
					firstHitCoord = hitCoord;
					selectZoneWasUpdated = true;
				}

				currentCoord = hitCoord;

				if (currentCoord != lastCoord) {
					lastCoord = hitCoord;
					selectZoneWasUpdated = true;
				}

			} else {
				if (isTouching)
					justLifted = true;
				isTouching = false;
			}
		} else {
			if (isTouching)
				justLifted = true;
			isTouching = false;
		}
	}
		

	void CheckReferneces () {
		if (justTouched) {
			print ("just touched: " + firstHitCoord.ToString ());
		}

		if (justLifted) {
			print ("just lifted");
		}

		if (isTouching) {
			print ("is Touching");
		}

		if (selectZoneWasUpdated) {
			print ("select zone was updated" + "\n" +
				currentCoord.ToString ());
		}
	}

	void FindObjectsAndNullReferences () {
		touchFieldsLayer = LayerMask.GetMask ("TouchFields");
		isTouching = false;
		justTouched = false;
		selectZoneWasUpdated = false;
	}

}
