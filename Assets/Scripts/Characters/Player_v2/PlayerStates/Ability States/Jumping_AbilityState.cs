using UnityEngine;

[CreateAssetMenu(fileName = "Jumping_AbilityState", menuName = "Player/AbilityState/Jumping")]
public class Jumping_AbilityState : AbilityState
{
    private float airTime;

    private bool leftGround;
    private const float MinAirTime = 0.08f;

    public override void OnEnter(Player_v2 player)
    {
        airTime = 0f;
        leftGround = false;
        player.isGrounded = false;
        if (player.StateMachine.CurrentState != player.InAirState)
        {
            player.StateMachine.ChangeState(player.InAirState);
            player.Anim.SetFloat(AnimatorHashing.Y_VEL_HASH, 0);
            player.Locomotion.ExecuteJump();
        }
        player.InputHandler.UseJumpInput();
    }

    protected override void OnExit(Player_v2 player) { }

    // ── Input Routing ─────────────────────────────────────────────────────────

    protected override void InputUpdate(Player_v2 player)
    {
        if (player.InputHandler.JumpInput)
        {
            player.InputHandler.UseJumpInput();
            return;
        }

        if (player.InputHandler.AttackInput)
        {
            player.InputHandler.UseAttackInput();
            player.SwitchAbilityState(player.CombatState);
        }
    }

    // ── AbilityStateUpdater ───────────────────────────────────────────────────

    public override void AbilityStateUpdater(Player_v2 player)
    {
        base.AbilityStateUpdater(player);
        if (!leftGround)
        {
            if (!player.isGrounded)
            {
                airTime = 0f;
                leftGround = true;
            }
            return;
        }

        float delta = Time.deltaTime;
        if (!player.isGrounded)
        {
            airTime += delta;
        }
        if (player.isGrounded && airTime >= MinAirTime)
        {
            player.SwitchAbilityState(player.Normal);
            player.Animation.SetVerticalVelocityBlend(1, delta);
        }
    }

    public override void HandleMovement(float delta, Player_v2 player)
    {
        Vector2 rawInput = player.InputHandler.MovementInput;
        Vector3 moveDirection = player.Locomotion.GetCameraRelativeDirection(rawInput);
        player.Controller.Move(moveDirection * (player.PlayerData.speed * delta));
        player.Animation.SetMovementBlend(rawInput.magnitude, delta);
    }

    public override void HandleRotation(float delta, Player_v2 player)
    {
        Vector3 rotationDirection = player.Locomotion.GetCameraRelativeDirection(player.InputHandler.MovementInput);

        if (rotationDirection.sqrMagnitude < 0.01f)
        {
            return;
        }
        Quaternion targetRotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
        float smoothFactor = 1f - Mathf.Exp(-player.PlayerData.turnSmoothTime * delta * 10f);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, smoothFactor);
    }
}