using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipScaleX : MonoBehaviour
{

    Vector3 origScale; //stores orig scale on start
    private bool isFacingLeft; 

    void Awake() {
        origScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        if (moveInput < 0 && !isFacingLeft) { 
            transform.localScale = new Vector3(-origScale.x, origScale.y, origScale.z);
            isFacingLeft = true;
        }

        else if (moveInput > 0 && isFacingLeft) {
            transform.localScale = origScale;
            isFacingLeft = false;        }
    }
}
