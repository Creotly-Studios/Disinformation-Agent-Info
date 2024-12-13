using UnityEngine;

public class RobotCombat : MonoBehaviour
{
    public float attackRange = 2f; // Range of the spherecast
    public float sphereRadius = 0.5f; // Radius of the sphere
    public Transform attackPoint;

    public void CheckAndDamage(int damage)
    {
        RaycastHit[] hits = Physics.SphereCastAll(attackPoint.position, sphereRadius, attackPoint.forward, attackRange);
        foreach (RaycastHit hit in hits)
        {
            // Check if the object hit has an enemy tag or component
            IDamagable damagable = hit.collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                // Check if the enemy is in front of the player
                Vector3 directionToEnemy = (hit.collider.transform.position - attackPoint.position).normalized;
                float dotProduct = Vector3.Dot(attackPoint.forward, directionToEnemy);

                if (dotProduct > 0.5f) // Adjust threshold to control front-facing precision
                {
                    damagable.TakeDamage(damage, AnimatorHashing.damageAnimation);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, sphereRadius);
            Gizmos.DrawLine(attackPoint.position, attackPoint.position + attackPoint.forward * attackRange);
            Gizmos.DrawWireSphere(attackPoint.position + attackPoint.forward * attackRange, sphereRadius);
        }
    }
}
