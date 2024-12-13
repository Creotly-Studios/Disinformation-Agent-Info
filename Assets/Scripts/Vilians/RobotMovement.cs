using UnityEngine;
using UnityEngine.AI;

public class RobotMovement : MonoBehaviour
{
    Robot robot;

    public float movementSpeed;
    public float rotationSpeed;

    private void Awake()
    {
        robot = GetComponent<Robot>();
    }

    private void RobotMovement_Update()
    {

    }

    //Functionalities

    public void HandleRotationWhileAttacking(Robot robot)
    {
        if (robot.target.Target == null)
        {
            return;
        }

        if (robot.canRotate != true)
        {
            return;
        }

        if (robot.performingAction != true)
        {
            return;
        }

        Vector3 targetDirection = robot.DirectionToTarget;
        targetDirection.y = 0.0f;
        targetDirection.Normalize();

        if (robot.DirectionToTarget == Vector3.zero)
        {
            targetDirection = robot.transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        robot.transform.rotation = Quaternion.Slerp(robot.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void PivotTowardsTarget(Robot robot)
    {
        if (robot.performingAction)
        {
            return;
        }

        if (robot.AngleOfTarget >= 20 && robot.AngleOfTarget <= 145)
        {
            robot.robotAnimation.PlayRootTargetAnimation(AnimatorHashing.turn_R_90, true);
        }
        else if (robot.AngleOfTarget >= -145 && robot.AngleOfTarget <= -20)
        {
            robot.robotAnimation.PlayRootTargetAnimation(AnimatorHashing.turn_L_90, true);
        }
        else if (robot.AngleOfTarget > 145 && robot.AngleOfTarget <= 180)
        {
            robot.robotAnimation.PlayRootTargetAnimation(AnimatorHashing.turn_R_180, true);
        }
        else if (robot.AngleOfTarget < -145 && robot.AngleOfTarget >= -180)
        {
            robot.robotAnimation.PlayRootTargetAnimation(AnimatorHashing.turn_L_180, true);
        }

    }

    public void RotateTowardsTarget()
    {
        if (robot.isMoving == true)
        {
            robot.transform.rotation = robot.agent.transform.rotation;
        }
    }

    public void HandleMovement(Vector3 targetPosition, float speed)
    {
        if (robot.navMeshPath == null)
        {
            robot.navMeshPath = new NavMeshPath();
        }
        if (robot.navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            robot.navMeshPath.ClearCorners();
        }
        if (robot.agent.CalculatePath(targetPosition, robot.navMeshPath))
        {
            robot.agent.SetPath(robot.navMeshPath);
        }

        Vector3 moveDirection = robot.agent.desiredVelocity;
        robot.characterController.Move(speed * Time.deltaTime * moveDirection);
    }
}
