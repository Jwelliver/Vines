using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegFix : MonoBehaviour
{
    HingeJoint2D hingeJoint2D;
    Rigidbody2D rb;


    void Awake()
    {
        hingeJoint2D = GetComponent<HingeJoint2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float z = rb.rotation;
        if (z < hingeJoint2D.limits.min)
        {
            // Debug.Log("reset hingejoint to min - Orig: "+ z);
            // transform.localEulerAngles = new Vector3(transform.localEulerAngles.z, transform.localEulerAngles.y, hingeJoint2D.limits.min);
            rb.rotation = hingeJoint2D.limits.min;

        }
        else if (z > hingeJoint2D.limits.max)
        {
            // transform.localEulerAngles = new Vector3(transform.localEulerAngles.z, transform.localEulerAngles.y, hingeJoint2D.limits.max);
            // Debug.Log("reset hingejoint to max - Orig: "+ z);
            rb.rotation = hingeJoint2D.limits.max;
        }
    }
}
