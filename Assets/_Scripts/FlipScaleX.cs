using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipScaleX : MonoBehaviour
{

    Vector3 origScale; //stores orig scale on start
    private bool isFacingLeft; 

    float moveInput;

    Transform myTransform;

    void Awake() {
        origScale = transform.localScale;
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");
        if (moveInput < 0 && !isFacingLeft) { 
            myTransform.localScale = new Vector3(-origScale.x, origScale.y, origScale.z);
            isFacingLeft = true;
        }

        else if (moveInput > 0 && isFacingLeft) {
            myTransform.localScale = origScale;
            isFacingLeft = false;        }
    }
}
