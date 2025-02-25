using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
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
    }

    void Update()
    {
        UpdateStageState();
    }

    public void TakeDamage(int healthDamage)
    {
        if (isDead) return;

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
        if (PlayerInSightRange())
        {
            agent.SetDestination(Player.position);
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

    void HandleStageTwo()
    {
        if (PlayerInAttackRange())
        {
            PerformMeleeAttack();
        }
        else if (PlayerInSightRange())
        {
            agent.isStopped = false;
            agent.SetDestination(Player.position);
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
}

public enum BossStage { Stage_one, Stage_two, Dead }