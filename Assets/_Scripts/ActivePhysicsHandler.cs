using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePhysicsHandler : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag=="physicsOverride") {
            other.attachedRigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.tag=="physicsOverride") {
            other.attachedRigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
    }
}
