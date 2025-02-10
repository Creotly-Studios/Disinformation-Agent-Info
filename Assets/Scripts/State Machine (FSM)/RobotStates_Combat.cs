using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "Combat State", menuName = "Creotly Studio/RobotStates/CombatState")]
public class RobotStates_Combat : RobotStates
{
    private bool hasAction;
    private List<PunchSO> possibleActions = new List<PunchSO>();

    private float verticalMovement;
    private float horizontalMovement;
    private float maxEngagementDistance = 5.0f;

    [Header("Logic")]
    [SerializeField] private float attackLikelihood;
    [SerializeField] private float minimumAttackRange;

    public override RobotStates RobotState_Update(Robot robot)
    {
        if (robot.performingAction)
        {
            robot.robotAnimation.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);
            return this;
        }

        HandleStrafingParameters(robot);
        if(robot.dontMove != true && robot.agent.enabled == false)
        {
            robot.agent.enabled = true;
        }
        robot.robotMovement.HandleRotationWhileAttacking();

        if (robot.target.Source == null)
        {
            return SwitchState(robot.idleState, robot);
        }

        if (robot.DistanceToTarget >= minimumAttackRange)
        {
            return SwitchState(robot.pursueState, robot);
        }

        if(EnemyCombatControllerScript.Instance.attackingRobot != robot)
        {
            HandleStrafingMovement(robot);
        }
        return this;
    }

    //Functionalities
    protected override void ResetStateParameters(Robot robot)
    {
        hasAction = false;
        possibleActions.Clear();
        base.ResetStateParameters(robot);
    }

    private void HandleStrafingMovement(Robot robot)
    {
        Vector3 targetPosition = robot.target.TargetPosition;
        Vector3 targetLookAt = new (targetPosition.x, robot.transform.position.y, targetPosition.z);

        robot.isMoving = true;
        robot.transform.LookAt(targetLookAt);
        robot.robotAnimation.SetBlendTreeParameter(horizontalMovement, verticalMovement, false, Time.deltaTime);
    }

    private void HandleStrafingParameters(Robot robot)
    {
        horizontalMovement = RandomValue();
        //if distance to target is less than 2 strafe backward;
        verticalMovement = (robot.DistanceToTarget <= 2.0f) ? -1.0f : robot.DistanceToTarget / maxEngagementDistance;
    }

    private float RandomValue()
    {
        float randomValue = Random.Range(-1, 1);

        if (randomValue >= -1.0f && randomValue <= 0.0f)
        {
            return -0.5f;
        }
        return 0.5f;
    }

    public void GetOffensiveActions(Robot robot)
    {
        if (hasAction == true)
        {
            return;
        }

        RobotCombat robotCombat = robot.robotCombat;
        for (int i = 0; i < robotCombat.attackArray.Length; i++)
        {
            if (possibleActions.Contains(robotCombat.attackArray[i]))
            {
                continue;
            }

            if (robotCombat.attackArray[i].attackAngle.lowerBound > robot.AngleOfTarget)
            {
                continue;
            }
            
            if (robotCombat.attackArray[i].attackAngle.upperBound < robot.AngleOfTarget)
            {
                continue;
            }
            possibleActions.Add(robotCombat.attackArray[i]);
        }

        int totalWeight = 0;
        for (int i = 0; i < possibleActions.Count; i++)
        {
            totalWeight += possibleActions[i].weight;
        }

        int randomWeight = Random.Range(1, totalWeight + 1);
        int processedWeight = 0;

        for (int i = 0; i < possibleActions.Count; i++)
        {
            processedWeight += possibleActions[i].weight;
            if (randomWeight <= processedWeight)
            {
                hasAction = true;
                robotCombat.currentAttack = possibleActions[i];
                possibleActions.Clear();
                break;
            }
        }
    }
}
