using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected Vector2 input;
    private bool jumpInput;
    private bool dashInput;
    private bool interactInput;
    private bool attackInput;

    public PlayerGroundedState(Player_v2 player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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
        input = player.InputHandler.MovementInput;
        jumpInput = player.InputHandler.JumpInput;
        dashInput = player.InputHandler.DashInput;
        attackInput = player.InputHandler.attackPressed;
        interactInput = player.InputHandler.InteractInput;

        if (jumpInput == true)
        {
            player.InputHandler.UseJumpInput();
            stateMachine.ChangeState(player.JumpState);
        }

        if (dashInput == true)
        {
            player.InputHandler.UseDashInput();
            stateMachine.ChangeState(player.DashState);
        }

        if (interactInput == true)
        {
            player.InputHandler.UseeInteractInput();
            stateMachine.ChangeState(player.InteractState);
        }

        if (attackInput == true)
        {
            player.CombatSystem.Attack();
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void FreezeInput()
    {
        input = Vector2.zero;
    }



}
