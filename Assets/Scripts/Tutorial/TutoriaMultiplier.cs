using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoriaMultiplier : MonoBehaviour
{
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;
    [SerializeField] private float speed;

    private float t;

    private void Update()
    {
        t += Time.deltaTime;

        transform.localScale = Vector3.one * (minSize + Mathf.Abs((maxSize - minSize) * Mathf.Sin(t * speed)));
    }
}
