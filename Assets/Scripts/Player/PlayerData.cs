using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("MAIN_STATS")] 
    public int maxHealth = 10;
    
    [Header("MOVEMENT")]
    public float speed = 5f;
    public float sprintSpeed = 8f;
    public float sprintDuration = 3f;
    public float sprintCooldown = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float jumpForwardForce = 2f;
    public float turnSmoothTime = 0.1f;

    [Header("INTERACTION")] 
    public float detectRadius = 2;
    public float detectRange = 2;
    
    
}
