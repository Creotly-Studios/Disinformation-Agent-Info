using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public bool attackPressed {get; private set;}

    public InputSystem_Actions InputSystemActions { get; set; }

    public Vector2 MovementInput {get; private set;}
    public bool JumpInput {get; private set;}
    public bool DashInput { get; private set; }
    public bool SprintInput { get; private set; }
    public bool InteractInput { get; private set; }
    public bool AttackInput { get; private set; }

    private void Awake()
    {
        InputSystemActions = new InputSystem_Actions();
    }
    
    private void Start() {
        InputSystemActions.Player.Jump.started += ctx => OnJumpInput(ctx);
        InputSystemActions.Player.Jump.canceled += ctx => OnJumpInput(ctx);
        InputSystemActions.Player.Interact.started += _ => {};

    }

    public void OnMovementInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            JumpInput = true;
        }
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            DashInput = true;
        }
    }

    public void OnsprintInput(InputAction.CallbackContext context)
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

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            InteractInput = true;
        }
        if (context.canceled)
        {
            InteractInput = false;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AttackInput = true;
        }
        // if (context.canceled)
        // {
        //     AttackInput = false;
        // }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        attackPressed = ctx.ReadValueAsButton();
    }


    public void UseJumpInput() => JumpInput = false;
    public void UseDashInput() => DashInput = false;
    public void UseeInteractInput() => InteractInput = false;
    public void UseAttackInput() => AttackInput = false;
}
