using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialFinger : MonoSingleton<TutorialFinger>
{
    public ITouchSource tutorialFingerTouchSource;
    public UnityEvent done;
    
    [Header("Refs")]
    [SerializeField] private GameObject finger;
    [SerializeField] private GameObject outlinePrefab;
    [SerializeField] private GameObject trailPrefab;

    [Header("Click Params")]
    [SerializeField] private float fingerMoveToShadowDelayDelay = .2f;
    [SerializeField] private float fingerShadowShift = .3f;
    
    [Header("Move finger params")]
    [SerializeField] private float fingerMoveToNewCoordSpeed = 5f;
    [SerializeField] private float fingerMoveToNewCoordShift = .25f;
    [SerializeField] private float delayBetweenActions = .5f;
    
    [Header("Rotate params")]
    [SerializeField] private float rotateShift = 1f;
    [SerializeField] private float rotateSectionTime = .5f;

    private GameObject currentTrail;
    private Queue<IEnumerator> queueOfCoroutines = new Queue<IEnumerator>();
    private Vector2 outOfScreenPosition;
    
    private bool isTouching = false;
    private bool isDoingSomething = false;

    private void Awake()
    {
        StartCoroutine(HandleQueueOfCoroutines());
        tutorialFingerTouchSource = new TutorialFingerTouchSource(
            () => transform.position,
            () => isTouching);

        outOfScreenPosition = transform.position;
    }

    public void ClickOnCoord(Coord coord)
    {
        queueOfCoroutines.Enqueue(MoveToCoordCoroutine(coord));
        queueOfCoroutines.Enqueue(DelayBetweenActions());
        queueOfCoroutines.Enqueue(Click());
    }

    public void RotateOnCoord(Coord coord, int turns)
    {
        queueOfCoroutines.Enqueue(MoveToCoordCoroutine(coord));
        queueOfCoroutines.Enqueue(DelayBetweenActions());
        queueOfCoroutines.Enqueue(RotateCoroutine(turns));
    }

    public void GetAway()
    {
        queueOfCoroutines.Enqueue(MoveToCoordCoroutine(Coord.CoordFromVector2(outOfScreenPosition)));
    }

    private IEnumerator HandleQueueOfCoroutines()
    {
        while (true)
        {
            if (queueOfCoroutines.Count == 0)
            {
                if (isDoingSomething)
                    done.Invoke();
                isDoingSomething = false;
                yield return null;
            }
            else
            {
                isDoingSomething = true;
                yield return queueOfCoroutines.Dequeue();
            }
        }
    }

    private IEnumerator MoveToCoordCoroutine(Coord coord)
    {
        Vector2 startPosition = transform.position;
        Vector2 endPosition = coord.ToVector2();
        
        if (startPosition == endPosition)
            yield break;

        Vector2 connectingVector = endPosition - startPosition;
        Vector2 middle = startPosition + (connectingVector / 2f);
        Vector2 shift = middle + connectingVector.Normal() * fingerMoveToNewCoordShift;

        float moveTime = connectingVector.magnitude / fingerMoveToNewCoordSpeed;

        yield return this.PlayAnimation(t =>
        {
             Vector2 pos = Vector2.Lerp(
                Vector2.Lerp(startPosition, shift, t),
                Vector2.Lerp(shift, endPosition, t), t);
            SetPosition(pos);
        }, moveTime, AnimationF.EasyInOut);
    }

    private IEnumerator RotateCoroutine(int turns)
    {
        yield return StartTouch();
        Vector2 startPosition = transform.position;
        float angle = 30f;
        Func<Vector2> angle2Vec = delegate
        {
            float a = Mathf.Deg2Rad * angle;
            float sin = Mathf.Sin(a);
            float cos = Mathf.Cos(a);
            return startPosition + (Vector2.right * cos + Vector2.up * sin) * rotateShift;
        };

        yield return this.PlayAnimation(t => SetPosition(Vector2.Lerp(startPosition, angle2Vec(), t)),
            rotateSectionTime, AnimationF.EasyInOut);

        if (turns == 5)
            turns = -1;
        if (turns == 4)
            turns = -2;

        yield return this.PlayAnimation(t =>
            {
                angle = 30f - 60f * turns * t;
                SetPosition(angle2Vec());
            },
            rotateSectionTime * Mathf.Abs(turns), AnimationF.EasyInOut);

        yield return EndTouch();
    }

    private IEnumerator Click()
    {
        GameObject outline = Instantiate(outlinePrefab, transform.position, Quaternion.identity);
        this.InvokeWithDelay(() => Destroy(outline), 1f);

        this.InvokeWithDelay(() => isTouching = true, fingerMoveToShadowDelayDelay * .25f);
        this.InvokeWithDelay(() => isTouching = false, fingerMoveToShadowDelayDelay * .75f);
        
        yield return this.PlayAnimation(SetShadowPosition, fingerMoveToShadowDelayDelay, AnimationF.EasyBetween, 1f, 1);
    }
    
    private IEnumerator StartTouch()
    {
        currentTrail = Instantiate(trailPrefab, transform);
        GameObject outline = Instantiate(outlinePrefab, transform.position, Quaternion.identity);
        this.InvokeWithDelay(() => Destroy(outline), 1f);

        yield return this.PlayAnimation(SetShadowPosition, fingerMoveToShadowDelayDelay, par => 1f-par);
        isTouching = true;
    }
    
    private IEnumerator EndTouch()
    {
        if (currentTrail != null)
        {
            GameObject lastTrail = currentTrail;
            lastTrail.transform.parent = transform.parent;
            this.InvokeWithDelay(() => Destroy(lastTrail), 1f);
        }

        isTouching = false;
        yield return this.PlayAnimation(SetShadowPosition, fingerMoveToShadowDelayDelay);
    }

    private IEnumerator DelayBetweenActions()
    {
        yield return new WaitForSeconds(delayBetweenActions);
    }

    private void SetShadowPosition(float t)
    {
        finger.transform.localPosition = Vector3.up * fingerShadowShift * t;
    }

    private void SetPosition(Vector2 position)
    {
        transform.position = position;
    }
    
    public class TutorialFingerTouchSource : ITouchSource
    {
        private Func<Vector2> touchPosFunc;
        private Func<bool> isTouchingFunc;

        public TutorialFingerTouchSource(Func<Vector2> touchPosFunc, Func<bool> isTouchingFunc)
        {
            this.touchPosFunc = touchPosFunc;
            this.isTouchingFunc = isTouchingFunc;
        }

        public Vector2 TouchPos()
        {
            return touchPosFunc();
        }

        public bool IsTouching()
        {
            return isTouchingFunc();
        }

        public bool TouchIsOnField()
        {
            return true;
        }
    }

}
