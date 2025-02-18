using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyCombatControllerScript : MonoBehaviour
{
    int index;
    public static EnemyCombatControllerScript Instance { get; private set; }

    private Coroutine enemyLoopRoutine;

    [Header("Robot Teams")]
    private List<Enemy> enemyList = new List<Enemy>();
    private List<int> robotIndexList = new List<int>();

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Multiple Instances");
            Destroy(gameObject);
        }
        Instance = this;
    }

    public Enemy RandomGameObject(GameObject exclude)
    {
        for(int i = 0; i < enemyList.Count; i++)
        {
            if(exclude == enemyList[i].gameObject)
            {
                continue;
            }
            robotIndexList.Add(i);
        }

        if(robotIndexList.Count <= 0)
        {
            return null;
        }

        int random = Random.Range(0, robotIndexList.Count);
        int index = robotIndexList[random];
        return enemyList[index];
    }
}
