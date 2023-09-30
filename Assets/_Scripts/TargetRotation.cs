using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Applies force to rb2D toward a target rotation, provided by another object.
*/

public class TargetRotation : MonoBehaviour
{
    [SerializeField] Transform rotationTargetTransform;
    [SerializeField] Transform positionTargetTransform;
    [SerializeField] float force;

    public bool rotationEnabled = true;
    public bool positionEnabled = true;

    private Rigidbody2D rb;
    float targetRotation;
    Vector3 targetPosition;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        handleRotation();
        // handlePosition(); //TODO looks better but weird behavior on grab.
    }

    public void setForce(float newForce)
    {
        force = newForce;
        rotationEnabled = force > 0;
    }

    void handleRotation()
    {
        if (rotationEnabled)
        {
            // float targetRotation = targetTransform.localEulerAngles.z;
            targetRotation = rotationTargetTransform.eulerAngles.z;
            rb.MoveRotation(Mathf.LerpAngle(rb.rotation, targetRotation, force * Time.deltaTime));
        }
    }

    void handlePosition()
    {
        if (positionEnabled)
        {
            // float targetRotation = targetTransform.localEulerAngles.z;
            targetPosition = positionTargetTransform.position;
            rb.MovePosition(targetPosition);
            // rb.MoveRotation(Mathf.LerpAngle(rb.rotation, targetRotation, force*Time.deltaTime ));
        }
    }

    public void resetAngularVelocity()
    {
        rb.angularVelocity = 0f;
    }
}
