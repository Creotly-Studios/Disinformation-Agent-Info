using UnityEngine;

public class PlayerInactiveState : PlayerState
{
    public PlayerInactiveState(Player_v2 player, PlayerStateMachine stateMachine,
        PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        AudioManager.Instance.PlaySFX(playerData.inactiveStateCamSound);
        if (player.dialogue_InactiveCamera != null)
        {
            player.dialogue_InactiveCamera.SetActive(true);
        }
    }

    public override void Exit()
    {
        if (player.dialogue_InactiveCamera != null)
        {
            player.dialogue_InactiveCamera.SetActive(false);
        }
        player.Combat.SetInteractiveFlag(false);
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // Consume all inputs so no actions queue while inactive.
        player.Controller.Move(Vector3.zero);
        player.InputHandler.UseAttackInput();
        player.InputHandler.UseJumpInput();
        player.InputHandler.UseDashInput();
        player.InputHandler.UseInteractInput();
    }
}
