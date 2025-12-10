using UnityEngine;
using System.Collections.Generic;

public class SceneStatusManager : MonoBehaviour
{
    private bool hasSet;
    private List<Enemy> activeEnemies;

    [SerializeField] private bool canAutoSave;
    public List<int> KilledEnemies { get; private set; } = new();

    private void OnEnable()
    {
        if(hasSet)
        {
            return;
        }

        SaveManagerSystem saveManager = SaveManagerSystem.Instance;
        activeEnemies = new(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
        if(saveManager != null)
        {
            saveManager.SetAutoSaveBool(canAutoSave, this);
        }
        hasSet = true;
    }

    private void OnDisable()
    {
        if(hasSet != true)
        {
            return;
        }
        hasSet = false;
    }

    public void AddKilledEnemy(Enemy enemy)
    {
        KilledEnemies.Add(activeEnemies.IndexOf(enemy));
    }

    public void ReloadEnemies()
    {
        activeEnemies = new(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
        for(int i = 0; i < activeEnemies.Count; i++)
        {
            if(KilledEnemies.Contains(i))
            {
                activeEnemies[i].HandleReloadDeath();
            }
        }
    }
}