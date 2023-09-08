/*
090723
PlayerGroundhandler

Monitors multiple groundcheck triggers and controls player animation.


init outline:

- if player bottom groundcheck isTouchingPlatform, and no others are, then player is upright:
    - disable crawling/standing anims;
    - set playerContoller.isGrounded==true and playerContoller.isUpright==true;
        - playerController should re-enable movement;

- if back-upper || back-lower are touchingPlatform:
    - flip player around to face platform;

- if front-upper && front-lower are touchingPlatform:
    - set playerController.isGrounded==true and isUpright==false;
        - playerController should disable movement;
    - start standUp anim;
    
- if front-upper is touchingPlatform && front-lower is not touchingPlatform:
    - set playerController.isGrounded==true and isUpright==false;
    - start crawl anim;

*/

using UnityEngine;


public class PlayerGroundHandler : MonoBehaviour
{
    [SerializeField] CharacterController2D playerController;
    [SerializeField] Animator playerAnimator;
    [SerializeField] GroundCheck frontUpper;
    [SerializeField] GroundCheck frontLower;
    [SerializeField] GroundCheck backUpper;
    [SerializeField] GroundCheck backLower;
    [SerializeField] GroundCheck bottom;

    public bool isGrounded = true;
    public bool isFallen = false;
    public bool isUpright = true;
    public bool isCrawling = false;
    public bool isStandingUp = false;


    // Update is called once per frame
    void Update()
    {
        updateGroundChecks();
    }

    void updateGroundChecks() {
        /* updates state according to ground checks*/
        checkIsGrounded();
        //if not grounded, exit;
        if(!isGrounded) { return; }
        checkIsUpright();
        //if isUpright, exit;
        if(isUpright) {return;}
        checkIsFallen();
    }

    /*
       =============== Grounded ===============
    */

    void checkIsGrounded() {
        /*returns true if any groundCheck isTouchingPlatform;*/
        bool groundedCheck = frontUpper.isTouchingPlatform || frontLower.isTouchingPlatform || backUpper.isTouchingPlatform || backLower.isTouchingPlatform || bottom.isTouchingPlatform;
        if(!groundedCheck && !isGrounded) {return;}
        else if(!groundedCheck && isGrounded) {onGroundedStart();}
        else if(groundedCheck && !isGrounded) {onGroundedEnd();}
    }

    void onGroundedStart() {
        isGrounded = true;
    }

    void onGroundedEnd() {
        isGrounded = false;
        isFallen = false;
        isUpright = false;
        isCrawling = false;
        isStandingUp = false;
    }

    /*
        =============== Upright ===============
    */

    void checkIsUpright() {
        /* returns true if only the bottom groundcheck is touchingPlatform*/
        bool uprightCheck = bottom.isTouchingPlatform && !isFallen;
        if(uprightCheck && !isUpright) {handleOnUprightStart();}
        else if (!uprightCheck && isUpright) {handleOnUprightEnd();}
    }

    void handleOnUprightStart() {
        isUpright = true;
        //todo: stop crawling/standing anim;
        isStandingUp = false;
        isCrawling = false;
        playerController.enableInput();
    }

    void handleOnUprightEnd() {
        isUpright = false;
    }

    /*
        =============== Fallen ===============
    */

    void checkIsFallen() {
        /* returns true if any front or back groundChecks are touchingPlatform*/
        bool fallenCheck = frontUpper.isTouchingPlatform || frontLower.isTouchingPlatform || backUpper.isTouchingPlatform || backLower.isTouchingPlatform;
        if(fallenCheck && !isFallen) {handleOnFallenStart();}
        else if(!fallenCheck && isFallen) {handleOnFallenEnd();}
    }

    void handleOnFallenStart() {
        isFallen = true;
        playerController.disableInput();
        bool upperTouching = frontUpper.isTouchingPlatform || backUpper.isTouchingPlatform;
        bool lowerTouching = frontLower.isTouchingPlatform || backLower.isTouchingPlatform;
        //if player is on their back, flip the player direction;
        bool isOnBack = backUpper.isTouchingPlatform || backLower.isTouchingPlatform;
        if(isOnBack) {
            playerController.flipPlayerFacingDirection();
        }
        //if front upper and front lower are touching platform, 
        if(upperTouching && lowerTouching) {
            isStandingUp = true;
            playerAnimator.SetBool("isStandingUp", true);
        }
        //if only front is touching,
        else if(upperTouching && !lowerTouching) {
            isCrawling = true;
            playerAnimator.SetBool("isCrawling", true);
        }
    }

    void handleOnFallenEnd() {
        isFallen = false;
    }


}
