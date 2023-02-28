using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] float minVelocity;
    [SerializeField] float maxVelocity;

    [SerializeField] float minAngle;
    [SerializeField] float maxAngle;

    [SerializeField] float pctChanceHitVine;
    [SerializeField] Collider2D arrowHead;

    bool hasCollided;

    Rigidbody2D rb;
    FixedJoint2D fixedJoint;

    AudioSource hitSound;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fixedJoint = GetComponent<FixedJoint2D>();
        transform.eulerAngles = new Vector3(0,0,Random.Range(minAngle,maxAngle));
        rb.velocity = transform.up * Random.Range(minVelocity,maxVelocity);
        hitSound = GetComponent<AudioSource>();
        if(Random.Range(0f,1f)<pctChanceHitVine) {
            arrowHead.includeLayers = LayerMask.NameToLayer("Swingable");
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(hasCollided) return;
        hitSound.Play();
        fixedJoint.connectedBody = col.rigidbody;
        fixedJoint.enabled = true;
        if(col.transform.root.tag=="Player") {
            col.transform.root.GetComponent<CharacterController2D>().hitByArrow();
        }
        hasCollided = true;
    }



}
