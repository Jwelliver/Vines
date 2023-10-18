using System;
using System.Collections.Generic;
using UnityEngine;


public class VineRoot : MonoBehaviour
{
    [SerializeField] ParticleSystem snapParticles;
    public bool isRootAnchored; // is this root still attached to the tree
    // * nSegments and segmentRbs need to be public for accessibility by VineSuspenseManager
    public int nSegments;
    public Rigidbody2D[] segmentRbs;
    VineSegment[] vineSegments;
    Joint2D[] segmentJoints;


    Vector3[] positions;
    LineRenderer lineRenderer;
    VineSuspenseManager vineSuspenseManager;

    private static float vineStretchSoundForce = 0.2f;
    private static float vineStressSoundForce = 0.85f;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        vineSuspenseManager = GetComponent<VineSuspenseManager>();
    }

    void ClearSegmentRefs()
    {
        vineSegments = null;
        segmentRbs = null;
        segmentJoints = null;
    }

    void OnDisable()
    {
        ClearSegmentRefs();
    }

    void OnDestroy()
    {
        ClearSegmentRefs();
    }

    public void Init(VineSegment[] _segments, bool _isRootAnchored = true, bool initSegments = true)
    {// Called from VineFactory; //TODO: this should take the vine's config (once that's setup)
        isRootAnchored = _isRootAnchored;
        nSegments = _segments.Length;
        vineSegments = _segments;
        segmentJoints = new Joint2D[nSegments];
        segmentRbs = new Rigidbody2D[nSegments];
        positions = new Vector3[nSegments];
        for (int i = 0; i < nSegments; i++)
        {
            segmentJoints[i] = _segments[i].GetComponent<Joint2D>();
            segmentRbs[i] = _segments[i].GetComponent<Rigidbody2D>();
            positions[i] = segmentRbs[i].position;
        }
        if (initSegments) { InitSegments(); }
        lineRenderer.positionCount = nSegments;
        // If this root is anchored, add our vineSuspenseManager instance to the resting queue
        // if (_isRootAnchored) // ! Disabled while sorting out mem issue w/ vineSuspenseMgr 101723
        // {
        //     VineSuspenseManager.vineRestingQueue.Add(vineSuspenseManager);
        // }

    }

    void FixedUpdate()
    {   // Update VineSegment positions for LineRenderer
        if (vineSuspenseManager.isVineSuspended || nSegments == 0) { return; }
        for (int i = 0; i < nSegments; i++)
        {
            positions[i] = segmentRbs[i].position;
        }
        lineRenderer.SetPositions(positions);
    }

    public Transform GetSegmentAtIndex(int index)
    {
        // if the index is out of range; return null
        if (index > nSegments - 1 || index < 0) return null;
        return vineSegments[index].transform;
    }

    public void OnVineSnap(Joint2D joint, int segmentIndex)
    {
        SfxHandler.vineSFX.playVineSnapSound();
        // segmentJoints.Remove(joint);
        GameManager.Instantiate(snapParticles, joint.attachedRigidbody.position, Quaternion.identity);
        DetachSegments(segmentIndex);
    }

    public void CheckVineStress()
    {
        // this will be called from SwingingController's FixedUpdate when player is on the vine.
        // We're calling from there to avoid having each VineRoot need an Update function;
        // This gets the average force applied to each joint; if any (or maybe avg of all?) are experiencing like 0.25 or 0.5 pct of their breaking point, play stretch noise.
        //  ... if it is nearing breaking, like 0.9, 0.95, play a vineSnap clip with increasing audio the closer to one.
        if (SfxHandler.vineSFX.vineAudio.isPlaying || segmentJoints == null) return;
        foreach (Joint2D joint in segmentJoints)
        {
            float pctOfBreakForce = joint.reactionForce.magnitude / joint.breakForce;

            if (pctOfBreakForce >= vineStressSoundForce)
            {
                SfxHandler.vineSFX.playVineStressSound(pctOfBreakForce - 0.1f);
                return;
            }
            if (pctOfBreakForce >= vineStretchSoundForce)
            {
                SfxHandler.vineSFX.playVineStretchSound();
                return;
            }
            // random probability to stretch on any given check.
            if (RNG.SampleProbability(0.005f) && PlayerInput.moveInput != 0) { SfxHandler.vineSFX.playVineStretchSound(); return; }
        }
    }

    void InitSegments()
    {
        // Handles setting up each segment; Ensures this vineRoot is the parent and sets the segment indexes
        int segIndex = 0;
        foreach (VineSegment seg in vineSegments)
        {
            seg.Init(this, segIndex);
            segIndex++;
        }
    }

    void DetachSegments(int segmentIndex)
    {
        // Remove from segmentIndex to the end of segments from the segments list
        int nSegmentsDetached = nSegments - segmentIndex;
        VineSegment[] segmentsRemaining = new VineSegment[segmentIndex]; //nRemaining = segmentIndex (i.e. indexAtDetach-0)
        VineSegment[] segmentsDetached = new VineSegment[nSegmentsDetached]; //nDetatched = nSegments-segmentIndex

        int detachedSegIndex = 0; //tracks detachedSegIndex so we can assign all segments in single loop
        // Get List of detached segments;
        for (int i = 0; i < nSegments; i++)
        {
            if (i < segmentIndex) { segmentsRemaining[i] = vineSegments[i]; }
            // else { segmentsDetached[i - nSegmentsDetached - 1] = vineSegments[i]; } // ?? Not sure; if this doesn't work; try using detachedSegIndex;
            else { segmentsDetached[detachedSegIndex] = vineSegments[i]; detachedSegIndex++; }

            // Unparent all segments so we can clone this vineroot without cloning all segments;
            vineSegments[i].transform.SetParent(null);
        }
        // Clone VineRoot and set detached segment head at segmentIndex and init it
        Transform newVineRoot = GameObject.Instantiate(transform, transform.parent);
        // reinit this vineRoot with the segments remaining with this root; Reinit them so they re-parent themselves;
        Init(segmentsRemaining, true, true);
        //Init new VineRoot; Set it to not anchored, and reinit detached segments;
        newVineRoot.GetComponent<VineRoot>().Init(segmentsDetached, false, true);
    }



}

