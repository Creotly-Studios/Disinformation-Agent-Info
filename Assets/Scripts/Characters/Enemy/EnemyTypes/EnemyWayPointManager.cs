using UnityEngine;

public class EnemyWayPointManager : MonoBehaviour
{
    public Transform[] waypoints;
    public static EnemyWayPointManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple EnemyWayPointManager instances detected! Destroying duplicate.");
            Destroy(gameObject);
        }
    }
}
