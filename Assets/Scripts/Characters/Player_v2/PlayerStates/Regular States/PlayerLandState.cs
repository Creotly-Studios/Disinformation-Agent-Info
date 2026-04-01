using UnityEngine;

public class PlayerLandState : PlayerGroundedState
{
    public PlayerLandState(Player_v2 player, PlayerStateMachine stateMachine,
        PlayerData playerData, string animBoolName = null)
        : base(player, stateMachine, playerData, animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        AudioManager.Instance.PlaySFX(playerData.land);
        player.Animation.SetVerticalVelocityBlend(1, Time.deltaTime);
    }

    public override void LogicUpdate()
    {
        if (isExitingState) return;

        if (player.InputHandler.MovementInput.magnitude > 0.01f)
            stateMachine.ChangeState(player.MoveState);
        else if (isAnimationFinished)
            stateMachine.ChangeState(player.IdleState);
    }
}
