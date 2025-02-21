using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour, IDamagable
{
    public Transform Player { get; protected set; }
    public LayerMask whatIsGround, whatIsPlayer;
    public int currentHealth;

    [Header("Enemy Settings")]
    public EnemyData e_data;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;

    bool isDead;

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
            KillTracker.Instance?.EnemyDied();
            Destroy(gameObject, e_data.destroyTime);
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool PlayerInSightRange()
    {
        return Physics.CheckSphere(transform.position, e_data.detectRange, whatIsPlayer);
    }

    public bool PlayerInAttackRange()
    {
        return Physics.CheckSphere(transform.position, e_data.attackRange, whatIsPlayer);
    }

    // DRAW GIZMOS FOR DETECT AND ATTACK RANGE
    protected void OnDrawGizmosSelected()
    {
        if (e_data == null) return;

        // Draw detection range in yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, e_data.detectRange);

        // Draw attack range in red
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, e_data.attackRange);
    }

    //handling animations
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
