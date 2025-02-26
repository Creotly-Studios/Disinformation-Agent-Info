using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("MAIN_STATS")] 
    public int maxHealth = 3;
    
    [Header("MOVEMENT")]
    public float speed = 5f;
    public float sprintSpeed = 8f;
    public float sprintDuration = 3f;
    public float sprintCooldown = 5f;
    public float sprintRechargeRate = 1f;
    public float gravity = -9.81f;
    [Space]
    public float jumpHeight = 2f;
    public float jumpForwardForce = 2f;
    public float turnSmoothTime = 0.1f;
    public float variableJumpHieghtMultiplier = 0.5f;
    [Space]
    public float dashForce = 5f;
    public float dashStaminaCost; // Add this field

    [Header("INTERACTION")] 
    public float detectRadius = 2;
    public float detectRange = 2;
    [Space] public float pushForce = 5f;

    [Header("ATTACKING/COMBOS")]
    public float attackRange = 3f;
    public float attackSphereSize = 2f;
    public int attackDamage = 1;
    [Space]
    public List<PunchSO> attackArray;
    [Space]
    public float timeBetweenCombos = 0.2f;
    public float timeBetweenAttackUsage = 0.2f;
    [Space]
    public float timeBetweenAttacks = 1f;
    
}
