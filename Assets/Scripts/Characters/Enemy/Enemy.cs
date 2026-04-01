using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))] // Add Rigidbody for knockback
public abstract class Enemy : MonoBehaviour, IDamagable, ISaveable
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
    protected CharacterSaveData saveData;
    public bool isKnockedBack; // Track if the enemy is currently being knocked back

    [SerializeField] bool isBotEnemy;

    [Header("Checks")]
    public Transform attackPoint;

    protected void Awake()
    {
        e_data = Instantiate(e_data);
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    protected void Start()
    {
        saveData = new()
        {
            name = name
        };
        EventBus.Save.OnRegisterSaveableAsset?.Invoke(this);
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

    protected void FinishAttack()
    {
        isAttacking = false;
        EnemyAttackManager.Instance.FinishAttack(this);
    }

    public void TakeDamage(int healthDamage)
    {
        if (isDead) return;

        currentHealth -= healthDamage;
        PlayHurtSound();
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

            EventBus.Quest.OnQuestObjectiveCompleted?.Invoke(true, false, ObjectiveType.FightBots, null);
            PlayDeathSound();
            PlayDeathDissolve();
            saveData.UpdateSaveData(0, currentHealth, transform.position, transform.rotation);
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

    public void PlayIdleAnim()
    {
        if (animator)
        {
            animator.SetBool("idle", true);
        }
    }

    public void PlayAttackAnim()
    {
        if (animator)
        {
            animator.SetTrigger("attack");
        }
    }

    public void PlayDeadAnim()
    {
        animator.SetBool("dead", true);
    }

    public void PlayDeathSound()
    {
        if (isBotEnemy)
        {
            AudioManager.Instance.PlaySFX(e_data.botDie);
            return;
        }
        AudioManager.Instance.PlaySFX(e_data.trollDie);
    }
    public void PlayHurtSound()
    {
        if (isBotEnemy)
        {
            AudioManager.Instance.PlaySFX(e_data.botHit);
            return;
        }
        AudioManager.Instance.PlaySFX(e_data.trollHit);
    }

    void PlayDeathDissolve()
    {
        StartCoroutine(DissolveEffect());
    }

    private IEnumerator DissolveEffect()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            Material[] dissolveMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < dissolveMaterials.Length; i++)
            {
                dissolveMaterials[i] = e_data.dissolveMaterial;
            }
            renderer.materials = dissolveMaterials;
        }
        float dissolveTime = e_data.destroyTime;
        float elapsedTime = 0f;

        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float cutoff = Mathf.Lerp(4f, -5f, elapsedTime / dissolveTime);
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    material.SetFloat("Vector1_CFBBCBA", cutoff);
                }
            }
            yield return null;
        }
        Destroy(gameObject);
    }

    public ObjectSaveData GetSaveData()
    {
        return saveData;
    }

    public void ReloadDataFromSavedFile(ObjectSaveData saveData)
    {
        CharacterSaveData characterData = saveData as CharacterSaveData;

        currentHealth = characterData.healthCount;
        if(currentHealth <= 0 && isDead != true)
        {
            isDead = true;
            Destroy(gameObject);
        }
        transform.SetPositionAndRotation(saveData.ObjectPosition, saveData.ObjectRotation);
    }

    public void UpdateSavedData()
    {
        if(saveData == null || this == null)
        {
            return;
        }
        saveData.UpdateSaveData(0, currentHealth, transform.position, transform.rotation);
    }
}