using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb; //player main rb
    [SerializeField] float normalMoveSpeed = 5f;
    [SerializeField] float swingingMoveSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] int maxJumps = 2;
    [SerializeField] float fallingInitSpeed = -10f;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] Animator controlsTextAnimator;
    [SerializeField] List<TargetRotation> ragdollParts = new List<TargetRotation>();
    [SerializeField] SwingNetAttack swingNetAttack;
    [SerializeField] float standupSpeed = 5f; //the speed at which the player is kept upright when grounded
    [SerializeField] GroundCheck groundCheck;
    [SerializeField] SfxHandler sfx;


    Transform myTransform;
    private int jumpCount = 0;

    private Animator animator;
    private bool isGrounded = false;
    private bool isSwinging;

    private SwingingController swingingController;
    // private GameManager gameManager;


    private bool playerTookFirstSwing;

    private bool inputEnabled = true;

    private float moveInput;
    private bool _jump;
    private bool isAttacking;

    private bool isHitByArrow;
    private bool isFacingLeft;

    // enum RagdollAnimationBlendStrengh {
    //     High=100,Med=50,Low=5,None=0
    // }

    // RagdollAnimationBlendStrengh ragdollAnimationBlendStrengh;

    float ragdollAnimationBlendHigh = 100f;
    float ragdollAnimationBlendMed = 20f;
    float ragdollAnimationBlendLow = 5f;
    float ragdollAnimationBlendNone = 0f;

    void Awake()
    {
        myTransform = transform;
    }


    private void Start()
    {
        // groundCheck = GetComponentInChildren<GroundCheck>();
        // rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        swingingController = GetComponent<SwingingController>();
        // gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // playerAudio = GetComponentInChildren<AudioSource>();
    }

    private void Update()
    {
        if (isHitByArrow) return;
        getInput();
        handleJump();
        // handleAttack(); //090823 disabled for prod; bugs/net not in use
        updateIsSwinging();
        handleFallingAnimation();
    }

    void FixedUpdate()
    {
        handleSpriteDirection();
        if (isSwinging) { handleSwingingMovement(); }
        else { handleNormalMovement(); }
    }

    void getInput()
    {
        if (!inputEnabled)
        {
            moveInput = 0f;
            return;
        }
        moveInput = Input.GetAxis("Horizontal");
        _jump = Input.GetKeyDown(jumpKey);
        isAttacking = Input.GetMouseButtonDown(0);
    }

    void handleAttack()
    {
        if (isAttacking)
        {
            swingNetAttack.attack();
            isAttacking = false;
        }
    }

    void updateIsSwinging()
    {
        bool swingStarted = !isSwinging && swingingController.isSwinging;
        bool swingEnded = isSwinging && !swingingController.isSwinging;
        if (swingStarted)
        {
            onSwingStart();
        }
        else if (swingEnded)
        {
            onSwingEnd();
        }
        isSwinging = swingingController.isSwinging;
    }

    void onSwingStart()
    {

        sfx.vineSFX.playVineImpactSound();
        jumpCount = 0;
        setTargetRotationForceInRagdollParts(ragdollAnimationBlendNone);
        animator.SetBool("isSwinging", true);
        animator.SetBool("isFlying", false);
        animator.SetBool("isFalling", false);
        if (!playerTookFirstSwing)
        {
            playerTookFirstSwing = true;
            controlsTextAnimator.SetTrigger("FadeOut");
        }
    }

    void onSwingEnd()
    {
        animator.SetBool("isFlying", true);
    }

    void handleSpriteDirection()
    {
        // Flip the sprite based on the direction of movement
        if (moveInput < 0 && !isFacingLeft)
        {
            setPlayerFacingDirection("left");
        }
        else if (moveInput > 0 && isFacingLeft)
        {
            setPlayerFacingDirection("right");
        }
    }

    void setPlayerFacingDirection(string newDir = "left")
    {
        isFacingLeft = newDir == "left";
        float xValue = isFacingLeft ? -1f : 1f;
        myTransform.localScale = new Vector3(xValue, 1f, 1f);
    }

    public void flipPlayerFacingDirection()
    {
        /* toggles player facing direction between left and right */
        string newDir = isFacingLeft ? "right" : "left";
        setPlayerFacingDirection(newDir);
    }

    void handleNormalMovement()
    {
        animator.SetBool("isSwinging", false);
        // Move the character horizontally
        if (moveInput != 0 && isGrounded)
        {
            rb.velocity = new Vector2(moveInput * normalMoveSpeed, rb.velocity.y);
        }

        //if player is in platform landing zone, move upright
        if (!isGrounded && !isSwinging && groundCheck.isTouchingLandZone)
        {
            keepPlayerUpright();
        }

        // Check if the character is on the ground
        if (groundCheck.isTouchingPlatform && !isGrounded)
        {
            onGroundTouched();
        }
        else if (!groundCheck.isTouchingPlatform && isGrounded)
        {
            onGroundLeft();
        }

        if (isGrounded)
        {
            keepPlayerUpright();
        }

        handleRunningAnimation();
    }

    // void keepPlayerUpright() {
    //     Quaternion targetRotation = Quaternion.Euler(0, 0, 0); // upright rotation
    //     float str = Mathf.Min(standupSpeed * Time.deltaTime, 1);
    //     float targetRotationAngle = targetRotation.eulerAngles.z;
    //     float nextRotation = Mathf.Lerp(rb.rotation, targetRotationAngle, str);
    //     rb.MoveRotation(nextRotation);
    // }

    void keepPlayerUpright()
    {
        if (isHitByArrow) return;
        // standupSpeed = 360; // Set this speed to your needs
        float currentZ = rb.rotation;
        float targetZ = 0;

        // Calculate the shortest distance to the target rotation
        float difference = Mathf.DeltaAngle(currentZ, targetZ);

        // Determine the amount to rotate by using the minimum between the desired amount and the maximum possible amount.
        float rotationAmount = Mathf.MoveTowards(0, difference, 360 * Time.deltaTime);

        // Apply the rotation to the Rigidbody.
        rb.MoveRotation(currentZ + rotationAmount);
    }

    void onGroundTouched()
    {
        //reset jumps
        jumpCount = 0;
        //stop flying anim
        animator.SetBool("isFlying", false);
        sfx.playerSFX.playJumpStopSound();
        //remove angular velocity and freeze rotation
        rb.angularVelocity = 0f;
        //reset angular velocity for ragdoll parts (e.g. legs)
        foreach (TargetRotation i in ragdollParts)
        {
            i.resetAngularVelocity();
        }
        isGrounded = true;
    }

    void onGroundLeft()
    {
        isGrounded = false;
    }

    void handleJump()
    {
        // Jump logic
        if (_jump)
        {
            if (isGrounded || jumpCount < maxJumps)
            {
                // rb.AddForce(new Vector2(rb.velocity.x, jumpForce));
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount++;
                sfx.playerSFX.playJumpStartSound();

                // Play jump animation
                animator.SetBool("isFlying", true);
            }
        }
    }

    void handleSwingingMovement()
    {
        if (moveInput != 0)
        {
            sfx.vineSFX.playVineStretchSound();
            // rb.AddForce(new Vector2(moveInput * swingingMoveSpeed, rb.velocity.y)) ;
            rb.AddForce(new Vector2(moveInput * swingingMoveSpeed, 0));
        }
    }

    void handleRunningAnimation()
    {
        if (isGrounded && moveInput != 0)
        {
            animator.SetBool("isRunning", true);
            setTargetRotationForceInRagdollParts(ragdollAnimationBlendHigh);
        }
        else if (isGrounded && moveInput == 0 && Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            animator.SetBool("isRunning", true);
            setTargetRotationForceInRagdollParts(ragdollAnimationBlendMed);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    void handleFallingAnimation()
    {
        if (isSwinging) return;
        if (!isGrounded && rb.velocity.y < fallingInitSpeed)
        {
            setTargetRotationForceInRagdollParts(ragdollAnimationBlendMed);
            animator.SetBool("isFlying", false);
            animator.SetBool("isFalling", true);
            sfx.playerSFX.playWhoaSound();

        }
        else
        {
            animator.SetBool("isFalling", false);
        }
    }

    void setTargetRotationForceInRagdollParts(float newForce)
    {
        foreach (TargetRotation i in ragdollParts)
        {
            i.setForce(newForce);
        }
    }

    // void toggleTargetRotationForceInRagdollParts(bool newState) {
    //     foreach(TargetRotation i in ragdollParts) {
    //         i.rotationEnabled = newState;
    //     }
    // }



    public void hitByArrow()
    {
        isHitByArrow = true;
        isSwinging = false;
        swingingController.handleSwingRelease(true);
        swingingController.enabled = false;
        sfx.playerSFX.playJumpStopSound(); //temp hit by arrow sound
        animator.SetBool("isFalling", true);
    }

    void OnDestroy()
    {
        foreach (TargetRotation i in ragdollParts)
        {
            if (i != null)
            {
                Destroy(i.transform.root.gameObject);
            }

        }
    }

    void OnDisable()
    {
        foreach (TargetRotation i in ragdollParts)
        {
            i.transform.root.gameObject.SetActive(false);
        }
    }


    public void onDeath()
    {

    }

    public void disableInput()
    {
        inputEnabled = false;
    }

    public void enableInput()
    {
        inputEnabled = true;
    }
}

