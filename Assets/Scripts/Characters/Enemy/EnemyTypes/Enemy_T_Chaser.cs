using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
public class Enemy_T_Chaser : MonoBehaviour
{
    public enum ChaserType { Direct, Roaming }
    public ChaserType chaserType;
    
    [Header("Roaming Settings")]
    [SerializeField] private float minRoamDistance = 5f;
    [SerializeField] private float maxRoamDistance = 15f;
    [SerializeField] private float roamUpdateInterval = 3f;

    private Enemy enemy;
    private NavMeshAgent agent;
    private Vector3 roamTarget;
    private bool isRoaming = false;
    private float nextRoamUpdate;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();
        // Set NavMeshAgent's stopping distance to 0 to prevent conflicts
        agent.stoppingDistance = 0f;
        nextRoamUpdate = Time.time + roamUpdateInterval;

        if (chaserType == ChaserType.Roaming)
        {
            SetNewRoamTarget();
        }
    }

    void Update()
    {
        if (!IsValidGameState()) return;
        if (enemy.isKnockedBack) return; 

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

    bool IsValidGameState()
    {
        return enemy != null 
            && enemy.Player != null 
            && enemy.currentHealth > 0 
            && !enemy.IsDead();
    }

    void DirectChase()
    {
        if (enemy.PlayerInSightRange())
        {
            MoveTowardsTargetWithOffset(enemy.Player.position);
        }
        else
        {
            agent.ResetPath();
        }
    }

    void RoamingChase()
    {
        bool playerVisible = enemy.PlayerInSightRange() && 
                           Player_v2.Instance != null && 
                           !Player_v2.Instance.IsPlayerDead();

        if (playerVisible)
        {
            MoveTowardsTargetWithOffset(enemy.Player.position);
            isRoaming = false;
        }
        else
        {
            HandleRoaming();
        }
    }

    void MoveTowardsTargetWithOffset(Vector3 targetPosition)
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget > enemy.e_data.stopDistance)
        {
            // Calculate the position that maintains the minimum distance
            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            Vector3 targetWithOffset = targetPosition - (directionToTarget * enemy.e_data.stopDistance);
            
            // Only update if we're outside the stopping distance threshold
            if (Vector3.Distance(transform.position, targetPosition) > enemy.e_data.stopDistance + 0.1f)
            {
                UpdateDestination(targetWithOffset);
            }
            else
            {
                agent.ResetPath();
            }
        }
        else
        {
            // If we're already closer than the stopping distance, stop moving
            agent.ResetPath();
        }
    }

    void HandleRoaming()
    {
        if (!isRoaming || agent.remainingDistance < 0.5f || Time.time >= nextRoamUpdate)
        {
            SetNewRoamTarget();
            nextRoamUpdate = Time.time + roamUpdateInterval;
        }
    }

    void UpdateDestination(Vector3 target)
    {
        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(target);
        }
    }

    void SetNewRoamTarget()
    {
        float randomDistance = Random.Range(minRoamDistance, maxRoamDistance);
        Vector3 randomDirection = Random.insideUnitSphere * randomDistance;
        randomDirection += transform.position;
        NavMeshHit hit;

        int maxAttempts = 5;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            if (NavMesh.SamplePosition(randomDirection, out hit, randomDistance, NavMesh.AllAreas))
            {
                roamTarget = hit.position;
                UpdateDestination(roamTarget);
                isRoaming = true;
                break;
            }
            randomDirection = Random.insideUnitSphere * randomDistance + transform.position;
            attempts++;
        }
    }
}