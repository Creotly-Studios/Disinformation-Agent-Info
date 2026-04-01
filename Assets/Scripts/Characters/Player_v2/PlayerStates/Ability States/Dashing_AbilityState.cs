using UnityEngine;

// Handles the dash ability layer.
// OnEnter delegates the actual physics to PlayerLocomotionManager.ExecuteDash —
// this SO owns state (timer, direction) and transition logic only.
// The locomotion SM does not change state during a dash; it remains in
// Idle or Move (the dash is brief enough that no animation state switch is needed).
[CreateAssetMenu(fileName = "Dashing_AbilityState", menuName = "Player/AbilityState/Dashing")]
public class Dashing_AbilityState : AbilityState
{
    private float   dashTimer;
    private Vector3 dashDirection;

    [SerializeField] private float dashDuration = 0.18f;

    // ── Enter / Exit ──────────────────────────────────────────────────────────

    public override void OnEnter(Player_v2 player)
    {
        dashTimer     = 0f;
        dashDirection = GetDashDirection(player);

        if (dashDirection.sqrMagnitude > 0.001f)
            player.transform.rotation = Quaternion.LookRotation(dashDirection);

        // Delegates stamina deduction, sound, and VFX to the locomotion manager.
        player.Locomotion.ExecuteDash();
    }

    protected override void OnExit(Player_v2 player) { }

    // ── Input Routing ─────────────────────────────────────────────────────────

    protected override void InputUpdate(Player_v2 player)
    {
        // Allow a follow-up dash after the first half of the current one.
        if (dashTimer > dashDuration * 0.5f
            && player.InputHandler.DashInput
            && player.Locomotion.CanDash())
        {
            player.InputHandler.UseDashInput();
            player.SwitchAbilityState(player.Dashing);
        }
    }

    // ── AbilityStateUpdater ───────────────────────────────────────────────────

    public override void AbilityStateUpdater(Player_v2 player)
    {
        base.AbilityStateUpdater(player);
        dashTimer += Time.deltaTime;
        if (dashTimer >= dashDuration)
            player.SwitchAbilityState(player.Normal);
    }

    // ── HandleMovement — called by PlayerLocomotionManager ────────────────────

    public override void HandleMovement(float delta, Player_v2 player)
    {
        // Decelerate in the back half of the dash for a smooth stop.
        float speed = player.PlayerData.dashForce;
        if (dashTimer > dashDuration * 0.7f) speed *= 0.4f;

        player.Controller.Move(dashDirection * (speed * delta));
        player.Animation.SetMovementBlend(player.InputHandler.MovementInput.magnitude, delta);
    }

    // ── HandleRotation — called by PlayerLocomotionManager ───────────────────

    public override void HandleRotation(float delta, Player_v2 player)
    {
        // Lock rotation to the dash direction for the full duration.
        if (dashDirection.sqrMagnitude > 0.001f)
            player.transform.rotation = Quaternion.LookRotation(dashDirection);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private Vector3 GetDashDirection(Player_v2 player)
    {
        Vector2 rawInput = player.InputHandler.MovementInput;
        if (rawInput.magnitude > 0.01f)
            return player.Locomotion.GetCameraRelativeDirection(rawInput).normalized;
        return player.transform.forward;
    }
}
