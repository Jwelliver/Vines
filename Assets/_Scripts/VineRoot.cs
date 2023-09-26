using System.Collections.Generic;
using UnityEngine;

public class VineRoot : MonoBehaviour
{
    [SerializeField] ParticleSystem snapParticles;
    SfxHandler sfx;
    List<VineSegment> segments = new List<VineSegment>();
    VineLineRenderer vineLineRenderer;
    public bool isRootAnchored; // is this root still attached to the tree


    void Awake()
    {
        sfx = GameObject.Find("SFX").GetComponent<SfxHandler>();
        vineLineRenderer = GetComponentInChildren<VineLineRenderer>();
    }

    public void Init(List<VineSegment> _segments, bool _isRootAnchored = true, bool initSegments = true)
    {// Called from VineFactory; //TODO: this should take the vine's config (once that's setup)
        isRootAnchored = _isRootAnchored;
        vineLineRenderer.Init(_segments);
        segments = _segments;
        if (initSegments) { InitSegments(); }
    }

    public Transform GetSegmentAtIndex(int index)
    {
        // if the index is out of range; return null
        if (index > segments.Count - 1 || index < 0) return null;
        return segments[index].transform;

    }

    public void OnVineSnap(Joint2D joint, int segmentIndex)
    {
        //TODO: Consider passing the vineSegment instance itself, or the transform;
        sfx.vineSFX.playVineSnapSound();
        GameManager.Instantiate(snapParticles, joint.attachedRigidbody.position, Quaternion.identity);
        DetachSegments(segmentIndex);
    }

    void InitSegments()
    {
        // Handles setting up each segment; Ensures this vineRoot is the parent and sets the segment indexes
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].Init(this, i);
        }
    }


    void DetachSegments(int segmentIndex)
    {
        // Remove from segmentIndex to the end of segments from the segments list
        List<VineSegment> segmentsRemaining = new List<VineSegment>();
        List<VineSegment> segmentsDetached = new List<VineSegment>();
        // Get List of detached segments
        for (int i = 0; i < segments.Count; i++)
        {
            if (i < segmentIndex) { segmentsRemaining.Add(segments[i]); }
            else { segmentsDetached.Add(segments[i]); }
        }

        segments = segmentsRemaining;
        // reinit this vineRoot with the new (still attached) segments list; no need to reinit segments since their indexes haven't changed.
        Init(segments, true, false);
        // Clone VineRoot and set detached segment head at segmentIndex and init it
        Transform newVineRoot = GameObject.Instantiate(transform);
        //Init new VineRoot; Set it to not anchored, and reint detached segments;
        newVineRoot.GetComponent<VineRoot>().Init(segmentsDetached, false, true);
    }

    // == ORIG (prerefactor)
    // void DetachSegments(Transform segment, int segmentIndex)
    // {
    //     // Remove from segmentIndex to the end of segments from the segments list
    //     List<Transform> segmentsRemaining = new List<Transform>();
    //     List<Transform> segmentsDetached = new List<Transform>();
    //     // Get List of detached segments
    //     for (int i = 0; i < segments.Count; i++)
    //     {
    //         if (i < segmentIndex) { segmentsRemaining.Add(segments[i]); }
    //         else { segmentsDetached.Add(segments[i]); }
    //     }

    //     segments = segmentsRemaining;
    //     // reinit our line renderer with the new segments list
    //     vineLineRenderer.Init(segments);
    //     // Clone VineRoot and set detached segment head at segmentIndex and init it
    //     Transform newVineRoot = GameObject.Instantiate(transform);
    //     //Init new vineLineRenderer
    //     newVineRoot.GetComponent<VineLineRenderer>().Init(segmentsDetached);
    //     //
    //     // Set segment as child of newVineRoot
    //     segment.SetParent(newVineRoot);
    // }





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

