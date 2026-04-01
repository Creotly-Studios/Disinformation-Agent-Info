using UnityEngine;

[CreateAssetMenu(fileName = "Combat_AbilityState", menuName = "Player/AbilityState/Combat")]
public class Combat_AbilityState : AbilityState
{
    [Header("Parameters")]
    [SerializeField] private float rotationSnapSpeed  = 20f;
    [SerializeField] private float maxCombatDuration  = 1.2f;

    private float combatDuration;

    public override void OnEnter(Player_v2 player)
    {
        combatDuration = 0f;
        player.Combat.TryAttack();
        player.PlayAttackEffect();
    }

    protected override void OnExit(Player_v2 player) 
    { 
        combatDuration = 0f;
    }

    protected override void InputUpdate(Player_v2 player)
    {
        if (player.InputHandler.DashInput && player.Locomotion.CanDash())
        {
            player.InputHandler.UseDashInput();
            player.SwitchAbilityState(player.Dashing);
            return;
        }

        if (player.InputHandler.AttackInput)
        {
            player.InputHandler.UseAttackInput();
            player.Combat.TryAttack();
        }
    }

    public override void AbilityStateUpdater(Player_v2 player)
    {
        base.AbilityStateUpdater(player);

        if (!player.isAttacking)
        {
            combatDuration += Time.deltaTime;
            if (combatDuration >= maxCombatDuration)
            {
                player.SwitchAbilityState(player.Normal);
            }
        }
    }

    public override void HandleMovement(float delta, Player_v2 player)
    {
        Vector3 moveDirection = player.Locomotion.GetCameraRelativeDirection(player.InputHandler.MovementInput);

        float speed = player.PlayerData.speed;
        player.Controller.Move(moveDirection * (speed * delta));
    }

    public override void HandleRotation(float delta, Player_v2 player)
    {
        Vector3 rotationDirection = player.Locomotion.GetCameraRelativeDirection(player.InputHandler.MovementInput);

        if (rotationDirection.sqrMagnitude < 0.01f) return;

        float      smoothFactor   = 1f - Mathf.Exp(-rotationSnapSpeed * delta);
        Quaternion targetRotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, smoothFactor);
    }
}
