using System;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Enemy_Kamikaze : MonoBehaviour
{
    private Enemy enemy;
    private bool _hasAttacked = false;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (_hasAttacked) return; // Prevent multiple explosions

        // If player exists and is in attack range, explode
        if (enemy.Player != null && enemy.PlayerInAttackRange())
        {
            Explode();
        }
    }

    private void Explode()
    {
        _hasAttacked = true; // Mark as exploded to prevent multiple attacks

        // Damage player
        IDamagable damagable = enemy.Player.GetComponent<IDamagable>();
        damagable?.TakeDamage(enemy.e_data.damage);

        // Play explosion effect if assigned
        if (enemy.e_data.deathEffect != null)
        {
            Instantiate(enemy.e_data.deathEffect, transform.position, Quaternion.identity);
        }

        // Destroy the enemy (simulate explosion)
        enemy.TakeDamage(1000);
    }
}
