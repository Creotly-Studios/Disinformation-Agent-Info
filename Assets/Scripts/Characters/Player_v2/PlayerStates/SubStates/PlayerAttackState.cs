using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    private int comboCounter;
    private float lastClickedTime;
    private float lastComboEnd;

    public PlayerAttackState(Player_v2 player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isAbilityDone = false;
        AudioManager.Instance.PlaySFX(playerData.attack[0]);

        // Start the attack combo
        if (Time.time - lastComboEnd > playerData.timeBetweenCombos && comboCounter < playerData.attackArray.Count)
        {
            if (Time.time - lastClickedTime >= playerData.timeBetweenAttackUsage)
            {
                PerformAttack();
            }
        }
        else
        {
            // If the combo is not valid, transition back to the grounded state
            isAbilityDone = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Check if the attack animation is complete
        AnimatorStateInfo stateInfo = player.Anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 0.9f && stateInfo.IsTag("attack"))
        {
            isAbilityDone = true; // Mark the ability as done
        }

        // If the attack is done, transition back to the grounded state
        if (isAbilityDone)
        {
            if (player.controller.isGrounded)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                stateMachine.ChangeState(player.InAirState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        // Reset combo if the player exits the attack state
        if (isAbilityDone)
        {
            // comboCounter = 0;
            lastComboEnd = Time.time;
        }
    }

    private void PerformAttack()
    {
        // Freeze input during the attack
        player.MoveState.FreezeInput();

        // Perform the attack action
        // playerData.attackArray[comboCounter].PerformAttackAction(player.Anim);
        player.Anim.runtimeAnimatorController = playerData.attackArray[comboCounter].animation;
        DealDamage(playerData.attackArray[comboCounter].damage);
        // Increment combo counter
        comboCounter++;
        lastClickedTime = Time.time;

        // Reset combo if it exceeds the number of attacks
        if (comboCounter >= playerData.attackArray.Count)
        {
            comboCounter = 0;
        }
    }

    private void DealDamage(int damage)
    {
        // Check for enemies in front of the player and deal damage
        RaycastHit[] hits = Physics.SphereCastAll(
            player.checkTransform.position, 
            playerData.attackSphereSize, 
            player.checkTransform.forward, 
            playerData.attackRange
        );

        foreach (RaycastHit hit in hits)
        {
            IDamagable damagable = hit.collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                // Check if the enemy is in front of the player
                Vector3 directionToEnemy = (hit.collider.transform.position - player.checkTransform.position).normalized;
                float dotProduct = Vector3.Dot(player.checkTransform.forward, directionToEnemy);

                if (dotProduct > 0.2f) // Adjust threshold to control front-facing precision
                {
                    Debug.Log($"Hit {hit.collider.name} in front!");
                    AudioManager.Instance.PlaySFX(playerData.attackHit);
                    damagable.TakeDamage(damage);
                }
            }
        }
    }
}