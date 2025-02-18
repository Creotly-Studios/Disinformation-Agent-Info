using UnityEngine;

public class Enemy_Melee : Enemy
{
    private float lastAttack;

    void Update()
    {
        if (currentHealth <= 0) return; // Stop attacking if dead
        if (Player != null && !Player_v2.Instance.IsPlayerDead())
        {
            CheckForAndMeleePlayer();
        }
    }

    private void CheckForAndMeleePlayer()
    {
        if (Time.time > lastAttack + e_data.attackRate && PlayerInAttackRange())
        {
            IDamagable damagable = Player.GetComponent<IDamagable>();
            damagable?.TakeDamage(e_data.damage);
            lastAttack = Time.time; // Update attack timer
        }
    }
}
