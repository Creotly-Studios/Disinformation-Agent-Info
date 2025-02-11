using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyCombatControllerScript : MonoBehaviour
{
    int index;
    public static EnemyCombatControllerScript Instance { get; private set; }

    private Coroutine enemyLoopRoutine;
    public Robot attackingRobot { get; private set; }

    [Header("Robot Teams")]
    private List<int> robotIndexList = new List<int>();
    public List<Robot> robotList { get; private set; } =  new List<Robot>();

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Multiple Instances");
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        enemyLoopRoutine = StartCoroutine(EnemyCombatLoop(null));
    }

    public void AddEnemy(Robot robot)
    {
        if (robotList.Contains(robot))
        {
            return;
        }
        robotList.Add(robot);
    }

    public void RemoveEnemy(Robot robot)
    {
        if (robotList.Contains(robot) != true) 
        { 
            return; 
        }
        robotList.Remove(robot);
    }

    public Robot RandomRobot(Robot excludeRobot)
    {
        robotIndexList.Clear();
        for(int i = 0; i < robotList.Count; i++)
        {
            if(excludeRobot != null)
            {
                if (robotList[i] == excludeRobot)
                {
                    continue;
                }
            }

            if(robotList[i].currentState != robotList[i].combatState)
            {
                continue;
            }
            robotIndexList.Add(i);
        }

        if(robotIndexList.Count == 0)
        {
            return null;
        }
        int random = Random.Range(0, robotIndexList.Count);
        return robotList[robotIndexList[random]];
    }

    private void SetStoppingDistance(PunchSO currentAttack)
    {
        foreach (Robot robot in robotList)
        {
            if (robot == attackingRobot)
            {
                robot.agent.stoppingDistance = currentAttack.distanceToAttack.upperBound;
                continue;
            }
            robot.agent.stoppingDistance = 5.75f;
        }
    }

    private IEnumerator EnemyCombatLoop(Robot robot)
    {
        if(robotList.Count == 0)
        {
            enemyLoopRoutine = StartCoroutine(EnemyCombatLoop(null));
            yield break;
        }
        float random = Random.Range(0.5f, 1.5f);
        yield return new WaitForSeconds(random);

        attackingRobot = RandomRobot(robot);
        index++;
        
        if (attackingRobot == null)
        {
            enemyLoopRoutine = StartCoroutine(EnemyCombatLoop(null));
            yield break;
        }
        RobotCombat robotCombat = attackingRobot.robotCombat;
        yield return new WaitUntil(() => attackingRobot.isStunned == false);
        yield return new WaitUntil(() => attackingRobot.isRetreating == false);

        attackingRobot.combatState.GetOffensiveActions(attackingRobot);
        yield return new WaitUntil(() => robotCombat.currentAttack != null);

        SetStoppingDistance(robotCombat.currentAttack);
        yield return new WaitUntil(() => attackingRobot.inAttackRange == true);

        attackingRobot.robotCombat.AttackTarget(attackingRobot);
        yield return new WaitUntil(() => robotCombat.currentAttack == null);

        attackingRobot.agent.stoppingDistance = 5.75f;
        attackingRobot.robotCombat.HandleRetreat(attackingRobot);

        float rnd = Random.Range(0, 0.75f);
        yield return new WaitForSeconds(rnd);

        if (robotList.Count > 0)
        {
            enemyLoopRoutine = StartCoroutine(EnemyCombatLoop(attackingRobot));
            yield break;
        }
    }
}
