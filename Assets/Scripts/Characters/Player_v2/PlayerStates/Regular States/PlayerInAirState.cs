using UnityEngine;

public class PlayerInAirState : PlayerState
{
    public PlayerInAirState(Player_v2 player, PlayerStateMachine stateMachine,
        PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (player.isGrounded && player.Controller.velocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.LandState);
        }
    }
}
