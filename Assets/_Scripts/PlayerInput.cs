
using UnityEngine;
using UnityEngine.InputSystem;

/* // TODO: May want to rename; Turns out the new input system package contains a component called PlayerInput which performs a similar role*/
public class PlayerInput : MonoBehaviour
{

    public static CustomInput customInput;
    public static bool inputEnabled = true;

    // TODO: may be worth renaming these bools to specifically represent key presses. e.g. jumpKeyPressed ?
    // Character Movement
    public static float moveInput;
    public static bool hasAttemptedJump;
    // public static bool isAttacking; //Not in use

    // Vine Grab/Release/Climb
    public static bool hasAttemptedGrab;
    public static bool hasReleased;
    public static bool isAttemptingClimbUp;
    public static bool isAttemptingClimbDown;

    [SerializeField] GameObject mobileControls; // TODO: temp placement here; move to dedicated class
    [SerializeField] LevelLoader levelLoader; //TODO: move



    void Awake()
    {
        customInput = new CustomInput();
        if (Application.isMobilePlatform)
        {
            mobileControls.SetActive(true);
        }

    }
    void OnEnable()
    {
        customInput.Enable();
        customInput.Player.HorizontalMovement.performed += OnHorizontalMovement;
        customInput.Player.HorizontalMovement.canceled += OnHorizontalMovementCanceled;
        customInput.Player.Jump.performed += OnJump;
        customInput.Player.Jump.canceled += OnJumpCanceled;
        customInput.Player.Grab.performed += OnGrabAttempt;
        customInput.Player.Grab.canceled += OnGrabAttemptCanceled;
        customInput.Player.ClimbUp.performed += ctx => isAttemptingClimbUp = true;
        customInput.Player.ClimbUp.canceled += ctx => isAttemptingClimbUp = false;
        customInput.Player.ClimbDown.performed += ctx => isAttemptingClimbDown = true;
        customInput.Player.ClimbDown.canceled += ctx => isAttemptingClimbDown = false;
        customInput.Player.Restart.performed += ctx => levelLoader.reloadCurrentLevel();
    }
    void OnDisable()
    {
        customInput.Disable();
        customInput.Player.HorizontalMovement.performed -= OnHorizontalMovement;
        customInput.Player.HorizontalMovement.canceled -= OnHorizontalMovementCanceled;
        customInput.Player.Jump.performed -= OnJump;
        customInput.Player.Jump.canceled -= OnJumpCanceled;
        customInput.Player.Grab.performed -= OnGrabAttempt;
        customInput.Player.Grab.canceled -= OnGrabAttemptCanceled;
        customInput.Player.ClimbUp.performed -= ctx => isAttemptingClimbUp = true;
        customInput.Player.ClimbUp.canceled -= ctx => isAttemptingClimbUp = false;
        customInput.Player.ClimbDown.performed -= ctx => isAttemptingClimbDown = true;
        customInput.Player.ClimbDown.canceled -= ctx => isAttemptingClimbDown = false;
        customInput.Player.Restart.performed -= ctx => levelLoader.reloadCurrentLevel();
        customInput = null;
    }

    public static void DisableInput()
    {
        inputEnabled = false;
    }

    public static void EnableInput()
    {
        inputEnabled = true;
    }

    private void OnHorizontalMovement(InputAction.CallbackContext callbackContext)
    {
        if (!inputEnabled) { moveInput = 0; return; }
        moveInput = callbackContext.ReadValue<float>();
    }

    private void OnHorizontalMovementCanceled(InputAction.CallbackContext callbackContext)
    {
        moveInput = 0f;
    }

    private void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (!inputEnabled) { moveInput = 0; hasAttemptedJump = false; return; }
        hasAttemptedJump = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext callbackContext)
    {
        hasAttemptedJump = false;
    }

    void OnGrabAttempt(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("GrabAttempt");
        if (!SwingingController.isSwinging) hasAttemptedGrab = true;
    }

    void OnGrabAttemptCanceled(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("GrabCancel");
        if (SwingingController.isSwinging) hasReleased = true;
    }




}