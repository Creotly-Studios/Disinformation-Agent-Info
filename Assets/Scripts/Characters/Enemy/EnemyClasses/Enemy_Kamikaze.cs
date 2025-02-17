using UnityEngine;

public class Enemy_Kamikaze : Enemy
{
    private bool _hasAttacked = false;

    void Update()
    {
        if (_hasAttacked) return; // Prevent multiple explosions

        // If player exists and is in attack range, explode
        if (Player != null && PlayerInAttackRange())
        {
            Explode();
        }
    }

    private void Explode()
    {
        _hasAttacked = true; // Mark as exploded to prevent multiple attacks

        // Damage player
        IDamagable damagable = Player.GetComponent<IDamagable>();
        damagable?.TakeDamage(e_data.damage);

        // Play explosion effect if assigned
        if (e_data.deathEffect != null)
        {
            Instantiate(e_data.deathEffect, transform.position, Quaternion.identity);
        }

        // Destroy the enemy (simulate explosion)
        TakeDamage(1000);
    }
}
