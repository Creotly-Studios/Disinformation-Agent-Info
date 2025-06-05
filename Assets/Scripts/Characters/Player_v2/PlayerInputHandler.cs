using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputHandler : MonoBehaviour
{
    public bool attackPressed { get; private set; }

    public InputSystem_Actions InputSystemActions { get; set; }
    Player_v2 player;

    public Vector2 MovementInput { get; private set; }
    public Vector2 CameraInput { get; private set; }

    public bool jumpPressed = false;
    public bool JumpInput { get; private set; }
    public bool DashInput { get; private set; }
    public bool SprintInput { get; private set; }
    public bool InteractInput { get; private set; }
    public bool AttackInput { get; private set; }

    private void OnEnable()
    {
        player = GetComponent<Player_v2>();
        if (InputSystemActions == null)
        {
            InputSystemActions = new InputSystem_Actions();

            // Movement and Camera Input
            InputSystemActions.Player.Move.performed += i => MovementInput = i.ReadValue<Vector2>();
            InputSystemActions.Player.Move.canceled += i => MovementInput = Vector2.zero;

            InputSystemActions.Player.Look.performed += i => CameraInput = i.ReadValue<Vector2>();

            // Action Inputs
            InputSystemActions.Player.Jump.started += OnJumpInput;
            InputSystemActions.Player.Dash.started += OnDashInput;

            InputSystemActions.Player.Jump.started += ctx => jumpPressed = ctx.ReadValueAsButton();
            InputSystemActions.Player.Jump.canceled += ctx => jumpPressed = ctx.ReadValueAsButton();

            InputSystemActions.Player.Sprint.started += OnSprintInput;
            InputSystemActions.Player.Sprint.canceled += OnSprintInput;

            InputSystemActions.Player.Attack.started += OnAttackInput;
            InputSystemActions.Player.Attack.canceled += OnAttackInput;
            InputSystemActions.Player.Interact.started += OnInteractInput;
            InputSystemActions.Player.Interact.canceled += OnInteractInput;
        }
        InputSystemActions.Enable();
    }

    private void OnDisable()
    {
        InputSystemActions.Disable();
    }

    private void OnInteractInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            InteractInput = true;
        }
        else if (ctx.canceled)
        {
            InteractInput = false;
        }
    }


    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started && player.CanUseMovementInput())
        {
            JumpInput = true;
        }
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.started && player.CanUseMovementInput())
        {
            // Check if the player has enough stamina to dash
            if (player.PlayerStatistics.CanDash())
            {
                DashInput = true;
            }
            else
            {
                Debug.Log("Not enough stamina to dash!");
            }
        }
    }

    public void OnSprintInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SprintInput = true;
        }
        if (context.canceled)
        {
            SprintInput = false;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.started && player.CanUseMovementInput())
        {
            AttackInput = true;
        }
        if (context.canceled)
        {
            AttackInput = false;
        }
    }

    public void UseJumpInput() => JumpInput = false;
    public void UseDashInput() => DashInput = false;
    public void UseInteractInput() => InteractInput = false;
    public void UseAttackInput() => AttackInput = false;
}