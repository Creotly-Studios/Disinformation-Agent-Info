using UnityEngine;

[CreateAssetMenu(fileName = "Normal_AbilityState", menuName = "Player/AbilityState/Normal")]
public class Normal_AbilityState : AbilityState
{
    public override void OnEnter(Player_v2 player)
    {
        base.OnEnter(player);
    }

    protected override void InputUpdate(Player_v2 player)
    {
        PlayerInputHandler input = player.InputHandler;
        if (input.DashInput && player.Locomotion.CanDash())
        {
            player.InputHandler.UseDashInput();
            player.SwitchAbilityState(player.Dashing);
            return;
        }

        if (input.JumpInput)
        {
            player.SwitchAbilityState(player.Jumping);
            return;
        }

        if (input.AttackInput)
        {
            player.InputHandler.UseAttackInput();
            player.SwitchAbilityState(player.CombatState);
        }
    }

    public override void HandleMovement(float delta, Player_v2 player)
    {
        if (player.Combat.IsInteractActive) return;

        Vector2 rawInput = player.InputHandler.MovementInput;
        float moveMagnitude = rawInput.magnitude;
        Vector3 moveDirection = player.Locomotion.GetCameraRelativeDirection(rawInput);

        if (moveMagnitude > 0.01f)
        {
            Vector3 targetVelocity = moveDirection * (moveMagnitude * player.Locomotion.CurrentMoveSpeed);
            float snapFactor = 1f - Mathf.Exp(-15f * delta);
            currentVelocity  = Vector3.Lerp(currentVelocity, targetVelocity, snapFactor);
        }
        else
        {
            float stopFactor = 1f - Mathf.Exp(-20f * delta);
            currentVelocity  = Vector3.Lerp(currentVelocity, Vector3.zero, stopFactor);
        }

        currentVelocity.y = 0f;
        player.Controller.Move(currentVelocity * delta);
        if (moveMagnitude > 0.01f)
        {
            player.Locomotion.TickFootsteps(delta);
        }
        player.Animation.SetMovementBlend(moveMagnitude, delta);
    }

    public override void HandleRotation(float delta, Player_v2 player)
    {
        Vector2 rawInput = player.InputHandler.MovementInput;
        Vector3 rotationDirection = player.Locomotion.GetCameraRelativeDirection(rawInput);

        if (rotationDirection.sqrMagnitude < 0.01f)
        {
            return;
        }
        float smoothFactor   = 1f - Mathf.Exp(-player.PlayerData.turnSmoothTime * delta * 10f);
        Quaternion targetRotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, smoothFactor);
    }
}
