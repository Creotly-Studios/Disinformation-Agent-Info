using System;
using UnityEngine;
using UnityEngine.Events;

public class KillTracker : MonoBehaviour
{
    public static KillTracker Instance { get; private set; }

    [Header("Kill Settings")]
    private int enemiesInScene = 0;
    private int currentKills = 0;

    [Header("Events")]
    public UnityEvent OnKillAllEnemies;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple KillTracker instances detected! Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Auto-count enemies at start (if enemies are tagged)
        enemiesInScene = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;
    }

    public void RegisterEnemy()
    {
        enemiesInScene++;
    }

    public void EnemyDied()
    {
        currentKills++;

        if (currentKills >= enemiesInScene)
        {
            Debug.Log("All enemies defeated!");
            OnKillAllEnemies?.Invoke();
        }
    }

    public void ResetTracker()
    {
        currentKills = 0;
        enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
}
