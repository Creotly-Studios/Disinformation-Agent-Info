using System;
using System.Collections;
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

    [SerializeField] bool isBotEnemy;

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
            KillTracker.Instance?.EnemyDied();
            PlayDeathSound();
            PlayDeathDisolve();
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

    public void PlayDeathSound()
    {
        if (isBotEnemy)
        {
            AudioManager.Instance.PlaySFX(e_data.botDie);
        } else AudioManager.Instance.PlaySFX(e_data.trollDie);
    }
    public void PlayHurtSound()
    {
        if (isBotEnemy)
        {
            AudioManager.Instance.PlaySFX(e_data.botHit);
        } else AudioManager.Instance.PlaySFX(e_data.trollHit);
    }

    void PlayDeathDisolve()
    {
        StartCoroutine(DisolveEffect());
    }

    private IEnumerator DisolveEffect()
    {
        // Get all renderers in the enemy and its children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Replace all materials with the dissolve material
        foreach (var renderer in renderers)
        {
            // Create an array of dissolve materials with the same length as the renderer's materials
            Material[] dissolveMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < dissolveMaterials.Length; i++)
            {
                dissolveMaterials[i] = e_data.dissolveMaterial; // Use the dissolve material from EnemyData
            }
            renderer.materials = dissolveMaterials; // Apply the dissolve materials
        }

        // Animate the dissolve effect
        float dissolveTime = e_data.destroyTime; // Use the destroy time from EnemyData
        float elapsedTime = 0f;

        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float cutoff = Mathf.Lerp(4f, -5f, elapsedTime / dissolveTime); // Adjust cutoff from 0 to 1

            // Update the _Cutoff property in all dissolve materials
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    material.SetFloat("Vector1_CFBBCBA", cutoff); // Update the _Cutoff property
                }
            }

            yield return null;
        }

        // Destroy the GameObject after the dissolve effect is complete
        Destroy(gameObject);
    }


}