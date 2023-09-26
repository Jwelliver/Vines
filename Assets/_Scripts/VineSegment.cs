using UnityEngine;


public class VineSegment : MonoBehaviour
{
    VineRoot vineRoot;
    int segmentIndex;
    //TODO: consider setting up index and/or reference to next/prev segment; use for climbing

    void Awake()
    {
        vineRoot = GetComponentInParent<VineRoot>();
    }

    public VineRoot GetVineRoot()
    {
        return vineRoot;
    }

    public void SetSegmentIndex(int _segmentIndex)
    {
        segmentIndex = _segmentIndex;
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
