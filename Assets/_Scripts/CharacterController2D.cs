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

    [SerializeField]  AudioSource playerAudio;
    [SerializeField]  AudioSource vineAudio;
    [SerializeField] List<AudioClip> whoas = new List<AudioClip>();
    [SerializeField] AudioClip jumpStartSound;
    [SerializeField] AudioClip jumpStopSound;
    [SerializeField] List<AudioClip> vineStretchAudioClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> vineImpactAudioClips = new List<AudioClip>();
    [SerializeField] Animator controlsTextAnimator;
    [SerializeField] List<Rigidbody2D> ragdollParts = new List<Rigidbody2D>();
    GroundCheck groundCheck;
    private int jumpCount = 0;

    // private MultiAnimator animator;
    private MultiAnimator animator;
    private bool isGrounded = false;
    private bool isSwinging;
    
    private SwingingController swingingController;
    // private GameManager gameManager;

    
    private bool playerTookFirstSwing;

    private float moveInput;
    private bool _jump;

    private bool isHitByArrow;
    private bool isFacingLeft; 

    private void Start()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
        // rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<MultiAnimator>();
        swingingController = GetComponent<SwingingController>();
        // gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // playerAudio = GetComponentInChildren<AudioSource>();
    }

    private void Update()
    {
        if(isHitByArrow) return;
        getInput();
        handleJump();
        updateIsSwinging();
        handleFallingAnimation();
    }

    void FixedUpdate() {
        handleSpriteDirection();
        if(isSwinging) {handleSwingingMovement();}
        else {handleNormalMovement();}
    }

    void updateIsSwinging() {
        bool swingStarted = !isSwinging && swingingController.isSwinging;
        bool swingEnded = isSwinging && !swingingController.isSwinging;
        if(swingStarted) {
            playVineImpactSound();
            jumpCount = 0;
        } else if(swingEnded) {
            animator.SetBool("isFlying", true);
        }
        isSwinging = swingingController.isSwinging;
    }

    void getInput() {
        moveInput = Input.GetAxis("Horizontal");
        _jump = Input.GetKeyDown(jumpKey);
    }

    void handleSpriteDirection() {
            // Flip the sprite based on the direction of movement
        if (moveInput < 0 && !isFacingLeft) { 
            transform.localScale = new Vector3(-1f, 1f, 1f);
            isFacingLeft=true;
        }

        else if (moveInput > 0 && isFacingLeft) {
            transform.localScale = new Vector3(1f, 1f, 1f);
            isFacingLeft = false;
        }  
        // updateHingeLimits();
    }

    // void updateHingeLimits() {
    //     float baseAngle = (isFacingLeft) ? 180 : 0;
    //     float range = 50;
    //     float lowerLimit = baseAngle - range;
    //     float upperLimit = baseAngle + range;
    //     JointAngleLimits2D newLimits = new JointAngleLimits2D();
    //     newLimits.min = lowerLimit;
    //     newLimits.max = upperLimit;
    //     foreach(HingeJoint2D h in gameObject.GetComponentsInChildren<HingeJoint2D>()) {
    //         // h.useLimits = newState;
    //         h.limits = newLimits;
    //         Debug.Log(h.name + ": "+h.referenceAngle);
    //     }
    // }

    void handleNormalMovement() {
        animator.SetBool("isSwinging",false);
        // Move the character horizontally

        if(moveInput!=0 && isGrounded) {
            rb.velocity = new Vector2(moveInput * normalMoveSpeed, rb.velocity.y);
        }
        
        // Check if the character is on the ground
        if(groundCheck.isTouchingPlatform && !isGrounded) {
            jumpCount = 0;
            animator.SetBool("isFlying", false);
            isGrounded = true;
            playJumpStopSound();
        } else if(!groundCheck.isTouchingPlatform && isGrounded) {
            isGrounded=false;
        }

        if (isGrounded && !rb.freezeRotation) {
            rb.rotation = 0;
            rb.freezeRotation=true;
            // toggleRagdollParts(false);
        } else if(!isGrounded && rb.freezeRotation) {
            rb.freezeRotation=false;
            // toggleRagdollParts(true);
        }

        handleRunningAnimation();
    }

    void handleJump() {
        // Jump logic
        if (_jump)
        {
            if (isGrounded || jumpCount < maxJumps)
            {
                // rb.AddForce(new Vector2(rb.velocity.x, jumpForce));
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount++;
                playJumpStartSound();

                // Play jump animation
                animator.SetBool("isFlying", true);
            }
        }
    }

    void handleSwingingMovement() {
        if(!playerTookFirstSwing) {
            playerTookFirstSwing=true;
            controlsTextAnimator.SetTrigger("FadeOut");
        }
        animator.SetBool("isSwinging",true);
        animator.SetBool("isFlying",false);
        animator.SetBool("isFalling",false);
        
        if(moveInput!=0) {
            playVineStretchSound();
            // rb.AddForce(new Vector2(moveInput * swingingMoveSpeed, rb.velocity.y)) ;
            rb.AddForce(new Vector2(moveInput * swingingMoveSpeed, 0)) ;
        }
    }

    void handleRunningAnimation() {
        if(isGrounded && moveInput!=0) {
            animator.SetBool("isRunning", true);
        } else {
            animator.SetBool("isRunning", false);
        }
    }

    void handleFallingAnimation() {
        if(isSwinging) return;
        if(!isGrounded && rb.velocity.y<fallingInitSpeed) {
            animator.SetBool("isFlying", false);
            animator.SetBool("isFalling", true);
            playWhoaSound();

        } else {
            animator.SetBool("isFalling", false);
        }
    }

    void toggleRagdollParts(bool newState) {
        foreach(Rigidbody2D rb in ragdollParts) {
            rb.bodyType = (newState) ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
        }
    }

    void playVineStretchSound() {
        // Debug.Log("VineStretchSound");
        if(vineAudio.isPlaying) return;
        if(Random.Range(0f,1f)>0.05f) return;
        AudioClip rndSound = vineStretchAudioClips[Random.Range(0,vineStretchAudioClips.Count)];
        vineAudio.PlayOneShot(rndSound);
    }

    void playVineImpactSound() {
        // if(Random.Range(0f,1f)>0.05f) return;
        AudioClip rndSound = vineImpactAudioClips[Random.Range(0,vineImpactAudioClips.Count)];
        vineAudio.PlayOneShot(rndSound);
    }


    void playJumpStartSound() {
        // Debug.Log("JumpStartSound");
        playerAudio.PlayOneShot(jumpStartSound);
    }

    void playJumpStopSound() {
        // Debug.Log("JumpStopSound");
        playerAudio.Stop();
        playerAudio.PlayOneShot(jumpStopSound);
    }

    void playWhoaSound() {
        // Debug.Log("whoaSound");
        if(playerAudio.isPlaying) return;
        AudioClip rndWhoaSound = whoas[Random.Range(0,whoas.Count)];
        playerAudio.PlayOneShot(rndWhoaSound);
    }

    public void hitByArrow() {
        isHitByArrow=true;
        onDeath();
        playJumpStopSound(); //temp hit by arrow sound
        animator.SetBool("isFalling", true);
    }

    public void onDeath() {
        isSwinging=false;
        swingingController.handleSwingRelease();
        swingingController.enabled=false;
    }
}

