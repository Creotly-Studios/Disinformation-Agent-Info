using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyCombatControllerScript : MonoBehaviour
{
    int index;
    public static EnemyCombatControllerScript Instance { get; private set; }

    private Coroutine enemyLoopRoutine;

    [Header("Robot Teams")]
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
}
