using UnityEngine;
using UnityEngine.Events;

public class KillTracker : MonoBehaviour
{
    public static KillTracker Instance { get; private set; }

    [Header("Kill Settings")]
    [SerializeField] private int maxKills = 10;
    [SerializeField] private int currentKills = 0;

    [Header("Events")]
    public UnityEvent onMaxKillsReached;
    public UnityEvent<int> onKillCountChanged;  // Passes current kill count

    private void Awake()
    {
        Instance = this;
    }

    public void AddKill()
    {
        currentKills++;
        
        // Notify listeners of the new kill count
        onKillCountChanged?.Invoke(currentKills);

        // Check if max kills reached
        if (currentKills >= maxKills)
        {
            onMaxKillsReached?.Invoke();
        }
    }

    public int GetCurrentKills()
    {
        return currentKills;
    }

    public void ResetKills()
    {
        currentKills = 0;
        onKillCountChanged?.Invoke(currentKills);
    }
}