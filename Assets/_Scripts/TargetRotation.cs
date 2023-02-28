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

    public bool rotationEnabled=true;
    public bool positionEnabled=true;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        handleRotation();
    }

    public void setForce(float newForce) {
        force = newForce;
    }

    void handleRotation() {
        if(rotationEnabled) {
            // float targetRotation = targetTransform.localEulerAngles.z;
            float targetRotation = rotationTargetTransform.eulerAngles.z;
            rb.MoveRotation(Mathf.LerpAngle(rb.rotation, targetRotation, force*Time.deltaTime ));
        }
    }

    void handlePosition() {
        if(positionEnabled) {
            // float targetRotation = targetTransform.localEulerAngles.z;
            Vector3 targetPosition = positionTargetTransform.position;
            rb.MovePosition(targetPosition);
            // rb.MoveRotation(Mathf.LerpAngle(rb.rotation, targetRotation, force*Time.deltaTime ));
        }
    }
}
