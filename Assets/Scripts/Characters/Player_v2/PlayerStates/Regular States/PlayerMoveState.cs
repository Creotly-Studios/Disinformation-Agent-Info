using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player_v2 player, PlayerStateMachine stateMachine,
        PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate(); // Fall detection + interact.
        if (isExitingState) return;

        if (input.magnitude < 0.01f)
            stateMachine.ChangeState(player.IdleState);
    }
}
