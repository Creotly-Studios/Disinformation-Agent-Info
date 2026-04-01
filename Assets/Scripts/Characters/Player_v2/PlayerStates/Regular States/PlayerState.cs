using UnityEngine;
public class PlayerState
{
    protected Player_v2          player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData         playerData;

    protected float startTime;
    protected bool  isAnimationFinished;
    protected bool  isExitingState;

    private readonly int animBoolHash;

    public PlayerState(Player_v2 player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName = null)
    {
        this.player       = player;
        this.stateMachine = stateMachine;
        this.playerData   = playerData;

        animBoolHash = 0;
        if(animBoolName != null)
        {
            animBoolHash = Animator.StringToHash(animBoolName);
        }
    }

    // ── State Lifecycle ───────────────────────────────────────────────────────

    public virtual void Enter()
    {
        DoChecks();
        if(animBoolHash != 0)
        {
            player.Animation.SetBool(animBoolHash, true);
        }
        startTime           = Time.time;
        isAnimationFinished = false;
        isExitingState      = false;
    }

    public virtual void Exit()
    {
        if (animBoolHash != 0)
        {
            player.Animation.SetBool(animBoolHash, false);
        }
        isExitingState = true;
    }

    public virtual void LogicUpdate()   { }
    public virtual void PhysicsUpdate() => DoChecks();
    public virtual void DoChecks()      { }

    // ── Animation Events ──────────────────────────────────────────────────────

    public virtual void AnimationTrigger()       { }
    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;
}
