using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Combat State", menuName = "Creotly Studio/RobotStates/CombatState")]
public class RobotStates_Combat : RobotStates
{
    private bool hasAction;
    private List<PunchSO> possibleActions = new List<PunchSO>();

    [Header("Selected Attack Styles")]
    [SerializeField] PunchSO currentAttack;
    [SerializeField] PunchSO[] attackArray;

    [Header("Parameters")]
    [SerializeField] private bool hasPerformedAction;
    [SerializeField] private float minimumAttackRange;

    [Header("Logic")]
    [SerializeField] private float recoveryTimer;
    [SerializeField] private float attackLikelihood;

    public void InitializeState()
    {
        for (int i = 0; i < attackArray.Length; i++)
        {
            attackArray[i] = Instantiate(attackArray[i]);
        }
    }

    public override RobotStates RobotState_Update(Robot robot)
    {
        if (robot.performingAction)
        {
            robot.robotAnimation.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);
            return this;
        }

        if (robot.agent.enabled == false)
        {
            robot.agent.enabled = true;
        }
        robot.robotMovement.RotateTowardsTarget();

        if(recoveryTimer > 0.0f)
        {
            recoveryTimer -= Time.deltaTime;
        }

        if(robot.target.Target == null)
        {
            return SwitchState(robot.idleState, robot);
        }

        if (robot.DistanceToTarget >= minimumAttackRange)
        {
            return SwitchState(robot.pursueState, robot);
        }

        if(recoveryTimer <= 0.0f)
        {
            recoveryTimer = 0.0f;
            if (hasAction != true)
            {
                GetOffensiveActions(robot);
            }
            return HandleRobotAttack(robot);
        }

        if (robot.currentVisualTarget != null && robot.currentVisualTarget.isAttacking && robot.DistanceToTarget <= 1.75f)
        {
            return HandleRobotDefend(robot);
        }

        return this;
    }

    #region Handle Attack

    private RobotStates HandleRobotAttack(Robot robot)
    {
        robot.robotMovement.HandleRotationWhileAttacking(robot);
        robot.robotAnimation.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);

        if (robot.performingAction)
        {
            return this;
        }

        if (hasPerformedAction != true)
        {
            hasPerformedAction = true;
            AttackTarget(robot);
            return this;
        }
        //robot.robotAnimation.PlayTargetAnimation(AnimatorHashing.stepBackHash, true);
        recoveryTimer = currentAttack.recoveryTime;
        return SwitchState(robot.combatState, robot);
    }

    private void AttackTarget(Robot robot)
    {
        currentAttack.PerformAttackAction(robot.animator);
        robot.robotCombat.CheckAndDamage(currentAttack.damage);
    }

    #endregion

    #region Handle Defence

    private RobotStates HandleRobotDefend(Robot robot)
    {
        return this;
    }

    #endregion

    //Functionalities

    protected override void ResetStateParameters(Robot robot)
    {
        hasAction = false;
        //hasDodged = false;
        //hasDodgeDirection = false;
        currentAttack = null;
        possibleActions.Clear();
        hasPerformedAction = false;
        base.ResetStateParameters(robot);
    }

    protected void GetOffensiveActions(Robot robot)
    {
        for (int i = 0; i < attackArray.Length; i++)
        {
            if (attackArray[i].distanceToAttack.lowerBound > robot.DistanceToTarget)
            {
                continue;
            }

            if (attackArray[i].distanceToAttack.upperBound < robot.DistanceToTarget)
            {
                continue;
            }

            if (attackArray[i].attackAngle.lowerBound > robot.AngleOfTarget)
            {
                continue;
            }

            if (attackArray[i].attackAngle.upperBound < robot.AngleOfTarget)
            {
                continue;
            }

            if (possibleActions.Contains(attackArray[i]))
            {
                continue;
            }

            possibleActions.Add(attackArray[i]);
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
                currentAttack = possibleActions[i];
                possibleActions.Clear();
                break;
            }
        }
    }
}
