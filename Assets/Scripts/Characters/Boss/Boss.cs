using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Boss : MonoBehaviour, IDamagable
{
    public Transform Player { get; protected set; }
    public LayerMask whatIsGround, whatIsPlayer;
    [SerializeField] public int maxhealth = 10;

    [SerializeField] private float detectRange;
    [SerializeField] private float attackRange;

    [Header("Boss Settings")]
    [SerializeField] public BossStage Stage;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;

    private int currentHealth;
    bool isDead;

    [Header("Checks")]
    [SerializeField] Transform melleAttackPoint;
    [SerializeField] Transform shootPoint;
    [SerializeField] int stage_Two_Treshold = 6;

    [Header("---For Stage1_2---")]
    [SerializeField] GameObject[] enemiesToSpawn;
    [SerializeField] int totalEnemiesToSpawnCount = 10;
    [SerializeField] float spawnEnemyRate;
    [SerializeField] float melleRate;

    [Space]
    [SerializeField] private GameObject bossNPC;

    private float nextSpawnTime;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private Rigidbody rb;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f; // Cooldown between attacks
    private float lastAttackTime;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    protected void Start()
    {
        SetBossStage(BossStage.Stage_one);
        Player = Player_v2.Instance.gameObject.transform;
        currentHealth = maxhealth;
        isDead = false;
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        rb.isKinematic = true;
        lastAttackTime = -attackCooldown; // Initialize last attack time to allow immediate first attack
    }

    void Update()
    {
        UpdateStageState();
    }

    public void TakeDamage(int healthDamage)
    {
        if (isDead) return;

        currentHealth -= healthDamage;
        Vector3 knockbackDirection = (transform.position - Player.transform.position).normalized;
        ApplyKnockback(knockbackDirection);
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
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool PlayerInSightRange()
    {
        return Physics.CheckSphere(transform.position, detectRange, whatIsPlayer);
    }

    public bool PlayerInAttackRange()
    {
        return Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
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

    void UpdateStageState()
    {
        if (currentHealth <= stage_Two_Treshold && Stage == BossStage.Stage_one)
        {
            SetBossStage(BossStage.Stage_two);
        }
        else if (currentHealth <= 0 && Stage != BossStage.Dead)
        {
            SetBossStage(BossStage.Dead);
        }

        switch (Stage)
        {
            case BossStage.Stage_one:
                HandleStageOne();
                break;

            case BossStage.Stage_two:
                HandleStageTwo();
                break;

            case BossStage.Dead:
                HandleDeathStage();
                break;
        }
    }

    void HandleStageOne()
    {
        if (isKnockedBack) return;
        if (PlayerInSightRange())
        {
            Vector3 directionToPlayer = (Player.position - transform.position).normalized;
            Vector3 destination = Player.position - directionToPlayer * chaseOffset;
            agent.SetDestination(destination); 
            animator.SetBool("isWalking", true);

            if (Time.time >= nextSpawnTime)
            {
                SpawnEnemies();
                nextSpawnTime = Time.time + spawnEnemyRate;
            }
        }
        else
        {
            PlayIdleAnim();
        }
    }

    [SerializeField] private float chaseOffset = 3f;
    void HandleStageTwo()
    {
        if (isKnockedBack) return;
        if (PlayerInAttackRange())
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                PerformMeleeAttack();
                lastAttackTime = Time.time; // Update the last attack time
            }
        }
        else if (PlayerInSightRange())
        {
            agent.isStopped = false;
            Vector3 directionToPlayer = (Player.position - transform.position).normalized;
            Vector3 destination = Player.position - directionToPlayer * chaseOffset;
            agent.SetDestination(destination); 
            animator.SetBool("isWalking", true);
        }
        else
        {
            PlayIdleAnim();
        }
    }

    void HandleDeathStage()
    {
        if (AreAllEnemiesDead())
        {
            Instantiate(bossNPC, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    void SpawnEnemies()
    {
        if (totalEnemiesToSpawnCount > 0)
        {
            int randomIndex = Random.Range(0, enemiesToSpawn.Length);
            GameObject enemy = Instantiate(enemiesToSpawn[randomIndex], shootPoint.position, shootPoint.rotation);
            spawnedEnemies.Add(enemy);
            totalEnemiesToSpawnCount--;
        }
    }

    bool AreAllEnemiesDead()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null && !enemy.GetComponent<Enemy>().IsDead())
            {
                return false;
            }
        }
        return true;
    }

    void PerformMeleeAttack()
    {
        PlayAttackAnim(); 
        Collider[] hitPlayers = Physics.OverlapSphere(melleAttackPoint.position, attackRange, whatIsPlayer);
        foreach (var player in hitPlayers)
        {
            if (player.CompareTag("Player"))
            {
                player.GetComponent<IDamagable>().TakeDamage(1);
            }
        }
    }

    public void OnAttackAnimationEvent()
    {
        PerformMeleeAttack();
    }

    void SetBossStage(BossStage _)
    {
        Stage = _;
    }
    
    bool isKnockedBack;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float knockbackForce;
    private void ApplyKnockback(Vector3 direction)
    {
        if (isKnockedBack) return; // Prevent multiple knockbacks at once

        isKnockedBack = true;
        agent.enabled = false; // Disable NavMeshAgent to allow Rigidbody movement
        rb.isKinematic = false; // Enable Rigidbody physics
        rb.AddForce(direction.normalized * knockbackForce, ForceMode.Impulse); // Apply knockback force

        Invoke(nameof(ResetAfterKnockback), knockbackDuration); // Reset after knockback duration
    }

    private void ResetAfterKnockback()
    {
        isKnockedBack = false;
        rb.isKinematic = true; // Disable Rigidbody physics
        rb.linearVelocity = Vector3.zero; // Reset velocity
        agent.enabled = true; // Re-enable NavMeshAgent
    }
}

public enum BossStage { Stage_one, Stage_two, Dead }