using UnityEngine;

[CreateAssetMenu(fileName = "Pursue State", menuName = "Creotly Studio/RobotStates/PursueState")]
public class RobotStates_Pursue : RobotStates
{
    [SerializeField] protected float timeInState = 7.5f;

    public override RobotStates RobotState_Update(Robot robot)
    {
        if(robot.performingAction)
        {
            robot.robotAnimation.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);
            return this;
        }

        timeInState -= Time.deltaTime;
        if (robot.target.Target == null)
        {
            return SwitchState(robot.idleState, robot);
        }

        if (robot.agent.enabled == false)
        {
            robot.agent.enabled = true;
        }
        return HumanoidEnemy_Updater(robot);
    }

    private RobotStates HumanoidEnemy_Updater(Robot robot)
    {
        robot.robotMovement.RotateTowardsTarget();
        return VisualChase(robot);
    }

    private RobotStates VisualChase(Robot robot)
    {
        if (robot.DistanceToTarget >= robot.agent.stoppingDistance)
        {
            robot.robotAnimation.HandleAnimation(robot.agent.stoppingDistance);

            Vector3 targetPosition = robot.target.Target.transform.position;
            robot.robotMovement.HandleMovement(targetPosition, robot.robotMovement.movementSpeed);
        }
        else
        {
            Debug.Log("reached");
            robot.isMoving = false;
            robot.agent.enabled = false;
            return SwitchState(robot.combatState, robot);
        }
        return this;
    }
}
