using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamagable
{
    public Transform Player { get; protected set; }
    public LayerMask whatIsGround, whatIsPlayer;
    public int currentHealth;

    [Header("Enemy Settings")]
    public EnemyData e_data;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;

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
    }


    public void TakeDamage(int healthDamage)
    {
        currentHealth -= healthDamage;
        if(currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        KillTracker.Instance?.EnemyDied();
        Destroy(gameObject);
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
}
