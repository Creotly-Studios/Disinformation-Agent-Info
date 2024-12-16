using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public InputSystem_Actions InputSystemActions { get; set; }

    public Vector2 currentMovementInput;
    public bool isMovementPressed;
    

    public bool jumpPressed = false;
    public bool attackPressed = false;
    public bool interactPressed = false;
    public bool pausePressed = false;
    public bool sprintPressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        instance = this;
        InputSystemActions = new InputSystem_Actions();
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        InputSystemActions.Player.Move.started += ctx => OnMove(ctx);
        InputSystemActions.Player.Move.performed += ctx => OnMove(ctx);
        InputSystemActions.Player.Move.canceled += ctx => OnMove(ctx);
        
        InputSystemActions.Player.Jump.started += ctx => OnJump(ctx);
        InputSystemActions.Player.Jump.canceled += ctx => OnJump(ctx);
        
        InputSystemActions.Player.Sprint.started += ctx => OnSprint(ctx);
        InputSystemActions.Player.Sprint.canceled += ctx => OnSprint(ctx);
        
        InputSystemActions.Player.Attack.started += ctx => OnAttack(ctx);
        InputSystemActions.Player.Attack.canceled += ctx => OnAttack(ctx);
        
        InputSystemActions.Player.Interact.started += ctx => OnInteract(ctx);
        InputSystemActions.Player.Interact.canceled += ctx => OnInteract(ctx);
        
        InputSystemActions.Player.Pause.started += ctx => OnPause(ctx, false);
        InputSystemActions.Player.Pause.canceled += ctx => OnPause(ctx, true);
    }

    private void OnPause(InputAction.CallbackContext ctx, bool y)
    {
        pausePressed = y;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        interactPressed = ctx.ReadValueAsButton();
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        attackPressed = ctx.ReadValueAsButton();
    }

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        sprintPressed = ctx.ReadValueAsButton();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        jumpPressed = ctx.ReadValueAsButton();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        currentMovementInput = ctx.ReadValue<Vector2>();
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    private void OnEnable()
    {
        InputSystemActions.Enable();
    }

    private void OnDisable()
    {
        InputSystemActions.Disable();
    }
}
