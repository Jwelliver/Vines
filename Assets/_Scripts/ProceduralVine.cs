using UnityEngine;

// ! 092123 NOT IN USE; Replaced By VineRoot after implementing VineFactory
//TODO: Rename to VineRoot
public class ProceduralVine : MonoBehaviour
{
    [SerializeField] ParticleSystem snapParticles;
    public SfxHandler sfx;

    void Awake()
    {
        try { sfx = GameObject.Find("SFX").GetComponent<SfxHandler>(); }
        catch { }
    }

    public void OnVineSnap(Joint2D joint)
    {
        //TODO: Consider passing the vineSegment instance itself, or the transform;
        sfx.vineSFX.playVineSnapSound();
        GameManager.Instantiate(snapParticles, joint.attachedRigidbody.position, Quaternion.identity);
    }
}


// ============= OLD: pre vine factory 092123
// public class ProceduralVine : MonoBehaviour
// {
//     [Header("Prefabs")]

//     [SerializeField] List<Transform> vineSegments; //list of possible vine segments
//     [SerializeField] List<Transform> adornments;

//     [Header("Vine Settings")]
//     public ProceduralVineSettings vineSettings = new ProceduralVineSettings();

//     public SfxHandler sfx;


//     void Awake()
//     {
//         try { sfx = GameObject.Find("SFX").GetComponent<SfxHandler>(); }
//         catch { }
//     }

//     void Start()
//     {
//         VineGenerator.GenerateVine(vineSettings, transform.position, transform, vineSegments, adornments);
//     }
// }