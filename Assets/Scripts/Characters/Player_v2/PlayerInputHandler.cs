using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public bool attackPressed { get; private set; }

    public InputSystem_Actions InputSystemActions { get; set; }
    Player_v2 player;

    public Vector2 MovementInput { get; private set; }
    public Vector2 CameraInput { get; private set; }

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
            InputSystemActions.Player.Move.performed += OnMovementInput;
            InputSystemActions.Player.Look.performed += i => CameraInput = i.ReadValue<Vector2>();

            // Action Inputs
            InputSystemActions.Player.Jump.started += OnJumpInput;
            InputSystemActions.Player.Dash.started += OnDashInput;
            InputSystemActions.Player.Sprint.started += OnSprintInput;
            InputSystemActions.Player.Sprint.canceled += OnSprintInput;

            InputSystemActions.Player.Attack.started += OnAttackInput;
            InputSystemActions.Player.Attack.canceled += OnAttackInput;
            InputSystemActions.Player.Interact.performed += ctx => OnInteractInput(ctx, true);
            InputSystemActions.Player.Interact.canceled += ctx => OnInteractInput(ctx, false);
        }
        InputSystemActions.Enable();
    }

    private void OnDisable()
    {
        InputSystemActions.Disable();
    }

    public void OnMovementInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    private void OnInteractInput(InputAction.CallbackContext ctx, bool status)
    {
        if(status == true)
        {
            print("true");
        }
        InteractInput = status;
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