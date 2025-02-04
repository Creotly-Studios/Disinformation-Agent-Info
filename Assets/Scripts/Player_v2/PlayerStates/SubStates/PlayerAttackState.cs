using System;
using System.Collections;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private float _lastClickedTime;
    private float _lastComboEnd;
    private int _comboCounter;
    
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
        Attack();
        // isAbilityDone = true;
    }

    public void Attack()
    {
        if (Time.time - _lastComboEnd > playerData.timeBetweenCombos && _comboCounter <= playerData.combo.Count)
        {
            player.CancelInvoke("EndCombo");
            if (Time.time - _lastClickedTime >= playerData.timeBetweenAttackUsage)
            {
                player.MoveState.FreezeInput();
                playerData.combo[_comboCounter].PerformAttackAction(player.Anim);

                CheckAndDamage(playerData.combo[_comboCounter].damage);
                
                _comboCounter++;
                _lastClickedTime = Time.time;

                if (_comboCounter >= playerData.combo.Count)
                {
                    _comboCounter = 0;
                }
            }
        }
        // player.MoveState.FreezeInput();
        
        if (player.InputHandler.attackPressed == false)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
    
    private void ExitAttack()
    {
        if (player.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f && player.Anim.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
        {
            player.Invoke("EndCombo", 1);
        }
    }

    void EndCombo()
    {
        _comboCounter = 0;
        _lastComboEnd = Time.time;
    }
    
    void CheckAndDamage(int damage)
    {
        RaycastHit[] hits = Physics.SphereCastAll(player.checkTransform.position, playerData.attackSphereSize, player.checkTransform.forward, playerData.attackRange);
        foreach (RaycastHit hit in hits)
        {
            // Check if the object hit has an enemy tag or component
            IDamagable damagable = hit.collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                // Check if the enemy is in front of the player
                Vector3 directionToEnemy = (hit.collider.transform.position - player.checkTransform.position).normalized;
                float dotProduct = Vector3.Dot(player.checkTransform.forward, directionToEnemy);

                if (dotProduct > 0.5f) // Adjust threshold to control front-facing precision
                {
                    Debug.Log($"Hit {hit.collider.name} in front!");
                    damagable.TakeDamage(damage, AnimatorHashing.damageAnimation);
                }
            }
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
        ExitAttack();
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
        // isAbilityDone = true;
    }


}
