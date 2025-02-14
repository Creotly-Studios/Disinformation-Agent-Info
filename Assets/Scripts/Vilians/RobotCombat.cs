using UnityEngine;

public class RobotCombat : MonoBehaviour
{
    [Header("Attack Properties")]
    public Transform attackPoint;
    public float attackRange = 2f;
    public float sphereRadius = 0.5f;

    [Header("Selected Attack Styles")]
    public PunchSO currentAttack;
    public PunchSO[] attackArray;

    private void Awake()
    {
        for (int i = 0; i < attackArray.Length; i++)
        {
            attackArray[i] = Instantiate(attackArray[i]);
        }
    }

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
                    damagable.TakeDamage(damage);
                }
            }
        }
    }

    public void RobotCombat_Updater(float delta, Robot robot)
    {
        if (robot.isRetreating == true)
        {
            if(robot.dontMove)
            {
                return;
            }

            if(robot.DistanceToTarget <= 3.75f)
            {
                robot.robotAnimation.SetBlendTreeParameter(0.0f, -1.0f, false, delta);
                return;
            }
            robot.isRetreating = false;
        }

        if(currentAttack != null && EnemyCombatControllerScript.Instance.attackingRobot == robot)
        {
            robot.inAttackRange = (robot.DistanceToTarget <= currentAttack.distanceToAttack.upperBound);
            MoveTowardsAttackRange(robot);
        }
    }

    public void AttackTarget(Robot robot)
    {
        robot.robotMovement.HandleRotationWhileAttacking();
        robot.robotAnimation.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);

        if(robot.performingAction)
        {
            return;
        }
        currentAttack.PerformAttackAction(robot);
        //CheckAndDamage(currentAttack.damage);
        Invoke(nameof(HasAttacked), 0.35f);
    }

    private void HasAttacked()
    {
        currentAttack = null;
    }    

    public void HandleRetreat(Robot robot)
    {
        robot.isMoving = true;
        robot.isRetreating = true;
    }

    public void MoveTowardsAttackRange(Robot robot)
    {
        if(robot.dontMove || robot.inAttackRange)
        {
            robot.isMoving = false;
            return;
        }
        robot.isMoving = true;
        robot.agent.enabled = true;
        Vector3 targetPosition = robot.target.TargetPosition;

        robot.robotAnimation.SetBlendTreeParameter(0f, 2.0f, true, Time.deltaTime);
        robot.robotMovement.HandleMovement(targetPosition, robot.robotMovement.movementSpeed);
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
