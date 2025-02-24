using UnityEngine;

public class Enemy_Kamikaze : Enemy
{
    private bool _hasAttacked = false;

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player") && !_hasAttacked)
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

    protected override void PerformAttack()
    {
        Explode();
    }
}
