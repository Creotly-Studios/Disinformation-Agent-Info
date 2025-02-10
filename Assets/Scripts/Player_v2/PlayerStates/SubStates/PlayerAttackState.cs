using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerAttackState : PlayerAbilityState
{
    private bool isAttacking;
    private int attackCounter;
    private float attackCoolDown = 0.25f;

    private Robot lockedTarget;
    private Coroutine attackCoroutine;

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
        isAbilityDone = true;
    }

    private void Attack()
    {
        HandleAttack();
        
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

    private void HandleAttack()
    {
        if(isAttacking)
        {
            return;
        }

        if(player.playerCombat.currentTarget == null)
        {
            lockedTarget = EnemyCombatControllerScript.Instance?.RandomRobot(null);
        }

        float magnitude = player.playerCombat.inputDirectionMagnitude;
        if (magnitude > 0.2f)
        {
            lockedTarget = player.playerCombat.currentTarget;
        }

        if(lockedTarget == null)
        {
            lockedTarget = EnemyCombatControllerScript.Instance?.RandomRobot(null);
        }

        if(lockedTarget == null)
        {
            return;
        }
        float distance = Vector3.Distance(player.transform.position, lockedTarget.transform.position);
        PickAttack(lockedTarget, distance);
    }

    private void PickAttack(Robot target, float distance)
    {
        if(target == null)
        {
            //Ground Punch
            return;
        }

        if(distance < 15)
        {
            int attackCount = playerData.attackArray.Count;
            attackCounter = (int)Mathf.Repeat((float)attackCounter + 1, attackCount);

            int random = Random.Range(0, attackCount);
            PunchSO currentAttack = /*(IsLastHit()) ? playerData.attackArray[random] :*/ playerData.attackArray[attackCounter];
            HandleAttackAction(currentAttack, target, 0.65f);
            return;
        }
        //Ground Punch
    }

    private void MoveTowardsTarget(Robot target, float duration)
    {
        Transform playerTransform = player.transform;

        player.playerCombat.OnTrajectory.Invoke(target);
        playerTransform.DOLookAt(target.transform.position, 0.2f);

        Vector3 targetPos = TargetOffset(playerTransform, target.transform);

        // Move using CharacterController.Move()
        player.StartCoroutine(MoveCharacter(targetPos, duration));
    }

    private IEnumerator MoveCharacter(Vector3 targetPos, float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = player.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, elapsed / duration);

            // Apply movement using CharacterController
            player.controller.Move(newPos - player.transform.position);

            yield return null;
        }
    }

    private Vector3 TargetOffset(Transform player, Transform target)
    {
        Vector3 direction = (player.position - target.position).normalized;
        return target.position + (direction * 0.95f);
    }

    private void HandleAttackAction(PunchSO currentAttack, Robot target, float movementDuration)
    { 
        currentAttack.PerformAttackAction(player.Anim);
        CheckAndDamage(currentAttack.damage);

        if (attackCoroutine != null)
        {
            player.StopCoroutine(attackCoroutine);
        }
        attackCoroutine = player.StartCoroutine(AttackRoutine(attackCoolDown));

        if(target == null)
        {
            return;
        }

        target.dontMove = false;
        MoveTowardsTarget(target, movementDuration);


        IEnumerator AttackRoutine(float duration)
        {
            isAttacking = true;
            player.MoveState.FreezeInput();
            yield return new WaitForSeconds(duration);

            isAttacking = false;
            yield return new WaitForSeconds(.2f);
        }
        //FinalBlowRoutine();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
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
