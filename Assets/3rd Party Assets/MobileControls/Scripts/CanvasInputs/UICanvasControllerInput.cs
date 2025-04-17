using UnityEngine;

public class UICanvasControllerInput : MonoBehaviour
{

    [Header("Output")]
    public PlayerInputHandler inputs;

    public void VirtualMoveInput(Vector2 virtualMoveDirection)
    {
        // inputs.MovementInput = virtualMoveDirection;
    }

    public void VirtualLookInput(Vector2 virtualLookDirection)
    {
        // inputs.look = virtualLookDirection;
    }

    public void VirtualJumpInput(bool virtualJumpState)
    {
        // inputs.jump = virtualJumpState;
    }

    public void VirtualSprintInput(bool virtualSprintState)
    {
        // inputs.sprint = virtualSprintState;
    }

    public void VirtualSwitchInput(bool virtualSwitchState)
    {
        // inputs.switchMode = virtualSwitchState;
    }
}