using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineLineRenderer : MonoBehaviour
{
    List<Transform> segmentTransforms = new List<Transform>();
    Vector3[] positions;
    LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Init(List<VineSegment> segments)
    {// Called by VineRoot
        segmentTransforms.Clear();
        foreach (VineSegment vineSegment in segments)
        {
            segmentTransforms.Add(vineSegment.transform);
        }
        positions = new Vector3[segmentTransforms.Count];
        lineRenderer.positionCount = positions.Length;
    }


    void FixedUpdate() //TODO: consider adding secondary update condition; e.g. onScreen.. can use bool controlled by OnBecameVisible; Actually, no.. because it would only work when the vine root was visible, not the bottom of the vine, so use another method
    {
        if (segmentTransforms.Count == 0) { return; }
        for (int i = 0; i < segmentTransforms.Count; i++)
        {
            positions[i] = segmentTransforms[i].position;
        }
        lineRenderer.SetPositions(positions);
    }
}
