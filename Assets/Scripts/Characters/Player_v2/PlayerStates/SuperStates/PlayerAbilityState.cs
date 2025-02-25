using UnityEngine;

public class PlayerAbilityState : PlayerState
{
    protected bool isAbilityDone;

    public PlayerAbilityState(Player_v2 player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        isAbilityDone = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAbilityDone)
        {
            if (player.controller.isGrounded && player.controller.velocity.magnitude < 0.01f)
            {
                stateMachine.ChangeState(player.IdleState); // Transition to IdleState
            }
            else
            {
                stateMachine.ChangeState(player.InAirState); // Transition to InAirState
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }



}
