using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Boss : MonoBehaviour, IDamagable, ISaveable
{
    private ObjectSaveData saveData;

    private Collider[] buffer;

    [Header("References")]
    public Transform Player { get; protected set; }
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;
    [SerializeField] private UnityEngine.UI.Image healthBarFill;
    [SerializeField] private GameObject bossNPC;
    
    [Header("Detection Settings")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private float detectRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float chaseOffset = 3f;
    
    [Header("Health Settings")]
    public int maxHealth = 10;
    [SerializeField] private int stageTransitionThreshold = 6;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] Transform meleeAttackPoint;
    [SerializeField] private int attackDamage = 1;
    
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDuration = 0.5f;
    [SerializeField] private float knockbackForce = 10f;
    
    [Header("Enemy Spawn Settings")]
    [SerializeField] Transform shootPoint;
    [SerializeField] GameObject[] enemiesToSpawn;
    [SerializeField] int totalEnemiesToSpawnCount = 10;
    [SerializeField] float spawnEnemyRate = 3f;
    
    // State tracking
    public BossStage Stage { get; private set; }
    private int currentHealth;
    private bool isDead;
    private bool isKnockedBack;
    private float nextSpawnTime;
    private float lastAttackTime;
    private Rigidbody rb;
    private readonly List<GameObject> spawnedEnemies = new();

    #region Unity Lifecycle Methods
    
    protected void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }
    
    protected void Start()
    {
        // Initialize
        saveData = new()
        {
            name = name
        };
        buffer = new Collider[5];
        SetBossStage(BossStage.Stage_one);
        EventBus.Save.OnRegisterSaveableAsset?.Invoke(this);

        if (Player_v2.Instance != null)
            Player = Player_v2.Instance.transform;
            
        currentHealth = maxHealth;
        isDead = false;
        
        if (rb != null)
            rb.isKinematic = true;
            
        lastAttackTime = -attackCooldown; // Allow immediate first attack
        UpdateHealthBar();
    }
    
    protected void Update()
    {
        if (isDead)
        {
            EventBus.Quest.OnQuestObjectiveCompleted?.Invoke(true, false, ObjectiveType.FightBots, null);
            Destroy(this);
            return;
        }
        UpdateStageState();
    }

    #endregion

    #region Health Management

    public void TakeDamage(int healthDamage)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - healthDamage);
        UpdateHealthBar();
        
        if (currentHealth <= 0)
        {
            HandleDeath();
        }
        else
        {
            // Only apply knockback if not dead
            Vector3 knockbackDirection = (transform.position - Player.transform.position).normalized;
            ApplyKnockback(knockbackDirection);
        }
    }
    
    private void HandleDeath()
    {
        if (!isDead)
        {
            isDead = true;
            PlayDeadAnim();
            agent.isStopped = true;
            agent.enabled = false;
            
            // Cancel any pending knockback reset
            CancelInvoke(nameof(ResetAfterKnockback));
            
            // Set stage to dead
            SetBossStage(BossStage.Dead);
        }
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float healthNormalized = (float)currentHealth / maxHealth;
            healthBarFill.fillAmount = healthNormalized;
        }
    }
    
    #endregion
    
    #region Stage Management
    
    private void UpdateStageState()
    {
        // Check for stage transitions
        if (currentHealth <= stageTransitionThreshold && Stage == BossStage.Stage_one)
        {
            SetBossStage(BossStage.Stage_two);
        }

        // Handle current stage behavior
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
    
    private void SetBossStage(BossStage newStage)
    {
        Stage = newStage;
        
        // Handle stage-specific initialization
        switch (newStage)
        {
            case BossStage.Stage_one:
                // Stage one initialization
                break;
                
            case BossStage.Stage_two:
                // Stage two initialization
                break;
                
            case BossStage.Dead:
                // Death initialization
                break;
        }
    }
    
    #endregion
    
    #region Stage Behaviors
    
    private void HandleStageOne()
    {
        if (isKnockedBack || Player == null) return;
        
        if (PlayerInSightRange())
        {
            ChasePlayer();
            
            // Spawn enemies at the defined rate
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
    
    private void HandleStageTwo()
    {
        if (isKnockedBack || Player == null) return;
        
        if (PlayerInAttackRange())
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // Stop moving when attacking
                agent.isStopped = true;
                PerformMeleeAttack();
                lastAttackTime = Time.time;
            }
        }
        else if (PlayerInSightRange())
        {
            ChasePlayer();
        }
        else
        {
            PlayIdleAnim();
        }
    }
    
    private void HandleDeathStage()
    {
        // if (AreAllEnemiesDead())
        // {
            if (bossNPC != null)
            {
                Instantiate(bossNPC, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        // }
    }
    
    private void ChasePlayer()
    {
        if (agent == null || !agent.isActiveAndEnabled) return;
        
        agent.isStopped = false;
        Vector3 directionToPlayer = (Player.position - transform.position).normalized;
        Vector3 destination = Player.position - directionToPlayer * chaseOffset;
        agent.SetDestination(destination);
        
        if (animator != null && !PlayerInAttackRange())
        {
            animator.SetBool("isWalking", true);
        } else {
            PlayIdleAnim();
        }
    }
    
    #endregion
    
    #region Enemy Spawning
    
    private void SpawnEnemies()
    {
        if (totalEnemiesToSpawnCount <= 0 || shootPoint == null || enemiesToSpawn.Length == 0) return;
        
        int randomIndex = Random.Range(0, enemiesToSpawn.Length);
        GameObject enemyPrefab = enemiesToSpawn[randomIndex];
        
        if (enemyPrefab != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, shootPoint.position, shootPoint.rotation);
            if (enemy != null)
            {
                spawnedEnemies.Add(enemy);
                totalEnemiesToSpawnCount--;
            }
        }
    }
    
    private bool AreAllEnemiesDead()
    {
        return false;
        //will add back logi for checking if all enemies are dead later on
    }
    
    #endregion
    
    #region Combat
    
    public bool PlayerInSightRange()
    {
        return Player != null && Physics.CheckSphere(transform.position, detectRange, whatIsPlayer);
    }

    public bool PlayerInAttackRange()
    {
        return Player != null && Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
    }
    
    // Used by animation event
    public void OnAttackAnimationEvent()
    {
        // Don't play animation again, just apply damage
        ApplyMeleeDamage();
    }
    
    private void PerformMeleeAttack()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetTrigger("attack");
        }
        
        // Actual damage will be applied via animation event
    }
    
    private void ApplyMeleeDamage()
    {
        if (meleeAttackPoint == null)
        {
            return;
        }
        
        int count = Physics.OverlapSphereNonAlloc(meleeAttackPoint.position, attackRange, buffer, whatIsPlayer);
        for(int i = 0; i < count; i++)
        {
            Collider player = buffer[i];
            if (player != null && player.CompareTag("Player"))
            {
                if (player.TryGetComponent<IDamagable>(out var damagable))
                {
                    damagable.TakeDamage(attackDamage);
                }
            }
        }
    }
    
    #endregion
    
    #region Animation
    
    public void PlayIdleAnim()
    {
        if (animator != null)
        {
            animator.SetBool("idle", true);
            animator.SetBool("isWalking", false);
        }
    }
    
    public void PlayDeadAnim()
    {
        if (animator != null)
        {
            animator.SetBool("dead", true);
        }
    }
    
    #endregion
    
    #region Knockback
    
    private void ApplyKnockback(Vector3 direction)
    {
        if (isKnockedBack || rb == null) return;

        isKnockedBack = true;
        
        if (agent != null && agent.isActiveAndEnabled)
            agent.enabled = false;
            
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
        }

        Invoke(nameof(ResetAfterKnockback), knockbackDuration);
    }

    private void ResetAfterKnockback()
    {
        isKnockedBack = false;
        
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }
        
        if (agent != null && !isDead)
            agent.enabled = true;
    }
    #endregion

    public ObjectSaveData GetSaveData()
    {
        return saveData;
    }

    public void ReloadDataFromSavedFile(ObjectSaveData saveData)
    {
        transform.SetPositionAndRotation(saveData.ObjectPosition, saveData.ObjectRotation);
    }
        
    public void UpdateSavedData()
    {
        saveData.UpdateSaveData(transform.position, transform.rotation, false);
    }
}

public enum BossStage { Stage_one, Stage_two, Dead }