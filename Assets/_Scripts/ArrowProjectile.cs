using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] float minVelocity;
    [SerializeField] float maxVelocity;

    [SerializeField] float minAngle;
    [SerializeField] float maxAngle;
    [SerializeField] float pctChanceBreakHinge;
    [SerializeField] float penetrationDistance;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D myCollider;
    [SerializeField] FixedJoint2D fixedJoint;
    [SerializeField] Transform spriteObj;
    // public ArrowSFX sfx; //assigned by ArrowGenerator on instantiation

    bool hasLanded;

    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = new Vector3(0, 0, RNG.RandomRange(minAngle, maxAngle));
        rb.velocity = transform.up * RNG.RandomRange(minVelocity, maxVelocity);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (hasLanded) return;
        VineSegment vineSegment;
        bool isVineSegment = col.gameObject.TryGetComponent<VineSegment>(out vineSegment);
        if (isVineSegment && RNG.SampleProbability(pctChanceBreakHinge))
        {
            vineSegment.ForceBreakJoint();
            return;
        }
        stickToBody(col);
        // Penetrate(col.rigidbody);
        if (col.gameObject.tag == "Player")
        {
            GameManager.playerRef.GetComponent<CharacterController2D>().hitByArrow(col.otherCollider);
        }
    }

    void stickToBody(Collision2D col)
    {
        SfxHandler.arrowSFX.PlayArrowHitSound();
        fixedJoint.connectedBody = col.rigidbody;
        // Vector2 contactPoint = col.contacts[0].point; //TODO: later, maybe we can use this to set the fixed joint anchor to allow penetration.
        myCollider.enabled = false;
        fixedJoint.enabled = true;
        hasLanded = true;
    }

    // void Penetrate(Rigidbody2D otherBody)
    // {
    //     sfx.PlayArrowHitSound();
    //     // Calculate the target point of penetration based on desired penetration distance
    //     Vector3 penetrationTarget = spriteObj.transform.position + (Vector3)rb.velocity.normalized * penetrationDistance;
    //     rb.simulated = false;
    //     myCollider.enabled = false;
    //     // Slowdown duration based on the initial velocity and desired penetration distance. 
    //     float slowdownDuration = penetrationDistance / rb.velocity.magnitude;

    //     // Start the penetration Coroutine
    //     StartCoroutine(PenetrateOverTime(penetrationTarget, slowdownDuration, otherBody));
    // }

    // // Coroutine to handle the penetration over time
    // IEnumerator PenetrateOverTime(Vector3 target, float duration, Rigidbody2D otherBody)
    // {
    //     // Calculate initial position and time  
    //     Vector3 initialPosition = spriteObj.transform.position;
    //     float time = 0;

    //     // While the arrow graphic has not reached the target point
    //     while (time < duration)
    //     {
    //         // Increment the time by the fraction of the frame time (slows down as it approaches 1)
    //         time += Time.deltaTime;
    //         // Update the position of the arrow graphic
    //         spriteObj.transform.position = Vector3.Lerp(initialPosition, target, time / duration);
    //         // Wait until next frame
    //         yield return null;
    //     }

    //     // Ensure arrow graphic position is set to the target point when done
    //     spriteObj.transform.position = target;
    //     fixedJoint.connectedBody = otherBody;
    // }



}
