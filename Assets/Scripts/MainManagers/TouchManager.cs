using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Manages all user touch input
/// </summary>
public class TouchManager : MonoSingleton <TouchManager> {

	/// <summary>
	/// Is touching field (doesn't matter game field, or BG)
	/// </summary>
	private bool isTouching;
	/// <summary>
	/// Started touching in this frame
	/// </summary>
	private bool justTouched;
	/// <summary>
	/// Stopped touching in this frame 
	/// </summary>
	private bool justLifted;
	
	/// <summary>
	/// Is in process of rotating
	/// </summary>
	private bool isRotating;
	/// <summary>
	/// Started rotating in this frame
	/// </summary>
	private bool justStartedRotating;
	/// <summary>
	/// Stopped rotating in this frame
	/// </summary>
	private bool justEndedRotating;
	/// <summary>
	/// I touch on the game field, where the hubbles are?
	/// </summary>
	private bool touchIsOnField;
	/// <summary>
	/// Was rotating on previous step
	/// </summary>
	[HideInInspector] public bool wasRotating;

	/// <summary>
	/// Event on start of touch
	/// </summary>
	public UnityEvent onTouchStart;
	/// <summary>
	/// Event on end of touch
	/// </summary>
	public UnityEvent onTouchEnd;

	/// <summary>
	/// Event on rotating start
	/// </summary>
	public UnityEvent onRotatingStart;
	/// <summary>
	/// Event on rotating end
	/// </summary>
	public UnityEvent onRotatingEnd;

	/// <summary>
	/// Event on start of touching surrounding
	/// </summary>
	public UnityEvent onTouchSurrounding;

	/// <summary>
	/// If started touching while animation or menu or beyond game field, but haven't lifted yet
	/// </summary>
	public bool liftedAfterFalseTouch;

	/// <summary>
	/// Min distance from point of touch start enough to start rotation
	/// </summary>
	public float minRotateRadius;

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

	private void Awake()
	{
		FindObjectsAndNullReferences ();
	}

	private void Update () { 

		DetermineTouches ();

		if (justTouched && (AnimationManager.Instance.isAnimating || MenuManager.Instance.MenuIsOpened || !touchIsOnField)) {
			liftedAfterFalseTouch = false;
			justTouched = false;
			if (!touchIsOnField) {
				onTouchSurrounding.Invoke ();
			}
		}
		
		if (justTouched && touchIsOnField) {
			onTouchStart.Invoke ();
		}

		if ((justLifted || justEndedRotating) && !liftedAfterFalseTouch) {
			liftedAfterFalseTouch = true;
			justLifted = false;
			justEndedRotating = false;
		}

		if (liftedAfterFalseTouch)
			DetermineRotating ();

		if (justLifted) {
			onTouchEnd.Invoke ();
		}

		if (justStartedRotating)
			onRotatingStart.Invoke ();

		if (justEndedRotating)
			onRotatingEnd.Invoke ();

//		CheckReferences ();
//		print (angle);
	}

	/// <summary>
	/// Determine touch conditions
	/// </summary>
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

	/// <summary>
	/// Determine rotation conditions
	/// </summary>
	void DetermineRotating () {
		justStartedRotating = false;
		if (isTouching) {
			if (!isRotating) {
				if ((startTouchPos - currentTouchPos).sqrMagnitude > minRotateRadius * minRotateRadius) {
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
			if ((startTouchPos - currentTouchPos).sqrMagnitude < minRotateRadius * minRotateRadius) {
				deltaAngle *= ((startTouchPos - currentTouchPos).magnitude) / minRotateRadius;
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

	/// <summary>
	/// Find and initialize everything in start of the game
	/// </summary>
	void FindObjectsAndNullReferences () {
		field = GameObject.FindWithTag ("MainField");
		minRotateRadius *= Camera.main.orthographicSize;
		if (field == null) {
			Debug.LogError ("There is no object with 'MainField' tag!");
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
		liftedAfterFalseTouch = true;
	}

	/// <summary>
	/// Remove every listeners from every event
	/// </summary>
	public void FreeListeners () {
		onTouchStart.RemoveAllListeners ();
		onTouchEnd.RemoveAllListeners ();
		onRotatingStart.RemoveAllListeners ();
		onRotatingEnd.RemoveAllListeners ();
	}

	/// <summary>
	/// Log state of each boolean
	/// </summary>
	void CheckReferences () {
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
