using UnityEngine;

public class PlayerLandState : PlayerGroundedState
{
    public PlayerLandState(Player_v2 player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.Instance.PlaySFX(playerData.attack[0]);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!isExitingState)
        {
            if (player.InputHandler.MovementInput.magnitude != 0)
            {
                stateMachine.ChangeState(player.MoveState);
            } else if(isAnimationFinished)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    
}
