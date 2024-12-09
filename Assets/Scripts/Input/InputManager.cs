using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private InputSystem_Actions _inputSystemActions;

    public Vector2 currentMovementInput;
    private bool _isMovementPressed;
    
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
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        currentMovementInput = ctx.ReadValue<Vector2>();
        _isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
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
