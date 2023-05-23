using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingController : MonoBehaviour
{


    [SerializeField] KeyCode grabKey = KeyCode.LeftShift;
    [SerializeField] LayerMask swingableLayer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D playerCollider;
    [SerializeField] CircleCollider2D grabCollider;
    [SerializeField] ScoreSystem scoreSystem;
    

    
    FixedJoint2D grabJoint;
    
    bool hasAttemptedGrab;
    bool hasReleased;
    public bool isSwinging;
    private float playerGravityScale; //stores player's gravity scale before setting to zero in order to reset upon swing release.

    // Start is called before the first frame update
    void Start()
    {
        // rb = GetComponent<Rigidbody2D>();
        grabJoint = GetComponentInChildren<FixedJoint2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(grabKey) && !isSwinging ) {
            hasAttemptedGrab=true;
            // Debug.Log("isAttemptingGrab()");
        } else if(Input.GetKeyUp(grabKey) && isSwinging) {
            hasReleased=true;
        }
    }

    void FixedUpdate() {
        handleSwingGrab();
        handleSwingRelease();
    }

    void handleSwingGrab() {
        if(hasAttemptedGrab){
            // Debug.Log("handleSwingGrab()");
            Collider2D[] colliders = Physics2D.OverlapCircleAll(grabCollider.transform.position, grabCollider.radius, swingableLayer);
            if (colliders.Length > 0) {
                Collider2D nearestCollider = getNearestCollider(colliders);
                transform.position = nearestCollider.transform.position;
                grabJoint.connectedBody = nearestCollider.attachedRigidbody;
                grabJoint.enabled = true;
                startSwing();
            }
        }
        hasAttemptedGrab=false;
    }

    void startSwing() {
        isSwinging = true;
        scoreSystem.onSwingGrab();
        
    }

    void endSwing() {
        // playerCollider.isTrigger=false;
        isSwinging = false;
        scoreSystem.onSwingRelease();
    }

    public void handleSwingRelease(bool forceRelease=false) {
        if(hasReleased || forceRelease) {
            grabJoint.enabled = false;
            grabJoint.connectedBody = null;
            endSwing();
        }
        hasReleased=false;
        hasAttemptedGrab=false;
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
}
