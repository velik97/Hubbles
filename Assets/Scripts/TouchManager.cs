using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class TouchManager : MonoSingleton <TouchManager> {



	private bool isTouching;
	private bool justTouched;
	private bool justLifted;
	private bool isRotating;
	private bool justStartedRotating;
	private bool justEndedRotating;
	private bool touchIsOnField;
	[HideInInspector] public bool wasRotating;

	public UnityEvent OnTouchStart;
	public UnityEvent OnTouchEnd;

	public UnityEvent OnRotatingStart;
	public UnityEvent OnRotatingEnd;

	public UnityEvent OnTouchSurrounding;

	public bool liftedAfterWrongTouch;

	public float minRotateRadious;

	private Vector2 previousRotateVector;
	private Vector2 currentRotateVector;
	[HideInInspector] public float angle;
	private float deltaAngle;
	[HideInInspector] public Vector2 startTouchPos;
	[HideInInspector] public Coord startTouchCoord;
	private Vector2 currentTouchPos;
	public GameObject field;
	private LayerMask touchFieldsLayer;

	private Camera mainCamera;

	public void StartGame () {
		FindObjectsAndNullReferences ();
	}

	void Update () {

		if (!GameManager.Instance.gameIsStarted)
			return;

//		if (AnimationManager.Instance.isAnimating  ||  MenuManager.Instance.MenuIsOpened ) {
//			isTouching = false;
//			justTouched = false;
//			justLifted = false;
//			isRotating = false;
//			justStartedRotating = false;
//			justEndedRotating = false;
//			return;
//		}

		DetermineTouches ();

		if (justTouched && (AnimationManager.Instance.isAnimating || MenuManager.Instance.MenuIsOpened || !touchIsOnField)) {
			liftedAfterWrongTouch = false;
			justTouched = false;
			if (!touchIsOnField) {
				OnTouchSurrounding.Invoke ();
			}
		}

		if ((justLifted || justEndedRotating) && !liftedAfterWrongTouch) {
			liftedAfterWrongTouch = true;
			justLifted = false;
			justEndedRotating = false;
		}

		if (liftedAfterWrongTouch)
			DetermineRotating ();

		if (justTouched && touchIsOnField) {
			OnTouchStart.Invoke ();
		}

		if (justLifted) {
			OnTouchEnd.Invoke ();
		}

		if (justStartedRotating)
			OnRotatingStart.Invoke ();

		if (justEndedRotating)
			OnRotatingEnd.Invoke ();

//		CheckReferneces ();
//		print (angle);
	}

	void DetermineTouches () {
		if (Input.touches.Length > 0 || Input.GetMouseButton(0)) {
			Vector3 inputPosition;

#if UNITY_EDITOR
			inputPosition = Input.mousePosition;
#else
			inputPosition = (Vector3)Input.touches[0].position;
#endif

			Ray camRay = mainCamera.ScreenPointToRay (inputPosition);
			RaycastHit hit;

			justTouched = false;
			justLifted = false;
			if (Physics.Raycast (camRay, out hit, 2f, touchFieldsLayer)) {
				if (!isTouching) {
					isTouching = true;
					if (Input.touches.Length > 0) {
						if (Input.touches [0].phase == TouchPhase.Began) {
							justTouched = true;
						}
					} else if (Input.GetMouseButtonDown (0)) {
						justTouched = true;
					}
					startTouchPos = (Vector2)hit.point;
					if (hit.transform.gameObject == field) {
						touchIsOnField = true;
					} else {
						touchIsOnField = false; 
					}
					startTouchCoord = Coord.CoordFromVector2 (startTouchPos);
					if (!Coord.MapContains (startTouchCoord))
						touchIsOnField = false;
				}
				currentTouchPos = (Vector2)hit.point;
			} else {
				justTouched = false;
				justLifted = false;
				if (isTouching) {
					justLifted = true;
				}
				isTouching = false;
			}
			
		} else {
			justTouched = false;
			justLifted = false;
			if (isTouching) {
				justLifted = true;
			}
			isTouching = false;
		}

	}

	void DetermineRotating () {
		justStartedRotating = false;
		if (isTouching) {
			if (!isRotating) {
				if ((startTouchPos - currentTouchPos).sqrMagnitude > minRotateRadious * minRotateRadious) {
					justStartedRotating = true;
					wasRotating = true;
					isRotating = true;
					previousRotateVector = currentTouchPos - startTouchPos;
				}
			}
		}
		
		if (isRotating) {
			currentRotateVector = currentTouchPos - startTouchPos;
			deltaAngle = Vector2.Angle (previousRotateVector, currentRotateVector);
			if (Vector3.Cross (previousRotateVector, currentRotateVector).z > 0) {
				deltaAngle *= -1;
			}
			if ((startTouchPos - currentTouchPos).sqrMagnitude < minRotateRadious * minRotateRadious) {
				deltaAngle *= ((startTouchPos - currentTouchPos).magnitude) / minRotateRadious;
			}
			previousRotateVector = currentRotateVector;
			angle += deltaAngle;
		}

		justEndedRotating = false;
		if (!isTouching) {
			justEndedRotating = isRotating;
			isRotating = false;
		}

		if (justStartedRotating) {
			angle = 0;
		}

		if (justTouched) {
			wasRotating = false;
		}

	}

	void FindObjectsAndNullReferences () {
		field = GameObject.FindWithTag ("MainField");
		minRotateRadious *= Camera.main.orthographicSize;
		if (field == null) {
			Debug.LogError ("There is no objext with 'MainField' tag!");
		}
		mainCamera = Camera.main;
		touchFieldsLayer = LayerMask.GetMask ("TouchFields");
		isTouching = false;
		justTouched = false;
		justLifted = false;
		justStartedRotating = false;
		justEndedRotating = false;
		isRotating = false;
		touchIsOnField = false;
		wasRotating = false;
		liftedAfterWrongTouch = true;

//		OnTouchStart.AddListener (delegate {
//			print ("justTouched"); 
//		});
//
//		OnTouchEnd.AddListener (delegate {
//			print ("justLifted"); 
//		});
//
//		OnRotatingStart.AddListener (delegate {
//			print ("justStartedRotating"); 
//		});
//
//		OnRotatingEnd.AddListener (delegate {
//			print ("justEndedRotating"); 
//		});
//
//		OnTouchSurrounding.AddListener (delegate {
//			print ("touchSurrounding"); 
//		});
	}

	public void FreeListeners () {
		OnTouchStart.RemoveAllListeners ();
		OnTouchEnd.RemoveAllListeners ();
		OnRotatingStart.RemoveAllListeners ();
		OnRotatingEnd.RemoveAllListeners ();
	}

	void CheckReferneces () {
		if (justTouched) {
			print ("just touched");
		}
		
		if (isTouching) {
			print ("is Touching");
		}
		
		if (justLifted) {
			print ("just lifted");
		}
		if (justStartedRotating) {
			print ("just Started Rotating");
		}
		
		if (isRotating) {
			print ("is Rotating");
		}
		
		if (justEndedRotating) {
			print ("just Ended Rotating");
		}
		
		if (wasRotating) {
			print ("was Rotating");
		}
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (startTouchPos, 0.1f);

	}

}
