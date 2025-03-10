using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    private int comboCounter;
    private float lastClickedTime;
    private float lastComboEnd;
    private float inputBufferTime = 0.2f;
    private bool inputBuffered = false;
    private float lastInputBufferTime;

    public PlayerAttackState(Player_v2 player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isAbilityDone = false;
        
        // If we're outside the combo time window, reset the counter
        if (Time.time - lastComboEnd > playerData.timeBetweenCombos)
        {
            comboCounter = 0;
        }

        // Play appropriate attack sound
        int attackIndex = comboCounter % playerData.attackArray.Count;
        if (attackIndex < playerData.attack.Length)
        {
            AudioManager.Instance.PlaySFX(playerData.attack[attackIndex]);
        }
        
        // Execute the attack
        PerformAttack();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Check if attack input was received
        if (player.InputHandler.AttackInput)
        {
            inputBuffered = true;
            lastInputBufferTime = Time.time;
            player.InputHandler.UseAttackInput(); // Clear the input after using it
        }

        // Check if the attack animation is complete
        AnimatorStateInfo stateInfo = player.Anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 0.9f && stateInfo.IsTag("attack"))
        {
            // If input was buffered and still within valid time window, continue the combo
            if (inputBuffered && Time.time - lastInputBufferTime < inputBufferTime)
            {
                // Restart the state to continue the combo
                stateMachine.ChangeState(player.AttackState);
                return;
            }
            else
            {
                // No input for next attack, so mark this combo as done
                isAbilityDone = true;
            }
        }

        // If the attack is done, transition back to the appropriate state
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

        // Record when the combo ended if we're truly exiting (not continuing the combo)
        if (!inputBuffered)
        {
            lastComboEnd = Time.time;
        }
        
        // Clear the input buffer
        inputBuffered = false;
    }

    private void PerformAttack()
    {
        // Freeze input during the attack
        player.MoveState.FreezeInput();

        // Get the attack index, making sure we loop through the attack array
        int attackIndex = comboCounter % playerData.attackArray.Count;
        
        // Set the animation
        player.Anim.runtimeAnimatorController = playerData.attackArray[attackIndex].animation;
        
        // Apply damage
        DealDamage(playerData.attackArray[attackIndex].damage);
        
        // Record when this attack happened
        lastClickedTime = Time.time;
        
        // Move to the next attack in the sequence
        comboCounter++;
    }

    private void DealDamage(int damage)
    {
        // Use OverlapSphere for more efficient hit detection
        Collider[] hitColliders = Physics.OverlapSphere(
            player.checkTransform.position, 
            playerData.attackSphereSize
        );

        bool hitSomething = false;

        foreach (Collider hitCollider in hitColliders)
        {
            IDamagable damagable = hitCollider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                // Check if the enemy is in front of the player
                Vector3 directionToEnemy = (hitCollider.transform.position - player.checkTransform.position).normalized;
                float dotProduct = Vector3.Dot(player.checkTransform.forward, directionToEnemy);

                if (dotProduct > 0.2f) // Front-facing check
                {
                    // Check line of sight to prevent hitting through walls
                    if (!Physics.Linecast(
                        player.checkTransform.position, 
                        hitCollider.bounds.center, 
                        LayerMask.GetMask("Environment")))
                    {
                        Debug.Log($"Hit {hitCollider.name} with damage {damage}!");
                        damagable.TakeDamage(damage);
                        hitSomething = true;
                    }
                }
            }
        }

        // Play hit sound only if something was hit
        if (hitSomething)
        {
            AudioManager.Instance.PlaySFX(playerData.attackHit);
        }
    }
    
    // Debugging visualization
    public void OnDrawGizmosSelected()
    {
        if (player != null && player.checkTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.checkTransform.position, playerData.attackSphereSize);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(player.checkTransform.position, player.checkTransform.forward * playerData.attackRange);
        }
    }
}