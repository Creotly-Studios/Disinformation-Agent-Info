using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    private float _turnSmoothVel;

    private float currentMoveSpeed;
    private float currentSprintTime;
    private float lastSprintTime = -Mathf.Infinity;
    public float SprintTimeNormalized {get; private set;}

    // Footstep sound variables
    private float footstepTimer;

    public PlayerMoveState(Player_v2 player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
        currentSprintTime = playerData.sprintDuration; // Initialize sprint duration
    }

    public override void Enter()
    {
        base.Enter();
        currentMoveSpeed = playerData.speed;
        footstepTimer = 0f; // Reset the footstep timer when entering the move state
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        input = player.InputHandler.MovementInput; // Get player input
        Move();
        
        if (!isExitingState)
        {
            if (input.magnitude < 0.1f)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }

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

            // Update footstep timer
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstepSounds();
                footstepTimer = playerData.footstepInterval; // Reset the timer
            }
        }
    }

    private void PlayFootstepSounds()
    {
        // Alternate between footstep sounds for variety
        int index = Random.Range(0, playerData.footsteps.Length);
        AudioManager.Instance.PlaySFX(playerData.footsteps[index]);
    }
}