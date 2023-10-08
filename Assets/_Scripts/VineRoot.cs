using System.Collections.Generic;
using UnityEngine;

public class VineRoot : MonoBehaviour
{
    [SerializeField] ParticleSystem snapParticles;
    List<VineSegment> segments = new List<VineSegment>();
    VineLineRenderer vineLineRenderer;
    public bool isRootAnchored; // is this root still attached to the tree

    public List<Joint2D> segmentJoints; // ref for use in CheckVineStress method

    private VineSFX vineSFX;

    void Awake()
    {
        // try { sfx = GameObject.Find("SFX").GetComponent<SfxHandler>(); }
        // catch { sfx = null; }
        vineSFX = SfxHandler.vineSFX;
        vineLineRenderer = GetComponentInChildren<VineLineRenderer>();
    }

    void OnDisable()
    {
        segmentJoints = null;
        segments = null;
    }

    public void Init(List<VineSegment> _segments, bool _isRootAnchored = true, bool initSegments = true)
    {// Called from VineFactory; //TODO: this should take the vine's config (once that's setup)
        isRootAnchored = _isRootAnchored;
        vineLineRenderer.Init(_segments);
        segments = _segments;
        segmentJoints = new List<Joint2D>();
        foreach (VineSegment segment in segments)
        {
            segmentJoints.Add(segment.GetComponent<Joint2D>());
        }
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
        vineSFX.playVineSnapSound();
        segmentJoints.Remove(joint);
        GameManager.Instantiate(snapParticles, joint.attachedRigidbody.position, Quaternion.identity);
        DetachSegments(segmentIndex);
    }

    public void CheckVineStress()
    {
        // this will be called from SwingingController's FixedUpdate when player is on the vine.
        // We're calling from there to avoid having each VineRoot need an Update function;
        // This gets the average force applied to each joint; if any (or maybe avg of all?) are experiencing like 0.25 or 0.5 pct of their breaking point, play stretch noise.
        //  ... if it is nearing breaking, like 0.9, 0.95, play a vineSnap clip with increasing audio the closer to one.
        if (segmentJoints == null) return;
        foreach (Joint2D joint in segmentJoints)
        {
            float pctOfBreakForce = joint.reactionForce.magnitude / joint.breakForce;

            // stretch if 
            if (pctOfBreakForce > 0.2f)
            {
                vineSFX.playVineStretchSound();
                return;
            }
            if (pctOfBreakForce > 0.85f)
            {
                vineSFX.playVineStressSound(pctOfBreakForce - 0.1f);
                return;
            }
            // random probability to stretch on any given check.
            if (RNG.SampleProbability(0.005f) && PlayerInput.moveInput != 0) { vineSFX.playVineStretchSound(); return; }
        }
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
        // Get List of detached segments;
        for (int i = 0; i < segments.Count; i++)
        {
            if (i < segmentIndex) { segmentsRemaining.Add(segments[i]); }
            else { segmentsDetached.Add(segments[i]); }
            // Unparent all segments so we can clone this vineroot without cloning all segments;
            segments[i].transform.SetParent(null);
        }

        // Clone VineRoot and set detached segment head at segmentIndex and init it
        Transform newVineRoot = GameObject.Instantiate(transform, transform.parent);
        // reinit this vineRoot with the segments remaining with this root; Reinit them so they re-parent themselves;
        Init(segmentsRemaining, true, true);
        //Init new VineRoot; Set it to not anchored, and reinit detached segments;
        newVineRoot.GetComponent<VineRoot>().Init(segmentsDetached, false, true);
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

