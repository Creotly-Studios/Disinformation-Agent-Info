using UnityEngine;

public class PlayerInactiveState : PlayerState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerInactiveState(Player_v2 player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        player.InputHandler.UseAttackInput();
        player.InputHandler.UseJumpInput();
        player.InputHandler.UseDashInput();
        player.InputHandler.UseeInteractInput();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
