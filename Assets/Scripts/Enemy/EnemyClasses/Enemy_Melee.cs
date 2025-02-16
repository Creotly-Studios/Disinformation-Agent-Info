using System;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Enemy_Melee : MonoBehaviour
{
    Enemy enemy;

    float lastAttack;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.Player != null)
        {
            if (!Player_v2.Instance.IsPlayerDead())
            {
                CheckForAndMeleePlayer();
            }
        }
    }

    private void CheckForAndMeleePlayer()
    {
        if (Time.time > lastAttack + enemy.e_data.attackRate)
        {
            if (Vector3.Distance(transform.position, enemy.Player.position) <= enemy.e_data.attackRange)
            {
                IDamagable damagable = enemy.Player.GetComponent<IDamagable>();
                damagable?.TakeDamage(enemy.e_data.damage);
            }
            lastAttack = Time.time;
        }
    }
}
