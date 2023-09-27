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
        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, climbSpeed * Time.fixedDeltaTime));
        // rb.MovePosition(nextVineSegment.position);
        attachJoints(nextVineSegment);
        yield return new WaitForSeconds(climbSecondsBetweenMove);
        if (shouldContinueClimb(direction)) StartCoroutine(Climb(direction));
        // }  
    }

    // ============ ORIG before trying prevSegment and nextSegment
    //TODO: replace with using GetNext and GetPrevSegment
    // private Rigidbody2D GetNextVineSegment(string direction)
    // {
    //     int checkRadiusOffsetY = direction == "up" ? 1 : -1;
    //     Vector3 checkRadiusOffset = new Vector3(0, checkRadiusOffsetY, 0);
    //     Vector2 checkRadiusStartPosition = grabJoint.connectedBody.transform.position + checkRadiusOffset;
    //     Collider2D[] colliders = Physics2D.OverlapCircleAll(checkRadiusStartPosition, 0.5f, swingableLayer);
    //     Collider2D closestSegment = null;
    //     float closestDistance = direction == "up" ? float.MaxValue : float.MinValue;

    //     // GameObject currentVineRoot = grabJoint.connectedBody.transform.GetComponent<VineSegment>().vineRoot.gameObject;
    //     GameObject currentVineRoot = currentVineSegmentRef.vineRoot.gameObject;
    //     // Debug.Log("currentVineRoot: "+currentVineRoot.name);

    //     foreach (Collider2D collider in colliders)
    //     {
    //         // skip over vine segments that do not belong to the same vine;
    //         GameObject vineRoot = collider.transform.GetComponent<VineSegment>().vineRoot.gameObject;
    //         if (!GameObject.ReferenceEquals(currentVineRoot, vineRoot))
    //         {
    //             continue;
    //         }
    //         float distance = Vector2.Distance(collider.transform.position, grabJoint.connectedBody.transform.position);
    //         if (direction == "up" && distance < closestDistance)
    //         {
    //             closestSegment = collider;
    //             closestDistance = distance;
    //         }
    //         else if (direction == "down" && distance > closestDistance)
    //         {
    //             closestSegment = collider;
    //             closestDistance = distance;
    //         }
    //     }
    //     return closestSegment.attachedRigidbody;
    // }
}
