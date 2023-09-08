using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineSegment : MonoBehaviour
{

    [SerializeField] ParticleSystem snapParticles;
    HingeJoint2D hinge;
    public ProceduralVine vineRoot;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // snapParticles = GetComponentInChildren<ParticleSystem>();
        vineRoot = transform.GetComponentInParent<ProceduralVine>();
        hinge = GetComponent<HingeJoint2D>();

    }

    void OnJointBreak2D(Joint2D joint) {
        // transform.root.GetComponent<ProceduralVine>().playVineSnapSound();
        GameManager.Instantiate(snapParticles, transform.position, Quaternion.identity);
        vineRoot.playVineSnapSound();
        
    }
}
