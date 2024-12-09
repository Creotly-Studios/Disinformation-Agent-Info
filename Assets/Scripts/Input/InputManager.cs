using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private InputSystem_Actions _inputSystemActions;

    public Vector2 currentMovementInput;
    public bool isMovementPressed = false;
    public bool sprintPressed = false;
    
    public bool jumpPressed = false;
    public bool attackPressed = false;
    public bool interactPressed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        instance = this;
        _inputSystemActions = new InputSystem_Actions();
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        _inputSystemActions.Player.Move.started += ctx => OnMove(ctx);
        _inputSystemActions.Player.Move.performed += ctx => OnMove(ctx);
        _inputSystemActions.Player.Move.canceled += ctx => OnMove(ctx);
        
        _inputSystemActions.Player.Jump.started += ctx => OnJump(ctx);
        _inputSystemActions.Player.Jump.canceled += ctx => OnJump(ctx);
        
        _inputSystemActions.Player.Sprint.started += ctx => OnSprint(ctx);
        _inputSystemActions.Player.Sprint.canceled += ctx => OnSprint(ctx);
        
        _inputSystemActions.Player.Attack.started += ctx => OnAttack(ctx);
        _inputSystemActions.Player.Attack.canceled += ctx => OnAttack(ctx);
        
        _inputSystemActions.Player.Interact.started += ctx => OnInteract(ctx);
        _inputSystemActions.Player.Interact.canceled += ctx => OnInteract(ctx);
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
        _inputSystemActions.Enable();
    }

    private void OnDisable()
    {
        _inputSystemActions.Disable();
    }
}
