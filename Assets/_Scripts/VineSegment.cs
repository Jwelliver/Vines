using UnityEngine;


public class VineSegment : MonoBehaviour
{
    public VineRoot vineRoot;
    int segmentIndex;
    //TODO: consider setting up index and/or reference to next/prev segment; use for climbing

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
}



//====== old; pre-vine factory 092123
// public class VineSegment : MonoBehaviour
// {

//     [SerializeField] SpriteRenderer spriteRenderer;
//     [SerializeField] List<Sprite> vineSegmentSprites = new List<Sprite>();
//     [SerializeField] ParticleSystem snapParticles;
//     HingeJoint2D hinge;
//     public ProceduralVine vineRoot;


//     // Start is called before the first frame update
//     void Awake()
//     {
//         // snapParticles = GetComponentInChildren<ParticleSystem>();
//         spriteRenderer = GetComponent<SpriteRenderer>(); // * temp left out for trying new vineSegment 091223
//         vineRoot = transform.GetComponentInParent<ProceduralVine>();
//         hinge = GetComponent<HingeJoint2D>();
//         assignRandomSprite();
//     }

//     void assignRandomSprite()
//     {
//         Sprite rndSprite = RNG.RandomChoice(vineSegmentSprites);
//         spriteRenderer.sprite = rndSprite;
//     }

//     void OnJointBreak2D(Joint2D joint)
//     {
//         // transform.root.GetComponent<ProceduralVine>().playVineSnapSound();
//         GameManager.Instantiate(snapParticles, transform.position, Quaternion.identity);
//         vineRoot.sfx.vineSFX.playVineSnapSound();
//     }
// }
