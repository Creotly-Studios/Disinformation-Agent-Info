using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class RobotMovement : MonoBehaviour
{
    Robot robot;

    [Header("Locomotion Parameters")]
    public float movementSpeed;
    public float rotationSpeed;

    [Header("Gravity Parameters")]
    [SerializeField] private Vector3 verticalVelocity;
    [SerializeField] protected float gravityForce = -30.0f;

    private void Awake()
    {
        robot = GetComponent<Robot>();
    }

    public void RobotMovement_Update(float delta)
    {
        HandleGravity(delta);
    }

    //Functionalities

    private void HandleGravity(float delta)
    {
        robot.isGrounded = robot.characterController.isGrounded;
        if(robot.isGrounded)
        {
            if (verticalVelocity.y < 0)
            {
                verticalVelocity.y = -2f;
            }
        }
        else
        {
            verticalVelocity.y += gravityForce * delta;
        }
        robot.characterController.Move(verticalVelocity * delta);
    }

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
