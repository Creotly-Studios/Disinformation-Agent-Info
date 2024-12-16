using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NPCStates", menuName = "Creotly Studio/NPCStates")]
public class NPCStates : ScriptableObject
{
    private float idleTime = 7.5f;

    [Header("General Parameters")]
    public float sphereRadius = 5.0f;

    [Header("Time")]
    public float idleTimeDefault = 7.5f;
    public float interactTimeDefault = 10.0f;

    [Header("Status")]
    private bool destinationSet;
    private Vector3 enemyDestination;
    public PatrolMode patrolMode = PatrolMode.Idle;

    public NPCStates RobotState_Update(NPC npcManager)
    {
        if (npcManager.performingAction)
        {
            npcManager.npcFunctions.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);
            return this;
        }

        if (npcManager.navMeshAgent.enabled == false)
        {
            npcManager.navMeshAgent.enabled = true;
        }

        //Handle Run Away From Violence
        return HandleAction(npcManager);
    }

    private NPCStates HandleAction(NPC npcManager)
    {
        if (npcManager.targetAngle < npcManager.angleLimit.lowerBound || npcManager.targetAngle > npcManager.angleLimit.upperBound)
        {
            npcManager.npcFunctions.PivotTowardsTarget();
        }

        npcManager.npcFunctions.RotateTowardsTarget();
        if (patrolMode == PatrolMode.Idle)
        {
            return Idle(npcManager);
        }
        return Walk(npcManager);
    }

    #region Actions

    private NPCStates Idle(NPC robot)
    {
        idleTime -= Time.deltaTime;
        robot.navMeshAgent.enabled = false;
        robot.npcFunctions.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);

        if (idleTime <= 0.0f)
        {
            if (destinationSet != true)
            {
                SetDestination(robot);
            }

            if (destinationSet)
            {
                destinationSet = false;
                robot.navMeshAgent.enabled = true;

                patrolMode = PatrolMode.Walk;
                return Walk(robot);
            }
        }
        return this;
    }

    private NPCStates Walk(NPC robot)
    {
        robot.SetPersonalTargetDetails(enemyDestination);
        if (robot.targetDistance >= robot.navMeshAgent.stoppingDistance)
        {
            robot.npcFunctions.SetBlendTreeParameter(0f, 0.55f, false, Time.deltaTime);
            robot.npcFunctions.HandleMovement(enemyDestination, robot.npcFunctions.movementSpeed);
        }
        else
        {
            patrolMode = PatrolMode.Idle;
            robot.navMeshAgent.enabled = false;
            return SwitchState(Idle(robot));
        }
        return this;
    }

    #endregion

    public NPCStates SwitchState(NPCStates nextState)
    {
        ResetStateFlags();
        return nextState;
    }

    protected void ResetStateFlags()
    {
        destinationSet = false;
        idleTime = idleTimeDefault;
    }

    private void SetDestination(NPC npcManager)
    {
        Vector3 randomPoint = Random.insideUnitSphere * sphereRadius + npcManager.transform.position;
        
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(randomPoint, out navMeshHit, sphereRadius, NavMesh.AllAreas))
        {
            enemyDestination = navMeshHit.position;
            npcManager.SetPersonalTargetDetails(enemyDestination);
            destinationSet = true;
            return;
        }
        destinationSet = false;
    }
}
