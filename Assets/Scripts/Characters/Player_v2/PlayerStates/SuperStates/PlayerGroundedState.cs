using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected Vector2 input;
    protected bool jumpInput;
    private bool dashInput;
    private bool interactInput;
    private bool attackInput;

    public bool isAttacking { get; private set; }

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
        player.InputHandler.InputSystemActions.Player.Interact.started += _ => { 
            Debug.Log("Interact Presed");
        };
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
        attackInput = player.InputHandler.AttackInput;
        interactInput = player.InputHandler.InteractInput;

        if (jumpInput)
        {
            player.InputHandler.UseJumpInput();
            stateMachine.ChangeState(player.JumpState);
        }

        if (dashInput)
        {
            if (player.PlayerStatistics.CanDash())
            {
                player.InputHandler.UseDashInput();
                player.PlayerStatistics.UseDash();
                stateMachine.ChangeState(player.DashState);
            }
            else
            {
                Debug.Log("Not enough stamina to dash!");
            }
        }

        if (interactInput)
        {
            player.InputHandler.UseInteractInput();
            stateMachine.ChangeState(player.InteractState);
        }

        if (attackInput)
        {
            player.InputHandler.UseAttackInput();
            stateMachine.ChangeState(player.AttackState); // Transition to AttackState
        }

        player.InvokeIInteractableFoundEvent();
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