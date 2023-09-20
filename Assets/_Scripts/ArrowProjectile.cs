using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] float minVelocity;
    [SerializeField] float maxVelocity;

    [SerializeField] float minAngle;
    [SerializeField] float maxAngle;

    [SerializeField] float pctChanceMissVine;
    [SerializeField] float pctChanceBreakHinge;
    // [SerializeField] Collider2D arrowHead;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D myCollider;
    [SerializeField] FixedJoint2D fixedJoint;
    public ArrowSFX sfx; //assigned by ArrowGenerator on instantiation

    bool hasLanded;

    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = new Vector3(0, 0, RNG.RandomRange(minAngle, maxAngle));
        rb.velocity = transform.up * RNG.RandomRange(minVelocity, maxVelocity);
        if (RNG.SampleProbability(pctChanceMissVine))
        {
            myCollider.excludeLayers = LayerMask.NameToLayer("Swingable");
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (hasLanded) return;
        HingeJoint2D otherHinge = col.gameObject.GetComponent<HingeJoint2D>();
        if (otherHinge != null && RNG.SampleProbability(pctChanceBreakHinge))
        {
            otherHinge.enabled = false;
            return;
        }
        stickToBody(col.rigidbody);
        if (col.transform.root.tag == "Player")
        {
            col.transform.root.GetComponentInChildren<CharacterController2D>().hitByArrow();
        }
    }

    void stickToBody(Rigidbody2D otherBody)
    {
        sfx.PlayArrowHitSound();
        fixedJoint.connectedBody = otherBody;
        fixedJoint.enabled = true;
        myCollider.enabled = false;
        hasLanded = true;
    }



}
