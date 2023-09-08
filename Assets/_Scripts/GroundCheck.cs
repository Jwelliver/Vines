using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool isTouchingPlatform;
    public bool isTouchingLandZone;

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag=="platform") {
            isTouchingPlatform = true;
        } else if(other.tag=="PlatformLandZone") {
            isTouchingLandZone = true;
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if(other.tag=="platform") {
            isTouchingPlatform = false;
        } else if(other.tag=="PlatformLandZone") {
            isTouchingLandZone = false;
        }
    }
}
