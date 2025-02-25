using UnityEngine;

public class Enemy_Shooter : Enemy
{
    private float lastFire;
    private Vector3 _aimDir;

    void Update()
    {
        if (currentHealth <= 0 || IsDead()) return;
        
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

            // Request permission to attack from the manager
            if (EnemyAttackManager.Instance.RequestAttackPermission(this))
            {
                PerformAttack();
            }
        }
    }

    protected override void PerformAttack()
    {
        PlayAttackAnim();
        CreateProjectile(_aimDir);
        lastFire = Time.time;
        
        // Release attack permission after shooting
        // You might want to adjust this timing based on your animation length
        Invoke("FinishAttack", e_data.shootRate * 0.5f);
    }

    void CreateProjectile(Vector3 dir)
    {
        // Spawn projectile at the attack point
        GameObject projectile = Instantiate(e_data.projectile, attackPoint.position, Quaternion.LookRotation(dir));

        EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
        if (projectileScript != null)
        {
            projectileScript.Setup(dir, e_data.projectileSpeed, e_data.projectileShelfLife, e_data);
        }
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

    // Optional: You can add this to handle the actual shooting at the animation event
    public void OnShootAnimationEvent()
    {
        if (PlayerInAttackRange())
        {
            CreateProjectile(_aimDir);
        }
    }
}