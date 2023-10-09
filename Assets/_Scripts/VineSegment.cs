using UnityEngine;


public class VineSegment : MonoBehaviour
{
    public VineRoot vineRoot;
    int segmentIndex;

    public void Init(VineRoot vineRootRef, int _segmentIndex)
    {
        vineRoot = vineRootRef;
        segmentIndex = _segmentIndex;
        transform.SetParent(vineRootRef.transform);
    }

    public Transform GetNextSegment()
    {
        // returns next segment in the vine if exists
        return vineRoot.GetSegmentAtIndex(segmentIndex + 1);
    }

    public Transform GetPrevSegment()
    {
        // returns previous segment in the vine if exists
        return vineRoot.GetSegmentAtIndex(segmentIndex - 1);
    }

    void OnJointBreak2D(Joint2D joint)
    {
        vineRoot.OnVineSnap(joint, segmentIndex);
    }

    public void ForceBreakJoint()
    {
        HingeJoint2D myHinge = GetComponent<HingeJoint2D>();
        myHinge.enabled = false;
        OnJointBreak2D(myHinge);

    }
}