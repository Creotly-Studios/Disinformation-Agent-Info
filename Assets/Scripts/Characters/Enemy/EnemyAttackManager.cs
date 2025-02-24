using UnityEngine;
using System.Collections.Generic;

public class EnemyAttackManager : MonoBehaviour
{
    private static EnemyAttackManager instance;
    public static EnemyAttackManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EnemyAttackManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("EnemyAttackManager");
                    instance = go.AddComponent<EnemyAttackManager>();
                }
            }
            return instance;
        }
    }

    private Enemy currentlyAttackingEnemy;
    [SerializeField] private float globalAttackCooldown = 1f;
    private float lastAttackTime;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    public bool RequestAttackPermission(Enemy requestingEnemy)
    {
        // If enemy is dead, deny permission
        if (requestingEnemy.IsDead()) return false;

        // If no enemy is attacking or enough time has passed since the last attack
        if (currentlyAttackingEnemy == null || 
            Time.time - lastAttackTime >= globalAttackCooldown)
        {
            currentlyAttackingEnemy = requestingEnemy;
            lastAttackTime = Time.time;
            return true;
        }
        
        return false;
    }

    public void FinishAttack(Enemy enemy)
    {
        if (currentlyAttackingEnemy == enemy)
        {
            currentlyAttackingEnemy = null;
        }
    }

    public bool IsCurrentlyAttacking(Enemy enemy)
    {
        return currentlyAttackingEnemy == enemy;
    }
}