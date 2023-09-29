using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwingingController : MonoBehaviour
{


    // [SerializeField] KeyCode grabKey = KeyCode.LeftShift;
    [SerializeField] KeyCode climbUpKey = KeyCode.W;
    [SerializeField] KeyCode climbDownKey = KeyCode.S;
    [SerializeField] LayerMask swingableLayer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] CircleCollider2D grabCollider;
    [SerializeField] ScoreSystem scoreSystem;
    [SerializeField] Joint2D grabJoint;
    [SerializeField] float climbSpeed = 1.0f; // the speed of climbing up and down vines
    [SerializeField] float climbSecondsBetweenMove = 1f;

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
        isSwinging = true;
        scoreSystem.onSwingGrab();

    }

    void endSwing()
    {
        // playerCollider.isTrigger=false;

        isSwinging = false;
        scoreSystem.onSwingRelease();
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
        KeyCode keyToCheck = direction == "up" ? climbUpKey : climbDownKey;
        return isClimbing && Input.GetKey(keyToCheck);
    }

    IEnumerator Climb(string direction = "down")
    {
        Rigidbody2D nextVineSegment;
        try
        {
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
        }
        catch (Exception e)
        {
            // Debug.LogError("Error: " + e);
            yield break;
        }

        if (!isClimbing) { yield break; }
        // Debug.Log("Climb > nextVineSegment" + nextVineSegment);
        Vector2 targetPosition = nextVineSegment.position;

        // while (rb.position.y != targetPosition.y)
        // {
        // ! 092723  removed Temporarily for mobile test; TODO: reimplement
        // rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, climbSpeed * Time.fixedDeltaTime));
        // rb.MovePosition(nextVineSegment.position);
        attachJoints(nextVineSegment);
        yield return new WaitForSeconds(climbSecondsBetweenMove);
        if (shouldContinueClimb(direction)) StartCoroutine(Climb(direction));
        // }  
    }
}
