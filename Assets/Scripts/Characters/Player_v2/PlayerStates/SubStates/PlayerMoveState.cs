using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    private float _turnSmoothVel;

    private float currentMoveSpeed;
    private float currentSprintTime;
    private float lastSprintTime = -Mathf.Infinity;
    public float SprintTimeNormalized {get; private set;}

    public PlayerMoveState(Player_v2 player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
        currentSprintTime = playerData.sprintDuration; // Initialize sprint duration
    }

    public override void Enter()
    {
        base.Enter();
        currentMoveSpeed = playerData.speed;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        input = player.InputHandler.MovementInput; // Get player input

        Move();

        // Transition to idle if no input
        if (input.magnitude < 0.1f && !isExitingState)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    // private void HandleSprint()
    // {
    //     bool sprint = player.InputHandler.SprintInput; // Check if sprint button is held

    //     if (sprint && currentSprintTime > 0f && Time.time - lastSprintTime > playerData.sprintCooldown)
    //     {
    //         currentMoveSpeed = playerData.sprintSpeed;
    //         currentSprintTime -= Time.deltaTime;
    //         player.Anim.SetFloat("moveVel", 1f); 
    //     }
    //     else
    //     {
    //         currentMoveSpeed = playerData.speed;
    //         player.Anim.SetFloat("moveVel", 0f);

    //         // Recover sprint stamina when not sprinting
    //         if (currentSprintTime < playerData.sprintDuration)
    //         {
    //             currentSprintTime += Time.deltaTime * playerData.sprintRechargeRate;
    //         }
    //     }

    //     // Store last sprint time if player runs out of stamina
    //     if (currentSprintTime <= 0f)
    //     {
    //         lastSprintTime = Time.time;
    //     }
    // }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void Move()
    {
        Vector3 dir = new Vector3(input.x, 0, input.y);

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float smoothedAngle = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetAngle, ref _turnSmoothVel, playerData.turnSmoothTime);
            player.transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            player.Move(moveDir * player.PlayerStatistics.CurrentMoveSpeed * Time.deltaTime);
        }
    }
}
