using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NPCStates", menuName = "Creotly Studio/NPCStates")]
public class PatrolState : ScriptableObject
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

    public PatrolState RobotState_Update(NPC npcManager)
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

    private PatrolState HandleAction(NPC npcManager)
    {
        npcManager.npcFunctions.RotateTowardsTarget();
        if (patrolMode == PatrolMode.Idle)
        {
            return Idle(npcManager);
        }
        return Walk(npcManager);
    }

    #region Actions

    private PatrolState Idle(NPC robot)
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
                patrolMode = PatrolMode.Walk;
                robot.navMeshAgent.enabled = true;

                idleTime = 6.0f;
                return Walk(robot);
            }
        }
        return this;
    }

    private PatrolState Walk(NPC robot)
    {
        idleTime -= Time.deltaTime;
        robot.SetPersonalTargetDetails(enemyDestination);
        bool isClose = robot.targetDistance < robot.navMeshAgent.stoppingDistance;

        if (isClose == true || idleTime <= 0.0f)
        {
            idleTime = 5.0f;
            patrolMode = PatrolMode.Idle;
            robot.navMeshAgent.enabled = false;
            return SwitchState(Idle(robot));
        }
        else
        {
            robot.npcFunctions.SetBlendTreeParameter(0f, 0.55f, false, Time.deltaTime);
            robot.npcFunctions.HandleMovement(enemyDestination, robot.npcFunctions.movementSpeed);
        }
        return this;
    }

    #endregion

    public PatrolState SwitchState(PatrolState nextState)
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
        Vector3 offsetDirection = Random.insideUnitSphere.normalized * 5.0f;
        Vector3 offsetPosition = npcManager.transform.position + offsetDirection;
        Vector3 randomPoint = Random.insideUnitSphere * sphereRadius + offsetPosition;
        
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(randomPoint, out navMeshHit, sphereRadius, NavMesh.AllAreas))
        {
            destinationSet = true;
            enemyDestination = navMeshHit.position;
            npcManager.SetPersonalTargetDetails(enemyDestination);
            return;
        }
        destinationSet = false;
    }
}
