using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public abstract class Enemy : MonoBehaviour, IDamagable
{
    public Transform Player { get; protected set; }
    public LayerMask whatIsGround, whatIsPlayer;
    public int currentHealth;

    [Header("Enemy Settings")]
    public EnemyData e_data;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;

    protected bool isDead;
    protected bool isAttacking;

    [Header("Checks")]
    public Transform attackPoint;

    protected void Awake()
    {
        e_data = Instantiate(e_data);
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    protected void Start()
    {
        Player = Player_v2.Instance.gameObject.transform;
        currentHealth = e_data.maxhealth;
        isDead = false;
        isAttacking = false;
    }

    // New abstract method for attack implementation
    protected abstract void PerformAttack();

    // New method to handle attack attempts
    protected bool TryAttack()
    {
        if (isDead || isAttacking) return false;

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
        if(isDead) return;
        
        currentHealth -= healthDamage;
        if (currentHealth <= 0)
        {
            HandleDeath();
        }
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
            KillTracker.Instance?.EnemyDied();
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