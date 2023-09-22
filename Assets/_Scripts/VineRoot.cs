using UnityEngine;

public class VineRoot : MonoBehaviour
{
    [SerializeField] ParticleSystem snapParticles;
    SfxHandler sfx;

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

