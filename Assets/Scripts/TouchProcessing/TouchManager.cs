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

	public float rotationDecreaseDistance = 0.5f;

	private ITouchSource touchSource;
	private Vector2 currentTouchPos;

	private Vector2 previousRotateVector;
	private Vector2 currentRotateVector;
	[HideInInspector] public float angle;
	private float deltaAngle;
	[HideInInspector] public Vector2 startTouchPos;
	[HideInInspector] public Coord startTouchCoord;
	[HideInInspector] public Vector2 startRotatingPos;
	[HideInInspector] public Coord startRotatingCoord;

	private float delayAfterAnimationOrMenuForFalseTouch = 0.05f;
	private float lastTimeOfAnimationOrMenu;

	private float[] rotateDistanceFractions = {0.04f, 0.16f, 0.36f, 0.64f}; // = {0.2, 0.4, 0.6, 0.8}^2
	private Vector2[] rotationVectorFractions = new Vector2[4];
	private bool[] fractionsAreReached = new bool[4];

	private void Awake()
	{
		FindObjectsAndNullReferences ();
		touchSource = new DesktopTouchSource();
	}

	public void SetUserInput()
	{
#if UNITY_EDITOR
		touchSource = new DesktopTouchSource();
#else
		touchSource = new MobileTouchSource();
#endif
	}

	public void SetOtherInput(ITouchSource otherTouchSource)
	{
		touchSource = otherTouchSource;
	}

	private void Update ()
	{		
		TouchState initialState = touchState.Value;
		TouchState resultState = initialState;

		if (touchSource.IsTouching())
		{
			currentTouchPos = touchSource.TouchPos();
			if (initialState == TouchState.Empty)
			{
				if (DetermineFalseTouch())
					resultState = TouchState.StartedFalseTouch;
				else if (touchSource.TouchIsOnField())
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
			if (DetermineRotating())
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
		if (resultState == TouchState.EndedRotating)
			UpdateAfterEndOfRotating();
		if (resultState == TouchState.EndedRotating ||
		    resultState == TouchState.EndedTouching ||
		    resultState == TouchState.EndedTouchingSurrounding || 
		    resultState == TouchState.EndedFalseTouch)
			touchState.Value = TouchState.Empty;
	}

	private bool DetermineFalseTouch()
	{
		if (AnimationManager.Instance.isAnimating || MenuManager.Instance.MenuIsOpened)
			lastTimeOfAnimationOrMenu = Time.time;

		return Time.time - lastTimeOfAnimationOrMenu < delayAfterAnimationOrMenuForFalseTouch;
	}

	/// <summary>
	/// Determine rotation conditions
	/// </summary>
	private bool DetermineRotating ()
	{
		float sqrDist = (startTouchPos - currentTouchPos).sqrMagnitude;
		
		for (var i = 0; i < fractionsAreReached.Length; i++)
		{
			if (fractionsAreReached[i])
				continue;
			fractionsAreReached[i] = sqrDist > minRotateRadius * minRotateRadius * rotateDistanceFractions[i];
			if (!fractionsAreReached[i])
				return false;
			rotationVectorFractions[i] = currentTouchPos;
		}
		
		if (sqrDist > minRotateRadius * minRotateRadius)
		{
			startRotatingCoord = Coord.RotationCoord(startTouchPos, currentTouchPos, rotationVectorFractions);
			startRotatingPos = Coord.Vector2FromCoord(startRotatingCoord);
			previousRotateVector = currentTouchPos - startRotatingPos;
			angle = 0f;
			return true;
		}
		return false;
	}

	private void FindRotatingAngle()
	{
		currentRotateVector = currentTouchPos - startRotatingPos;
		deltaAngle = Vector2.Angle (previousRotateVector, currentRotateVector);
		if (Vector3.Cross (previousRotateVector, currentRotateVector).z > 0) {
			deltaAngle *= -1;
		}
		if (currentRotateVector.sqrMagnitude < rotationDecreaseDistance * rotationDecreaseDistance)
		{
			print(0);
			deltaAngle *= currentRotateVector.magnitude / rotationDecreaseDistance;
		}
		previousRotateVector = currentRotateVector;
		angle += deltaAngle;
	}

	private void UpdateAfterEndOfRotating()
	{
		fractionsAreReached = new bool[4];
	}

	/// <summary>
	/// Find and initialize everything in start of the game
	/// </summary>
	void FindObjectsAndNullReferences () {
		minRotateRadius *= Camera.main.orthographicSize;
		rotationDecreaseDistance *= Camera.main.orthographicSize;
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
	
	public abstract class UserTouchSource : ITouchSource
	{
		private readonly LayerMask touchFieldsLayer = LayerMask.GetMask("TouchFields");
		private readonly Camera mainCamera = Camera.main;
		private Vector2 pos = Vector2.zero;

		public Vector2 TouchPos()
		{
			Ray camRay = mainCamera.ScreenPointToRay(InputPosition());
			RaycastHit hit;
			
			if (Physics.Raycast(camRay, out hit, 2f, touchFieldsLayer))
			{
				pos = hit.point;
			}			
			return pos;
		}

		public abstract bool IsTouching();

		public bool TouchIsOnField()
		{
			Ray camRay = mainCamera.ScreenPointToRay(InputPosition());
			RaycastHit hit;
			
			if (Physics.Raycast(camRay, out hit, 2f, touchFieldsLayer))
			{
				if (Coord.MapContains(Coord.CoordFromVector2(pos)))
				{
					return true;
				}
			}
			return false;
		}

		protected abstract Vector2 InputPosition();
	}

	public class DesktopTouchSource : UserTouchSource
	{
		public override bool IsTouching()
		{
			return Input.GetMouseButton(0);
		}

		protected override Vector2 InputPosition()
		{
			return Input.mousePosition;
		}
	}
	
	public class MobileTouchSource : UserTouchSource
	{
		public override bool IsTouching()
		{
			return Input.touches.Length > 0;
		}

		protected override Vector2 InputPosition()
		{
			return Input.touches.Length > 0 ? Input.touches[0].position : Vector2.zero;
		}
	}
}