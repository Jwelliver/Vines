
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
    [SerializeField] PauseMenu pauseMenu;



    void Awake()
    {
        customInput = new CustomInput();
        if (GameContext.ActiveSettings.useTouchScreenControls)
        {
            mobileControls.SetActive(true);
        }

    }
    void OnEnable()
    {
        customInput.Enable();
        customInput.GamePlay.HorizontalMovement.performed += OnHorizontalMovement;
        customInput.GamePlay.HorizontalMovement.canceled += OnHorizontalMovementCanceled;
        customInput.GamePlay.Jump.performed += OnJump;
        customInput.GamePlay.Jump.canceled += OnJumpCanceled;
        customInput.GamePlay.Grab.performed += OnGrabAttempt;
        customInput.GamePlay.Grab.canceled += OnGrabAttemptCanceled;
        customInput.GamePlay.ClimbUp.performed += ctx => isAttemptingClimbUp = true;
        customInput.GamePlay.ClimbUp.canceled += ctx => isAttemptingClimbUp = false;
        customInput.GamePlay.ClimbDown.performed += ctx => isAttemptingClimbDown = true;
        customInput.GamePlay.ClimbDown.canceled += ctx => isAttemptingClimbDown = false;
        customInput.GamePlay.Restart.performed += ctx => { levelLoader.reloadCurrentLevel(); Debug.Log("Restart btn Pressed"); };
        customInput.GamePlay.Pause.performed += ctx => { pauseMenu.OnPauseButtonPressed(); Debug.Log("Pause button pressed"); };
        customInput.GamePlay.ShowFPS.performed += ctx => FramesPerSecond.OnFpsButtonPressed();
    }
    void OnDisable()
    {
        customInput.Disable();
        customInput.GamePlay.HorizontalMovement.performed -= OnHorizontalMovement;
        customInput.GamePlay.HorizontalMovement.canceled -= OnHorizontalMovementCanceled;
        customInput.GamePlay.Jump.performed -= OnJump;
        customInput.GamePlay.Jump.canceled -= OnJumpCanceled;
        customInput.GamePlay.Grab.performed -= OnGrabAttempt;
        customInput.GamePlay.Grab.canceled -= OnGrabAttemptCanceled;
        customInput.GamePlay.ClimbUp.performed -= ctx => isAttemptingClimbUp = true;
        customInput.GamePlay.ClimbUp.canceled -= ctx => isAttemptingClimbUp = false;
        customInput.GamePlay.ClimbDown.performed -= ctx => isAttemptingClimbDown = true;
        customInput.GamePlay.ClimbDown.canceled -= ctx => isAttemptingClimbDown = false;
        customInput.GamePlay.Restart.performed -= ctx => levelLoader.reloadCurrentLevel();
        customInput.GamePlay.Pause.performed -= ctx => pauseMenu.OnPauseButtonPressed();
        customInput.GamePlay.ShowFPS.performed -= ctx => FramesPerSecond.OnFpsButtonPressed();
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
        // Debug.Log("GrabAttempt");
        if (!SwingingController.isSwinging) hasAttemptedGrab = true;
    }

    void OnGrabAttemptCanceled(InputAction.CallbackContext callbackContext)
    {
        // Debug.Log("GrabCancel");
        if (SwingingController.isSwinging) hasReleased = true;
    }




}
