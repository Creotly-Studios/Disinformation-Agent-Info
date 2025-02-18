using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerAttackState : PlayerAbilityState
{
    private bool isAttacking;
    private int attackCounter;
    private Coroutine attackCoroutine;
    private float attackCoolDown = 0.25f;

    public PlayerAttackState(Player_v2 player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    private void Attack()
    {
        player.playerCombat.HandleAttack(player);
    }
    
    private void ExitAttack()
    {
        if (player.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f && player.Anim.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
        {
            player.Invoke("EndCombo", 1);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        Attack();
        if (player.InputHandler.attackPressed == false)
        {
            isAbilityDone = true;
        }
        player.Move(Vector3.zero);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    public IEnumerator AbilityDone()
    {
        yield return new WaitForSeconds(playerData.timeBetweenAttacks);
        isAbilityDone = true;
    }
}
