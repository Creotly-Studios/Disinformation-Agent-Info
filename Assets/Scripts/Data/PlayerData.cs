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
    public float coyoteTime = 0.5f;
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

    [Header("SFX")]
    public AudioClip jump;
    public AudioClip dash;
    public AudioClip land;
    public AudioClip interact;
    public AudioClip dead_GameOver;
    public AudioClip inactiveStateCamSound;
    [Space]
    public AudioClip attackHit;
    public AudioClip[] footsteps;
    public float footstepInterval = 0.35f;
    public AudioClip[] coinPickup;
    public AudioClip[] hurt;
    
}
