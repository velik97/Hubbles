using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TutorialHand : MonoBehaviour
{
    [SerializeField] private GameObject outline;
    private TrailRenderer trailRenderer;
}
