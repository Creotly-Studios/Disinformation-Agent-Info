using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Idle State", menuName = "Creotly Studio/RobotStates/IdleState")]
public class RobotStates_Idle : RobotStates
{
    private float idleTime = 7.5f;

    [Header("General Parameters")]
    public float sphereRadius = 5.0f;

    [Header("Time")]
    public float idleTimeDefault = 7.5f;
    public float interactTimeDefault = 10.0f;

    [Header("Status")]
    private bool destinationSet;
    public PatrolMode patrolMode = PatrolMode.Idle;

    public override RobotStates RobotState_Update(Robot robot)
    {
        if(robot.performingAction)
        {
            robot.robotAnimation.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);
            return this;
        }

        if(robot.dontMove != true && robot.agent.enabled == false)
        {
            robot.agent.enabled = true;
        }

        if (robot.target.Source != null)
        {
            return SwitchState(robot.pursueState, robot);
        }
        return IdleState_Updater(robot);
    }

    private RobotStates IdleState_Updater(Robot robot)
    {
        robot.robotMovement.RotateTowardsTarget();
        if (patrolMode == PatrolMode.Idle)
        {
            return Idle(robot);
        }
        return Walk(robot);
    }

    protected override void ResetStateParameters(Robot robot)
    {
        idleTime = idleTimeDefault;
        base.ResetStateParameters(robot);
    }

    //Functionalities

    private RobotStates Idle(Robot robot)
    {
        idleTime -= Time.deltaTime;
        robot.agent.enabled = false;
        robot.robotAnimation.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);

        if (idleTime <= 0.0f)
        {
            if (destinationSet != true)
            {
                SetDestination(robot);
            }
            else
            {
                destinationSet = false;
		idleTime = idleTimeDefault;
                robot.agent.enabled = true;

                patrolMode = PatrolMode.Walk;
            }
        }
        return this;
    }

    private RobotStates Walk(Robot robot)
    {
        robot.SetPersonalTargetDetails(enemyDestination);
        if (robot.DistanceToTarget >= robot.agent.stoppingDistance)
        {
            robot.robotAnimation.SetBlendTreeParameter(0f, 0.55f, false, Time.deltaTime);
            robot.robotMovement.HandleMovement(enemyDestination, robot.robotMovement.movementSpeed);
        }
        else
        {
            robot.isMoving = false;
            robot.agent.enabled = false;
            patrolMode = PatrolMode.Idle;
        }
        return this;
    }

    private void SetDestination(Robot robot)
    {
        Vector3 randomPoint = Random.insideUnitSphere * sphereRadius + robot.transform.position;

        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(randomPoint, out navMeshHit, sphereRadius, NavMesh.AllAreas))
        {
            enemyDestination = navMeshHit.position;
            robot.SetPersonalTargetDetails(enemyDestination);
            destinationSet = true;
            return;
        }
        destinationSet = false;
    }
}