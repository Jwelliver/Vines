using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
// using UnityEngine.InputSystem;


public class CharacterController2D : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb; //player main rb
    [SerializeField] float normalMoveSpeed = 5f;
    [SerializeField] float swingingMoveSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] int maxJumps = 2;
    [SerializeField] float fallingInitSpeed = -10f;
    [SerializeField] Animator controlsTextAnimator;
    [SerializeField] List<TargetRotation> ragdollParts = new List<TargetRotation>();
    [SerializeField] SwingNetAttack swingNetAttack;
    // [SerializeField] float standupSpeed = 5f; //the speed at which the player is kept upright when grounded //* Not in Use 100323
    [SerializeField] GroundCheck groundCheck;
    [SerializeField] SfxHandler sfx;


    private int jumpCount = 0;

    private Animator animator;
    private bool isGrounded = false;
    private bool isSwinging;

    private SwingingController swingingController;

    private bool playerTookFirstSwing;

    private bool isHitByArrow;
    private bool isFacingLeft;
    private FlipScaleX[] flipScaleXArr;
    private GameManager gameManager;

    enum PhysicsAnimationBlendStrength
    {
        High = 100, Med = 20, Low = 5, None = 0
    }

    void Awake()
    {
        // input = new CustomInput();
        animator = GetComponent<Animator>();
        swingingController = GetComponent<SwingingController>();
        flipScaleXArr = transform.parent.GetComponentsInChildren<FlipScaleX>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnDisable()
    {
        // Disable ragdoll parts
        foreach (TargetRotation i in ragdollParts)
        {
            i.transform.root.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isHitByArrow) return;

        // handleAttack(); //090823 disabled for prod; bugs/net not in use
        updateIsSwinging();
        handleFallingAnimation();
    }

    void FixedUpdate()
    {
        handleJump();
        handleSpriteDirection();
        if (isSwinging) { handleSwingingMovement(); }
        else { handleNormalMovement(); }
    }

    // void handleAttack() // * 092523 Not in use
    // {
    //     if (isAttacking)
    //     {
    //         swingNetAttack.attack();
    //         isAttacking = false;
    //     }
    // }

    void updateIsSwinging()
    {
        bool swingStarted = !isSwinging && SwingingController.isSwinging;
        bool swingEnded = isSwinging && !SwingingController.isSwinging;
        if (swingStarted)
        {
            onSwingStart();
        }
        else if (swingEnded)
        {
            onSwingEnd();
        }
        isSwinging = SwingingController.isSwinging;
    }

    void onSwingStart()
    {
        sfx.vineSFX.playVineImpactSound();
        jumpCount = 0;
        setTargetRotationForceInRagdollParts(PhysicsAnimationBlendStrength.None);
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
        if (PlayerInput.moveInput < 0 && !isFacingLeft)
        {
            setPlayerFacingDirection("left");
        }
        else if (PlayerInput.moveInput > 0 && isFacingLeft)
        {
            setPlayerFacingDirection("right");
        }
    }

    void setPlayerFacingDirection(string newDir = "left")
    {
        isFacingLeft = newDir == "left";
        foreach (FlipScaleX flip in flipScaleXArr)
        {
            flip.FlipX(newDir);
        }
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

        // Check if the character is on the ground
        if (groundCheck.isTouchingPlatform && !isGrounded) { onGroundTouched(); }
        else if (!groundCheck.isTouchingPlatform && isGrounded) { onGroundLeft(); }

        if (isGrounded)
        {
            keepPlayerUpright();
            // Move the character horizontally
            if (PlayerInput.moveInput != 0)
            {
                rb.velocity = new Vector2(PlayerInput.moveInput * normalMoveSpeed, rb.velocity.y);
            }
            // Deccelerate
            if (PlayerInput.moveInput == 0 && Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                float slowdownFactor = rb.velocity.x > 0f ? -normalMoveSpeed : normalMoveSpeed;
                rb.velocity += new Vector2(slowdownFactor * Time.fixedDeltaTime, 0);
            }
        }

        //if player is in platform landing zone, move upright
        if (!isGrounded && !isSwinging && groundCheck.isTouchingLandZone)
        {
            keepPlayerUpright();
        }

        handleRunningAnimation();
    }

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
        //Enable TargetRotation
        setTargetRotationForceInRagdollParts(PhysicsAnimationBlendStrength.High);
        isGrounded = true;
    }

    void onGroundLeft()
    {
        isGrounded = false;
    }

    void handleJump()
    {
        // Jump logic
        if (PlayerInput.hasAttemptedJump)
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
            PlayerInput.hasAttemptedJump = false;
        }
    }

    void handleSwingingMovement()
    {
        if (PlayerInput.moveInput != 0)
        {
            // sfx.vineSFX.playVineStretchSound(); // TODO: 100123 removed for testing physicallybased sfx
            // rb.AddForce(new Vector2(moveInput * swingingMoveSpeed, rb.velocity.y)) ;
            rb.AddForce(new Vector2(PlayerInput.moveInput * swingingMoveSpeed, 0));
        }
    }

    void handleRunningAnimation()
    {
        if (isGrounded && PlayerInput.moveInput != 0)
        {
            animator.SetBool("isRunning", true);
            setTargetRotationForceInRagdollParts(PhysicsAnimationBlendStrength.High);
        }
        else if (isGrounded && PlayerInput.moveInput == 0 && Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            animator.SetBool("isRunning", true);
            setTargetRotationForceInRagdollParts(PhysicsAnimationBlendStrength.Low);
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
            setTargetRotationForceInRagdollParts(PhysicsAnimationBlendStrength.High);
            animator.SetBool("isFlying", false);
            animator.SetBool("isFalling", true);
            sfx.playerSFX.playWhoaSound();

        }
        else
        {
            animator.SetBool("isFalling", false);
        }
    }

    void setTargetRotationForceInRagdollParts(PhysicsAnimationBlendStrength strength)
    {
        foreach (TargetRotation i in ragdollParts)
        {
            i.setForce((int)strength);
        }
    }

    // void toggleTargetRotationForceInRagdollParts(bool newState) {
    //     foreach(TargetRotation i in ragdollParts) {
    //         i.rotationEnabled = newState;
    //     }
    // }

    bool IsCollisionFatal(Collider2D playerColliderHit)
    {
        // determine whether a collision (i.e. with an arrow) has hit a vital component, and whether player should die

        //* 092823 crude quick way to only trigger a fatal shot if the collider hit i NOT one of the legs;
        // ... i.e. if one of the legs was hit, just ignore the hit.
        foreach (TargetRotation target in ragdollParts)
        {
            if (target.gameObject.GetComponent<Collider2D>() == playerColliderHit)
            {
                return false;
            }
        }
        return true;
    }



    public void hitByArrow(Collider2D playerColliderHit)
    {
        if (IsCollisionFatal(playerColliderHit))
        {
            isHitByArrow = true;
            isSwinging = false;
            swingingController.handleSwingRelease(true);
            swingingController.enabled = false;
            sfx.playerSFX.playJumpStopSound(); //temp hit by arrow sound
            animator.SetBool("isFalling", true);
        }

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

    public void OnDeath() //TODO: move to CharacterController
    {
        sfx.playerSFX.StopAllAudio();
        // Rigidbody2D playerRb = other.transform.root.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Static;
        // disable player sprite
        // playerBody.SetActive(false);
        rb.transform.root.gameObject.SetActive(false);

        gameManager.OnPlayerDied();
    }


}

