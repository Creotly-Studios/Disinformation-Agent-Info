using System;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Enemy_Melee : MonoBehaviour
{
    private Enemy enemy;
    private float lastAttack;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (enemy.currentHealth <= 0) return; // Stop attacking if dead
        if (enemy.Player != null && !Player_v2.Instance.IsPlayerDead())
        {
            CheckForAndMeleePlayer();
        }
    }

    private void CheckForAndMeleePlayer()
    {
        if (Time.time > lastAttack + enemy.e_data.attackRate && enemy.PlayerInAttackRange())
        {
            IDamagable damagable = enemy.Player.GetComponent<IDamagable>();
            damagable?.TakeDamage(enemy.e_data.damage);
            lastAttack = Time.time; // Update attack timer
        }
    }
}
