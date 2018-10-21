using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Manages all user touch input
/// </summary>
public class TouchManager : MonoSingleton <TouchManager>
{
	public ReactiveProperty<TouchState> touchState;

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

	private float delayAfterAnimationOrMenuForFalseTouch = 0.05f;
	private float lastTimeOfAnimationOrMenu;

	private void Awake()
	{
		FindObjectsAndNullReferences ();
	}

	private void Update ()
	{
		TouchState initialState = touchState.Value;
		TouchState resultState = initialState;
		bool isTouching = false;
		bool touchIsOnField = false;
		DetermineTouches(out isTouching, out touchIsOnField);

		bool possibleFalseTouch = false;
		DetermineFalseTouch(out possibleFalseTouch);

		if (isTouching)
		{
			if (initialState == TouchState.Empty)
			{
				if (possibleFalseTouch)
					resultState = TouchState.StartedFalseTouch;
				else if (touchIsOnField)
				{
					startTouchPos = currentTouchPos;
					startTouchCoord = Coord.CoordFromVector2(startTouchPos);
					resultState = TouchState.StartedTouching;
				}
				else
					resultState = TouchState.StartedTouchingSurrounding;
			}
		}
		else
		{
			switch (initialState)
			{
				case TouchState.StartedFalseTouch:
					resultState = TouchState.EndedFalseTouch;
					break;
				case TouchState.StartedTouching:
					resultState = TouchState.EndedTouching;
					break;
				case TouchState.StartedRotating:
					resultState = TouchState.EndedRotating;
					break;
				case TouchState.StartedTouchingSurrounding:
					resultState = TouchState.EndedTouchingSurrounding;
					break;
			}
		}

		if (initialState == TouchState.StartedTouching)
		{
			bool isRotating = false;
			DetermineRotating(out isRotating);
			if (isRotating)
			{
				resultState = TouchState.StartedRotating;
			}
		}
		else if (initialState == TouchState.StartedRotating)
		{
			FindRotatingAngle();
		}
		
		if (resultState != initialState)
			touchState.Value = resultState;
		if (resultState == TouchState.EndedRotating ||
		    resultState == TouchState.EndedTouching ||
		    resultState == TouchState.EndedTouchingSurrounding || 
		    resultState == TouchState.EndedFalseTouch)
			touchState.Value = TouchState.Empty;
	}
	
	

	/// <summary>
	/// Determine touch conditions
	/// </summary>
	private void DetermineTouches (out bool isTouching, out bool touchIsOnField)
	{
		isTouching = false;
		touchIsOnField = false;
		if (Input.touches.Length > 0 || Input.GetMouseButton(0))
		{
			Vector3 inputPosition;

#if UNITY_EDITOR
			inputPosition = Input.mousePosition;
#else
			inputPosition = (Vector3)Input.touches[0].position;
#endif

			Ray camRay = mainCamera.ScreenPointToRay(inputPosition);
			RaycastHit hit;
			
			if (Physics.Raycast(camRay, out hit, 2f, touchFieldsLayer))
			{
				isTouching = true;
				currentTouchPos = (Vector2)hit.point;
				if (hit.transform.gameObject == field && Coord.MapContains(Coord.CoordFromVector2(currentTouchPos)))
				{
					touchIsOnField = true;
				}
			}
		}
	}

	private void DetermineFalseTouch(out bool possibleFalseTouch)
	{
		if (AnimationManager.Instance.isAnimating || MenuManager.Instance.MenuIsOpened)
			lastTimeOfAnimationOrMenu = 0f;
		else
			lastTimeOfAnimationOrMenu += Time.deltaTime;
		possibleFalseTouch = lastTimeOfAnimationOrMenu < delayAfterAnimationOrMenuForFalseTouch;
	}

	/// <summary>
	/// Determine rotation conditions
	/// </summary>
	private void DetermineRotating (out bool isRotating)
	{
		isRotating = false;
		if ((startTouchPos - currentTouchPos).sqrMagnitude > minRotateRadius * minRotateRadius) {
			previousRotateVector = currentTouchPos - startTouchPos;
			isRotating = true;
			angle = 0f;
		}
	}

	private void FindRotatingAngle()
	{
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
		touchState = new ReactiveProperty<TouchState>();
		touchState.Value = TouchState.Empty;
	}

	/// <summary>
	/// Remove every listeners from every event
	/// </summary>
	public void RemoveAllListeners () {
		touchState.RemoveAllListeners();
	}


	void OnDrawGizmos () {
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (startTouchPos, 0.1f);
	}

}