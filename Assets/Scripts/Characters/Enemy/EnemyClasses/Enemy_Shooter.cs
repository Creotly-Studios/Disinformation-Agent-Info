using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Enemy_Shooter : MonoBehaviour
{
    private Enemy enemy;
    private float lastFire;
    private Vector3 _aimDir;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (enemy.Player != null && !Player_v2.Instance.IsPlayerDead())
        {
            CheckForAndShootPlayer();
        }
    }

    void CheckForAndShootPlayer()
    {
        if (enemy.PlayerInAttackRange() && Time.time > lastFire + enemy.e_data.shootRate)
        {
            // Set aim direction toward the player
            _aimDir = (enemy.Player.position - enemy.attackPoint.position).normalized;

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
        GameObject projectile = Instantiate(enemy.e_data.projectile, enemy.attackPoint.position, Quaternion.LookRotation(dir));

        // EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
        // if (projectileScript != null)
        // {
        //     projectileScript.Setup(dir, enemy.e_data.projectileSpeed, enemy.e_data.projectileShelfLife, enemy.e_data);
        // }
    }

    bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 direction = enemy.Player.position - transform.position;
        
        if (Physics.Raycast(transform.position, direction, out hit, enemy.e_data.attackRange))
        {
            return hit.collider.CompareTag("Player"); // Ensure nothing blocks the shot
        }
        return false;
    }
}
