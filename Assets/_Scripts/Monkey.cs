using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyAI : MonoBehaviour
{
    public Transform player; // The player's transform
    public float movementSpeed; // The monkey's movement speed
    public float visionRange; // How far the monkey can "see" the player
    public float vineDetectionRange; // How close the monkey needs to be to a vine to grab it
    public LayerMask vineLayer; // The layer(s) the vines are on
    public float vineReleaseTime; // How long the monkey will hold onto a vine before releasing it
    public float vineSwingForce; // How much force the monkey applies to the vine while swinging
    public float minSwingAngle; // The minimum angle (in degrees) the monkey can swing at
    public float maxSwingAngle; // The maximum angle (in degrees) the monkey can swing at

    private Rigidbody2D rb;
    private bool hasTarget; // Whether the monkey has a target to chase (the player)
    private Vector2 targetPos; // The position the monkey is trying to reach
    private bool onVine; // Whether the monkey is currently holding onto a vine
    private GameObject currentVine; // The vine the monkey is currently holding onto
    private float timeOnVine; // How long the monkey has been on the current vine
    private Vector2 lastVinePos; // The position of the last vine the monkey was holding onto

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hasTarget = false;
        onVine = false;
        timeOnVine = 0f;
    }

    void Update()
    {
        // Check if the player is within vision range
        float distToPlayer = Vector2.Distance(transform.position, player.position);
        if (distToPlayer <= visionRange)
        {
            // Set the player as the target
            hasTarget = true;
            targetPos = player.position;
        }

        // If the monkey is holding onto a vine
        if (onVine)
        {
            // Check if it's time to release the vine
            timeOnVine += Time.deltaTime;
            if (timeOnVine >= vineReleaseTime)
            {
                ReleaseVine();
            }
        }

        // If the monkey doesn't have a target, do nothing
        if (!hasTarget)
        {
            return;
        }

        // If the monkey is close enough to a vine, grab it
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, vineDetectionRange, vineLayer);
        foreach (Collider2D collider in colliders)
        {
            GrabVine(collider.gameObject);
            break;
            // if (collider.gameObject.CompareTag("Vine"))
            // {
            //     GrabVine(collider.gameObject);
            //     break;
            // }
        }
    }

    void FixedUpdate()
    {
        if (hasTarget)
        {
            if (onVine)
            {
                // Apply force to the vine to swing
                Vector2 vinePos = currentVine.transform.position;
                Vector2 vineToMonkey = (Vector2)transform.position - vinePos;
                float vineToMonkeyAngle = Mathf.Atan2(vineToMonkey.y, vineToMonkey.x) * Mathf.Rad2Deg;
                float swingAngle = Mathf.Clamp(vineToMonkeyAngle, minSwingAngle, maxSwingAngle);
                Vector2 swingDir = Quaternion.AngleAxis(swingAngle, Vector3.forward) * Vector2.right;
                rb.AddForce(swingDir * vineSwingForce, ForceMode2D.Force);
            }
            else
            {
                // Move towards the target
                Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
                rb.MovePosition(rb.position + dir * movementSpeed * Time.fixedDeltaTime);
            }
        }
    }

    void GrabVine(GameObject vine)
    {
        // Release the current vine if there is one
        ReleaseVine();

        // Set the current vine and onVine flag
        currentVine = vine;
        onVine = true;
        timeOnVine = 0f;
        lastVinePos = vine.transform.position;

        // Set the monkey's position and rotation to match the vine's end point
        // transform.position = vine.GetComponent<Vine>().endPoint.position;
        // transform.rotation = vine.GetComponent<Vine>().endPoint.rotation;
    }

    void ReleaseVine()
    {
        if (onVine)
        {
            // Calculate the velocity to apply to the monkey when releasing the vine
            Vector2 currentVinePos = currentVine.transform.position;
            Vector2 vineToMonkey = (Vector2)transform.position - currentVinePos;
            Vector2 vineVelocity = (currentVinePos - lastVinePos) / Time.fixedDeltaTime;
            Vector2 monkeyVelocity = vineVelocity + vineToMonkey.normalized * vineSwingForce;
            monkeyVelocity = monkeyVelocity.normalized * Mathf.Clamp(monkeyVelocity.magnitude, 0f, movementSpeed);
            rb.velocity = monkeyVelocity;

            // Set onVine flag to false and reset timeOnVine
            onVine = false;
            timeOnVine = 0f;
        }
    }
}
