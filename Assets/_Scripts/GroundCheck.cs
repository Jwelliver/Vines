using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool isTouchingPlatform;

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag=="platform") {
            isTouchingPlatform = true;
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if(other.tag=="platform") {
            isTouchingPlatform = false;
        }
    }
}
