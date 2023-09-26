using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineLineRenderer : MonoBehaviour
{
    List<Transform> segments = new List<Transform>();
    Vector3[] positions;
    LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Init(List<Transform> _segments, float width, Color color)
    {// Called by VineRoot
        segments = _segments;
        positions = new Vector3[_segments.Count];
        lineRenderer.positionCount = positions.Length;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }


    void FixedUpdate() //TODO: consider adding secondary update condition; e.g. onScreen.. can use bool controlled by OnBecameVisible; Actually, no.. because it would only work when the vine root was visible, not the bottom of the vine, so use another method
    {
        if (segments.Count == 0) { return; }
        for (int i = 0; i < segments.Count; i++)
        {
            positions[i] = segments[i].position;
        }
        lineRenderer.SetPositions(positions);
    }
}
