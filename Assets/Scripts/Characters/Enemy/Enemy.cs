using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))] // Add Rigidbody for knockback
public abstract class Enemy : MonoBehaviour, IDamagable
{
    public Transform Player { get; protected set; }
    public LayerMask whatIsGround, whatIsPlayer;
    public int currentHealth;

    [Header("Enemy Settings")]
    public EnemyData e_data;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody rb; // Rigidbody for knockback

    protected bool isDead;
    protected bool isAttacking;
    public bool isKnockedBack; // Track if the enemy is currently being knocked back

    [Header("Checks")]
    public Transform attackPoint;

    protected void Awake()
    {
        e_data = Instantiate(e_data);
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        rb.isKinematic = true; // Ensure Rigidbody doesn't interfere with NavMeshAgent by default
    }

    protected void Start()
    {
        Player = Player_v2.Instance.gameObject.transform;
        currentHealth = e_data.maxhealth;
        isDead = false;
        isAttacking = false;
        isKnockedBack = false;
    }

    // New abstract method for attack implementation
    protected abstract void PerformAttack();

    // New method to handle attack attempts
    protected bool TryAttack()
    {
        if (isDead || isAttacking || isKnockedBack) return false; // Prevent attacking while knocked back

        if (PlayerInAttackRange() && EnemyAttackManager.Instance.RequestAttackPermission(this))
        {
            isAttacking = true;
            PlayAttackAnim();
            PerformAttack();
            return true;
        }
        return false;
    }

    // Call this when attack animation/action is complete
    protected void FinishAttack()
    {
        isAttacking = false;
        EnemyAttackManager.Instance.FinishAttack(this);
    }

    public void TakeDamage(int healthDamage)
    {
        if (isDead) return;

        currentHealth -= healthDamage;
        Vector3 knockbackDirection = (transform.position - Player.transform.position).normalized;
        ApplyKnockback(knockbackDirection, e_data.knockbackForce); // Apply knockback when taking damage

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void ApplyKnockback(Vector3 direction, float force)
    {
        if (isKnockedBack) return; // Prevent multiple knockbacks at once

        isKnockedBack = true;
        agent.enabled = false; // Disable NavMeshAgent to allow Rigidbody movement
        rb.isKinematic = false; // Enable Rigidbody physics
        rb.AddForce(direction.normalized * force, ForceMode.Impulse); // Apply knockback force

        Invoke(nameof(ResetAfterKnockback), e_data.knockbackDuration); // Reset after knockback duration
    }

    private void ResetAfterKnockback()
    {
        isKnockedBack = false;
        rb.isKinematic = true; // Disable Rigidbody physics
        rb.linearVelocity = Vector3.zero; // Reset velocity
        agent.enabled = true; // Re-enable NavMeshAgent
    }

    private void HandleDeath()
    {
        if (!isDead)
        {
            PlayDeadAnim();
            isDead = true;
            if (isAttacking)
            {
                FinishAttack(); // Release attack lock if dead while attacking
            }

            QuestObjectives objective = QuestManager.Instance.FindQuestObjective(ObjectiveType.FightBots);
            if (objective != null && objective.isDone != true)
            {
                QuestSO quest = QuestManager.Instance.activeQuest;
                quest.IncreaseQuestObjectiveProgressLevels(objective);
            }
            Destroy(gameObject, e_data.destroyTime);
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public bool PlayerInSightRange()
    {
        return Physics.CheckSphere(transform.position, e_data.detectRange, whatIsPlayer);
    }

    public bool PlayerInAttackRange()
    {
        return Physics.CheckSphere(transform.position, e_data.attackRange, whatIsPlayer);
    }

    protected void OnDrawGizmosSelected()
    {
        if (e_data == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, e_data.detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, e_data.attackRange);
    }

    public void PlayIdleAnim()
    {
        if (animator)
        {
            animator.SetBool("idle", true);
            animator.SetBool("isWalking", false);
        }
    }

    public void PlayAttackAnim()
    {
        if (animator)
        {
            animator.SetBool("isWalking", false);
            animator.SetTrigger("attack");
        }
    }

    public void PlayDeadAnim()
    {
        animator.SetBool("dead", true);
    }
}