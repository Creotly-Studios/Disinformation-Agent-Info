using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
public class Enemy_T_Waypointwalker : MonoBehaviour
{
    public float speed = 3f;
    public bool useNavMesh = false;

    private Transform[] waypoints;
    private int _currentWaypointIndex = 0;
    private bool _movingForward = true;
    private Enemy enemy;
    private NavMeshAgent agent;

    void Start()
    {
        enemy = GetComponent<Enemy>();

        // Get waypoints from the Waypoint Manager
        if (EnemyWayPointManager.Instance != null && EnemyWayPointManager.Instance.waypoints.Length > 0)
        {
            waypoints = EnemyWayPointManager.Instance.waypoints;
        }
        else
        {
            Debug.LogWarning("No waypoints found in EnemyWayPointManager!");
            return;
        }

        if (useNavMesh)
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = speed;
                agent.autoBraking = false;
            }
        }

        transform.position = waypoints[_currentWaypointIndex].position;
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (useNavMesh && agent != null)
        {
            PatrolWithNavMesh();
        }
        else
        {
            PatrolWithoutNavMesh();
        }
    }

    void PatrolWithNavMesh()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GetNextWaypoint();
            agent.SetDestination(waypoints[_currentWaypointIndex].position);
        }
    }

    void PatrolWithoutNavMesh()
    {
        Transform targetWaypoint = waypoints[_currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.2f)
        {
            GetNextWaypoint();
        }
    }

    void GetNextWaypoint()
    {
        if (_movingForward)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= waypoints.Length)
            {
                _currentWaypointIndex = waypoints.Length - 2;
                _movingForward = false;
            }
        }
        else
        {
            _currentWaypointIndex--;
            if (_currentWaypointIndex < 0)
            {
                _currentWaypointIndex = 1;
                _movingForward = true;
            }
        }
    }
}
