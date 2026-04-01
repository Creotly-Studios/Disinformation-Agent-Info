public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player_v2 player, PlayerStateMachine stateMachine,PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName)
    {

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!isExitingState && input.magnitude >= 0.1f)
        {
            stateMachine.ChangeState(player.MoveState);
        }
    }
}
