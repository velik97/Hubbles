using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFinger : MonoBehaviour
{
    [SerializeField] private GameObject finger;
    [SerializeField] private GameObject outlinePrefab;
    [SerializeField] private GameObject trailPrefab;

    [SerializeField] private float fingerMoveDelay = .2f;
    [SerializeField] private float fingerShift = .3f;

    private GameObject currentTrail;
    private Camera mainCamera;

    private void Start()
    {
        TouchManager.Instance.isClicking.AddListener(c =>
        {
            if (c)
                StartClick();
            else
                StopClick();
        });
        TouchManager.Instance.currentTouchPos.AddListener(SetPosition);
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 inputPosition = Input.mousePosition;
        Ray camRay = mainCamera.ScreenPointToRay(inputPosition);
        RaycastHit hit;		
        if (Physics.Raycast(camRay, out hit, 2f))
        {
            SetPosition(hit.point);
        }
    }

    private void StartClick()
    {
        currentTrail = Instantiate(trailPrefab, transform);
        GameObject outline = Instantiate(outlinePrefab, transform.position, Quaternion.identity);
        this.InvokeWithDelay(() => Destroy(outline), 1f);

        this.PlayAnimation(SetShadowPosition, fingerMoveDelay, par => 1f-par);
    }
    
    private void StopClick()
    {
        if (currentTrail != null)
        {
            GameObject lastTrail = currentTrail;
            lastTrail.transform.parent = transform.parent;
            this.InvokeWithDelay(() => Destroy(lastTrail), 1f);
        }

        this.PlayAnimation(SetShadowPosition, fingerMoveDelay);
    }

    private void SetShadowPosition(float t)
    {
        finger.transform.localPosition = Vector3.up * fingerShift * t;
    }

    private void SetPosition(Vector2 position)
    {
        transform.position = position;
    }
}
