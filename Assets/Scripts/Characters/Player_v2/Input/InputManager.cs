using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public InputSystem_Actions InputSystemActions { get; private set; }

    public Vector2 currentMovementInput;
    public bool isMovementPressed;

    public bool jumpPressed = false;
    public bool skipPressed;
    public bool attackPressed = false;
    public bool sprintPressed;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
        instance = this;

        InputSystemActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        if (InputSystemActions == null)
        {
            Debug.LogWarning("InputSystemActions was null in OnEnable. Initializing now.");
            InputSystemActions = new InputSystem_Actions();
        }

        // Subscribe to input events
        InputSystemActions.Player.Move.started += OnMove;
        InputSystemActions.Player.Move.performed += OnMove;
        InputSystemActions.Player.Move.canceled += OnMove;

        InputSystemActions.Player.Jump.started += OnJump;
        InputSystemActions.Player.Jump.canceled += OnJump;

        InputSystemActions.Player.Sprint.started += OnSprint;
        InputSystemActions.Player.Sprint.canceled += OnSprint;

        InputSystemActions.Player.Attack.started += OnAttack;
        InputSystemActions.Player.Attack.canceled += OnAttack;

        InputSystemActions.Enable();
    }

    private void OnDisable()
    {
        if (InputSystemActions != null)
        {
            InputSystemActions.Disable();
        }
    }

    private void OnDestroy()
    {
        // Clean up input actions when the object is destroyed
        if (InputSystemActions != null)
        {
            InputSystemActions.Player.Move.started -= OnMove;
            InputSystemActions.Player.Move.performed -= OnMove;
            InputSystemActions.Player.Move.canceled -= OnMove;

            InputSystemActions.Player.Jump.started -= OnJump;
            InputSystemActions.Player.Jump.canceled -= OnJump;

            InputSystemActions.Player.Sprint.started -= OnSprint;
            InputSystemActions.Player.Sprint.canceled -= OnSprint;

            InputSystemActions.Player.Attack.started -= OnAttack;
            InputSystemActions.Player.Attack.canceled -= OnAttack;

            InputSystemActions.Dispose(); // Dispose of the input system to free resources
        }
    }

    // Input event handlers
    private void OnMove(InputAction.CallbackContext ctx)
    {
        currentMovementInput = ctx.ReadValue<Vector2>();
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        jumpPressed = ctx.ReadValueAsButton();
    }

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        sprintPressed = ctx.ReadValueAsButton();
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        attackPressed = ctx.ReadValueAsButton();
    }
}