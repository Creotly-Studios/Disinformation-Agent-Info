public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Player_v2 player, PlayerStateMachine stateMachine,
        PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName) { }

    public override void Enter()
    {
        base.Enter();
        AudioManager.Instance.PlaySFX(playerData.dead_GameOver);
    }
}
