using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected Vector2 input;

    public PlayerGroundedState(Player_v2 player, PlayerStateMachine stateMachine,
        PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        input = player.InputHandler.MovementInput;
        if (!player.isGrounded)
        {
            player.StateMachine.ChangeState(player.InAirState);
            return;
        }

        if (player.InputHandler.InteractInput && !player.isAttacking)
        {
            player.Combat.TryInteract();
            return;
        }
        player.InvokeInteractableFoundEvent();
    }
}
