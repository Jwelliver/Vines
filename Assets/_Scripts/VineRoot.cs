using System.Collections.Generic;
using UnityEngine;

public class VineRoot : MonoBehaviour
{
    [SerializeField] ParticleSystem snapParticles;
    SfxHandler sfx;
    List<Transform> segments = new List<Transform>();
    VineLineRenderer vineLineRenderer;

    float width;
    Color color;

    void Awake()
    {
        sfx = GameObject.Find("SFX").GetComponent<SfxHandler>();
        vineLineRenderer = GetComponentInChildren<VineLineRenderer>();
    }

    public void Init(List<Transform> _segments, float _width, Color _color)
    {// Called from VineFactory; //TODO: this should take the vine's config (once that's setup)
        segments = _segments;
        width = _width;
        color = _color;
        vineLineRenderer.Init(_segments, _width, _color);
    }

    public void OnVineSnap(Joint2D joint, int segmentIndex)
    {
        //TODO: Consider passing the vineSegment instance itself, or the transform;
        sfx.vineSFX.playVineSnapSound();
        GameManager.Instantiate(snapParticles, joint.attachedRigidbody.position, Quaternion.identity);
        DetachSegments(joint.transform, segmentIndex);
    }

    void DetachSegments(Transform segment, int segmentIndex)
    {
        // Remove from segmentIndex to the end of segments from the segments list
        List<Transform> segmentsRemaining = new List<Transform>();
        List<Transform> segmentsDetached = new List<Transform>();
        // Get List of detached segments
        for (int i = 0; i < segments.Count; i++)
        {
            if (i < segmentIndex) { segmentsRemaining.Add(segments[i]); }
            else { segmentsDetached.Add(segments[i]); }
        }

        segments = segmentsRemaining;
        // reinit our line renderer with the new segments list
        vineLineRenderer.Init(segments, width, color);
        // Clone VineRoot and set detached segment head at segmentIndex and init it
        Transform newVineRoot = GameObject.Instantiate(transform);
        //Init new vineLineRenderer
        newVineRoot.GetComponent<VineLineRenderer>().Init(segmentsDetached, width, color);
        // Set segment as child of newVineRoot
        segment.SetParent(newVineRoot);
    }

    // void OnBecameVisible()
    // {
    //     Debug.Log("VineRoot.OnBecameVisible() name: " + name);
    //     foreach (Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>())
    //     {
    //         rb.WakeUp();
    //     }
    // }

    // void OnBecameInvisible()
    // {
    //     Debug.Log("VineRoot.OnBecameInvisible() name: " + name);
    //     foreach (Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>())
    //     {
    //         rb.WakeUp();
    //     }
    // }
}

