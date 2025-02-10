using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    private float _turnSmoothVel;
    private bool sprint;

    private float currentMoveSpeed;

    public PlayerMoveState(Player_v2 player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        currentMoveSpeed = playerData.speed;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Move();
        sprint = player.InputHandler.SprintInput;
        if (input.magnitude < 0.1f)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        if (sprint)
        {
            currentMoveSpeed = playerData.sprintSpeed;
            player.Anim.SetFloat("moveVel", 1);
        } else {
            currentMoveSpeed = playerData.speed;
            player.Anim.SetFloat("moveVel", 0);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void Move()
    {
        // Debug.Log($"Input: {input}, Camera.main: {Camera.main}, Player: {player}, Data: {playerData.speed}");

        Vector3 dir = new Vector3(input.x, 0, input.y);

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float smoothedAngle = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetAngle, ref _turnSmoothVel, playerData.turnSmoothTime);
            player.transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            player.Move(moveDir * currentMoveSpeed * Time.deltaTime);
        }
    }

 
}
