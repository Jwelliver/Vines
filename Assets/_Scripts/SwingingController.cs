using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwingingController : MonoBehaviour
{


    [SerializeField] KeyCode grabKey = KeyCode.LeftShift;
    [SerializeField] KeyCode climbUpKey = KeyCode.W;
    [SerializeField] KeyCode climbDownKey = KeyCode.S;
    [SerializeField] LayerMask swingableLayer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D playerCollider;
    [SerializeField] CircleCollider2D grabCollider;
    [SerializeField] ScoreSystem scoreSystem;
    [SerializeField] Joint2D grabJoint;
    [SerializeField] float climbSpeed = 1.0f; // the speed of climbing up and down vines
    [SerializeField] float climbSecondsBetweenMove = 1f;

    private bool isClimbing;

    Transform myTransform;
    
    bool hasAttemptedGrab;
    bool hasReleased;

    // bool isClimbingUp;
    // bool isClimbingDown;

    public bool isSwinging;
    private float playerGravityScale; //stores player's gravity scale before setting to zero in order to reset upon swing release.



    void Awake() {
        myTransform = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        // rb = GetComponent<Rigidbody2D>();
        // grabJoint = GetComponentInChildren<FixedJoint2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        handleGrabInput();
        handleClimbInput();
    }

    void handleGrabInput() {
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

    void attachJoints(Rigidbody2D vineSegment) {
        grabJoint.connectedBody = vineSegment;
        grabJoint.enabled = true;
    }

    void releaseJoints() {
        grabJoint.connectedBody=null;
        grabJoint.enabled = false;
    }

    void handleSwingGrab() {
        if(hasAttemptedGrab){
            // Debug.Log("handleSwingGrab()");
            Collider2D[] colliders = Physics2D.OverlapCircleAll(grabCollider.transform.position, grabCollider.radius, swingableLayer);
            if (colliders.Length > 0) {
                Collider2D nearestCollider = getNearestCollider(colliders);
                myTransform.position = nearestCollider.transform.position;
                attachJoints(nearestCollider.attachedRigidbody);
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
            endClimb();
            releaseJoints();
            endSwing();
        }
        hasReleased=false;
        hasAttemptedGrab=false;
    }


    private Collider2D getNearestCollider(Collider2D[] colliders) {
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



    void handleClimbInput()  {   
        if(!isSwinging) {return;}
        if(isClimbing && !Input.GetKey(climbUpKey)&&!Input.GetKey(climbDownKey) ) {
            endClimb();
        }
        
        if(!isClimbing && Input.GetKeyDown(climbUpKey)) {
            StartCoroutine(Climb("up"));
        }

        if(!isClimbing && Input.GetKeyDown(climbDownKey)) {
            StartCoroutine(Climb("down"));
        }

        if(Input.GetKeyUp(climbUpKey) || Input.GetKeyUp(climbDownKey)) {
            endClimb();
        }
    }

    void endClimb() {
        isClimbing = false;
        StopAllCoroutines();
    }

    IEnumerator Climb(string direction) {

        Rigidbody2D nextVineSegment = GetNextVineSegment(direction);

        if(nextVineSegment==null) {
            endClimb();
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Climb > nextVineSegment" + nextVineSegment);
        Vector2 targetPosition = nextVineSegment.position;

        // while (rb.position.y != targetPosition.y)
        // {
            rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, climbSpeed * Time.deltaTime));
            // rb.MovePosition(nextVineSegment.position);
            attachJoints(nextVineSegment);  
            yield return new WaitForSeconds(climbSecondsBetweenMove);
            StartCoroutine(Climb(direction));
        // }  
        
        
    }
    
private Rigidbody2D GetNextVineSegment(string direction) {
    int checkRadiusOffsetY = direction=="up" ? 1 : -1;
    Vector3 checkRadiusOffset = new Vector3(0,checkRadiusOffsetY);
    Vector2 checkRadiusStartPosition = grabJoint.connectedBody.transform.position + checkRadiusOffset;
    Collider2D[] colliders = Physics2D.OverlapCircleAll(checkRadiusStartPosition, 0.5f, swingableLayer);
    Collider2D closestSegment = null;
    float closestDistance = direction == "up" ? float.MaxValue : float.MinValue;

    GameObject currentVineRoot = grabJoint.connectedBody.transform.GetComponent<VineSegment>().vineRoot.gameObject;
    // Debug.Log("currentVineRoot: "+currentVineRoot.name);

    foreach (Collider2D collider in colliders)
    {
        // skip over vine segments that do not belong to the same vine;
        GameObject vineRoot = collider.transform.GetComponent<VineSegment>().vineRoot.gameObject;
        if(!GameObject.ReferenceEquals(currentVineRoot,vineRoot)) {
            continue;
        }
        float distance = Vector2.Distance(collider.transform.position, grabJoint.connectedBody.transform.position);
        if (direction == "up" && distance < closestDistance)
        {
            closestSegment = collider;
            closestDistance = distance;
        }
        else if (direction == "down" && distance > closestDistance)
        {
            closestSegment = collider;
            closestDistance = distance;
        }
    }
    return closestSegment.attachedRigidbody;
}
}
