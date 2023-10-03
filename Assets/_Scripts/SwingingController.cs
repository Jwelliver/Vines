using System;
using System.Collections;
using UnityEngine;

public class SwingingController : MonoBehaviour
{

    [SerializeField] LayerMask swingableLayer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] CircleCollider2D grabCollider;
    [SerializeField] ScoreSystem scoreSystem;
    [SerializeField] Joint2D grabJoint;
    [SerializeField] float climbSpeed = 1.0f; // the speed of climbing up and down vines
    [SerializeField] float climbSecondsBetweenMove = 1f;
    [SerializeField] float minSlideForce;

    Transform myTransform;
    public static bool isClimbing;
    public static bool isSwinging;
    public static VineSegment currentVineSegmentRef;

    void Awake()
    {
        myTransform = transform;
    }

    void OnDisable()
    {
        currentVineSegmentRef = null;
    }

    void OnDestroy()
    {
        currentVineSegmentRef = null;
    }



    void FixedUpdate()
    {
        if (isSwinging)
        {
            if (currentVineSegmentRef == null) { isSwinging = false; } //TODO: put this here to handle an odd case where player starts level with isSwinging apparently = true
            else { currentVineSegmentRef.vineRoot.CheckVineStress(); }
        }
        handleSwingGrab();
        handleSwingRelease();
        handleClimbInput();
    }

    void attachJoints(Rigidbody2D vineSegmentRb)
    {
        if (vineSegmentRb == null) { return; }
        grabJoint.connectedBody = vineSegmentRb;
        grabJoint.enabled = true;
        currentVineSegmentRef = vineSegmentRb.transform.GetComponent<VineSegment>();
    }

    void releaseJoints()
    {
        grabJoint.connectedBody = null;
        grabJoint.enabled = false;
        currentVineSegmentRef = null;
    }

    void handleSwingGrab()
    {
        if (PlayerInput.hasAttemptedGrab)
        {
            // Debug.Log("handleSwingGrab()");
            Collider2D[] colliders = Physics2D.OverlapCircleAll(grabCollider.transform.position, grabCollider.radius, swingableLayer);
            if (colliders.Length > 0)
            {
                Collider2D nearestCollider = getNearestCollider(colliders);
                myTransform.position = nearestCollider.transform.position;
                attachJoints(nearestCollider.attachedRigidbody);
                startSwing();
            }
        }
        PlayerInput.hasAttemptedGrab = false;
    }

    void startSwing()
    {
        scoreSystem.onSwingGrab();
        isSwinging = true;

    }

    void endSwing()
    {
        // playerCollider.isTrigger=false;
        scoreSystem.onSwingRelease();
        isSwinging = false;
    }

    public void handleSwingRelease(bool forceRelease = false)
    {
        if (PlayerInput.hasReleased || forceRelease)
        {
            endClimb();
            releaseJoints();
            endSwing();
        }
        PlayerInput.hasReleased = false;
        PlayerInput.hasAttemptedGrab = false;
    }


    private Collider2D getNearestCollider(Collider2D[] colliders)
    {
        Collider2D nearestCollider = colliders[0];
        float nearestDistance = float.MaxValue;
        foreach (Collider2D collider in colliders)
        {
            float distance = Vector2.Distance(collider.transform.position, grabCollider.transform.position);
            if (distance < nearestDistance)
            {
                nearestCollider = collider;
                nearestDistance = distance;
            }
        }
        return nearestCollider;
    }

    void handleClimbInput()
    {
        if (!isSwinging) { return; }
        if (isClimbing && !PlayerInput.isAttemptingClimbUp && !PlayerInput.isAttemptingClimbDown)
        {
            endClimb();
        }

        if (!isClimbing && PlayerInput.isAttemptingClimbUp)
        {
            isClimbing = true;
            StartCoroutine(Climb("up"));
        }

        if (!isClimbing && PlayerInput.isAttemptingClimbDown)
        {
            isClimbing = true;
            StartCoroutine(Climb("down"));
        }
    }

    void endClimb()
    {
        isClimbing = false;
        StopAllCoroutines();
    }

    bool shouldContinueClimb(string direction)
    {
        bool isClimbButtonStillPressed = direction == "up" ? PlayerInput.isAttemptingClimbUp : PlayerInput.isAttemptingClimbDown;
        return isClimbing && isClimbButtonStillPressed;
    }

    IEnumerator Climb(string direction = "down")
    {
        do
        {
            Rigidbody2D nextVineSegment;

            // Try to get nextSegment as Transform from the currentVineSegmentRef;
            Transform nextSegmentTransform = direction == "up" ? currentVineSegmentRef.GetPrevSegment() : currentVineSegmentRef.GetNextSegment();
            // If it's not found, abort.
            if (nextSegmentTransform == null)
            {
                endClimb();
                yield break;
            }
            // Otherwise, get the nextVineSegment's RigidBody2D
            nextVineSegment = nextSegmentTransform.GetComponent<Rigidbody2D>();

            float reactionForceMag = grabJoint.reactionForce.magnitude;
            Vector2 targetPosition = nextVineSegment.position;

            if (direction == "down")//&& reactionForceMag >= minSlideForce
            {
                // float acc = Mathf.Abs(reactionForceMag / rb.mass);
                // float vel = acc * Time.deltaTime;
                // float displacement = Mathf.Abs(Vector2.Distance(currentVineSegmentRef.GetComponent<Rigidbody2D>().position, targetPosition));
                // float seconds = Mathf.Abs(displacement / vel);

                //directionToTargetPos = grabCollider.transform.position - targetPosition;
                //float velocityInDirection = Vector3.Dot(rb.velocity, directionToTargetPos); // this is the magnitude of velocity in the direction to target (unverified)

                attachJoints(nextVineSegment);
                // Debug.Log("Slide: seconds: " + seconds + " | d: " + displacement + " | vel: " + vel + " acc: " + acc);
                // yield return new WaitForSeconds(seconds);
            }
            else if (direction == "up")
            {
                attachJoints(nextVineSegment);
                // yield return new WaitForSeconds(climbSecondsBetweenMove);
            }

            yield return new WaitForSeconds(climbSecondsBetweenMove);
        }
        while (shouldContinueClimb(direction));
    }


    // =========== bak 093023
    // IEnumerator Climb(string direction = "down")
    // {
    //     Rigidbody2D nextVineSegment;
    //     try
    //     {
    //         // Try to get nextSegment as Transform from the currentVineSegmentRef;
    //         Transform nextSegmentTransform = direction == "up" ? currentVineSegmentRef.GetPrevSegment() : currentVineSegmentRef.GetNextSegment();
    //         // If it's not found, abort.
    //         if (nextSegmentTransform == null)
    //         {
    //             endClimb();
    //             yield break;
    //         }
    //         // Otherwise, get the nextVineSegment's RigidBody2D
    //         nextVineSegment = nextSegmentTransform.GetComponent<Rigidbody2D>();
    //     }
    //     catch (Exception e)
    //     {
    //         // Debug.LogError("Error: " + e);
    //         yield break;
    //     }

    //     if (!isClimbing) { yield break; }
    //     // Debug.Log("Climb > nextVineSegment" + nextVineSegment);

    //     float reactionForceMag = grabJoint.reactionForce.magnitude;

    //     Vector2 targetPosition = nextVineSegment.position;
    //     if (direction == "down" && reactionForceMag> minSlideForce)
    //     {
    //         attachJoints(nextVineSegment);
    //     }
    //     else if (direction == "up")
    //     {
    //         attachJoints(nextVineSegment);
    //     }
    //     // // while (rb.position.y != targetPosition.y)
    //     // // {
    //     // // ! 092723  removed Temporarily for mobile test; TODO: reimplement
    //     // // rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, climbSpeed * Time.fixedDeltaTime));
    //     // // rb.MovePosition(nextVineSegment.position);

    //     // Slide
    //     while(shouldContinueClimb(direction)) {

    //     }

    //     float acc = reactionForceMag / rb.mass;
    //     float vel = acc * Time.deltaTime;
    //     float displacement = Vector2.Distance(currentVineSegmentRef.GetComponent<Rigidbody2D>().position, targetPosition);
    //     float seconds = displacement / vel;

    //     // TODO: set climbseconds to velocity derived from reaction force
    //     yield return new WaitForSeconds(climbSecondsBetweenMove);
    //     if (shouldContinueClimb(direction)) StartCoroutine(Climb(direction));
    //     // }  
    // }

}
