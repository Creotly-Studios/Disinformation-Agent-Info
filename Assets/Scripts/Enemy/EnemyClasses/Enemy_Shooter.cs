using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Enemy_Shooter : MonoBehaviour
{
    Enemy enemy;

    float lastFire;
    private Vector3 _aimDir;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if(enemy.Player != null)
        {
            if(Player_v2.Instance != null && !Player_v2.Instance.IsPlayerDead())
            {
                CheckForAndShootPlayer();
            }
        }
    }

    void CheckForAndShootPlayer()
    {

        if (enemy.PlayerInAttackRange())
        {
            if (Time.time > lastFire + enemy.e_data.shootRate)
            {
                CreateProjectile(_aimDir);
                lastFire = Time.time;
            }
        }
    }

    void CreateProjectile(Vector3 dir)
    {
        GameObject projectile = Instantiate(enemy.e_data.projectile, enemy.attackPoint.position, Quaternion.identity) as GameObject;
        // EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
        // if (projectileScript != null)
        // {
        //     projectileScript.Setup(dir, baseEnemyData.projectileSpeed, baseEnemyData.projectileShelfLife, baseEnemyData);
        // }
    }
}
