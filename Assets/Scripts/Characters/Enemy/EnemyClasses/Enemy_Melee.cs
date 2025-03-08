using System.Collections;
using UnityEngine;

public class Enemy_Melee : Enemy
{
    private float lastAttack;
    [SerializeField] private float delayAmount;

    void Update()
    {
        if (currentHealth <= 0 || IsDead() || Player_v2.Instance.IsPlayerAttacking()) return;

        if (Player != null && !Player_v2.Instance.IsPlayerDead())
        {
            StartCoroutine(CallPlayerAttack());
        }
    }

    IEnumerator CallPlayerAttack()
    {
        yield return new WaitForSeconds(delayAmount);
        CheckForAndMeleePlayer();
        StopAllCoroutines();
    }

    private void CheckForAndMeleePlayer()
    {
        // Check if enough time has passed since last attack and player is in range
        if (Time.time > lastAttack + e_data.attackRate && PlayerInAttackRange())
        {
            // Request permission to attack from the manager
            if (EnemyAttackManager.Instance.RequestAttackPermission(this))
            {
                PerformMeleeAttack();
            }
        }
    }

    protected override void PerformAttack()
    {
        PerformMeleeAttack();
    }

    private void PerformMeleeAttack()
    {
        PlayAttackAnim();
        IDamagable damagable = Player.GetComponent<IDamagable>();
        damagable?.TakeDamage(e_data.damage);
        lastAttack = Time.time;

        // Release attack permission after attack animation time
        // You might want to adjust this timing based on your animation length
        Invoke("FinishAttack", e_data.attackRate * 0.5f);
    }

    // Optional: You can add this to handle the actual damage at the animation event
    public void OnMeleeAnimationHit()
    {
        if (PlayerInAttackRange())
        {
            IDamagable damagable = Player.GetComponent<IDamagable>();
            damagable?.TakeDamage(e_data.damage);
        }
    }
}