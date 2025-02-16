using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy_T_Chaser : MonoBehaviour
{
    public enum ChaserType { Direct, Roaming }
    public ChaserType chaserType;

    private Enemy enemy;
    private NavMeshAgent agent;
    private Vector3 roamTarget;
    private bool isRoaming = false;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();

        if (chaserType == ChaserType.Roaming)
        {
            SetNewRoamTarget();
        }
    }

    void Update()
    {
        if (enemy.Player == null || enemy.currentHealth <= 0) return;
        if(Player_v2.Instance!=null && Player_v2.Instance.IsPlayerDead()) {
            RoamingChase();
            return;
        }

        switch (chaserType)
        {
            case ChaserType.Direct:
                DirectChase();
                break;
            
            case ChaserType.Roaming:
                RoamingChase();
                break;
        }
    }

    void DirectChase()
    {
        if (enemy.PlayerInSightRange())
        {
            agent.SetDestination(enemy.Player.position);
        }
        else
        {
            agent.ResetPath();
        }
    }

    void RoamingChase()
    {
        if (enemy.PlayerInSightRange() && Player_v2.Instance != null && !Player_v2.Instance.IsPlayerDead())
        {
            agent.SetDestination(enemy.Player.position);
            isRoaming = false;
        }
        else
        {
            if (!isRoaming || agent.remainingDistance < 0.5f)
            {
                SetNewRoamTarget();
            }
        }
    }

    void SetNewRoamTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * enemy.e_data.detectRange;
        randomDirection += transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, enemy.e_data.detectRange, NavMesh.AllAreas))
        {
            roamTarget = hit.position;
            agent.SetDestination(roamTarget);
            isRoaming = true;
        }
    }
}
