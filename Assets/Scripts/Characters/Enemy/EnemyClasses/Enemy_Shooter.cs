using UnityEngine;

public class Enemy_Shooter : Enemy
{
    private float lastFire;
    private Vector3 _aimDir;

    void Update()
    {
        if (Player != null && !Player_v2.Instance.IsPlayerDead())
        {
            CheckForAndShootPlayer();
        }
    }

    void CheckForAndShootPlayer()
    {
        if (PlayerInAttackRange() && Time.time > lastFire + e_data.shootRate)
        {
            // Set aim direction toward the player
            _aimDir = (Player.position - attackPoint.position).normalized;

            // Ensure there's a clear line of sight before shooting
            if (CanSeePlayer())
            {
                CreateProjectile(_aimDir);
                lastFire = Time.time;
            }
        }
    }

    void CreateProjectile(Vector3 dir)
    {
        // Spawn projectile at the attack point
        GameObject projectile = Instantiate(e_data.projectile, attackPoint.position, Quaternion.LookRotation(dir));

        // EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
        // if (projectileScript != null)
        // {
        //     projectileScript.Setup(dir, enemy.e_data.projectileSpeed, enemy.e_data.projectileShelfLife, enemy.e_data);
        // }
    }

    bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 direction = Player.position - transform.position;
        
        if (Physics.Raycast(transform.position, direction, out hit, e_data.attackRange))
        {
            return hit.collider.CompareTag("Player"); // Ensure nothing blocks the shot
        }
        return false;
    }
}
