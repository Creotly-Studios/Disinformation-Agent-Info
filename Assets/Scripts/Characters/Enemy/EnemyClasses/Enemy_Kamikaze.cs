using System;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Enemy_Kamikaze : MonoBehaviour
{
    Enemy enemy;
    private bool _hasAttacked = false;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.Player != null)
        {
            if (Player_v2.Instance != null && !Player_v2.Instance.IsPlayerDead())
            {
                KillSelfAndDamagePlayer();
            }
        }
    }

    private void KillSelfAndDamagePlayer()
    {
        _hasAttacked = true;
        IDamagable damagable = enemy.Player.GetComponent<IDamagable>();
        damagable?.TakeDamage(enemy.e_data.damage);
        enemy.TakeDamage(1000);
    }
}
