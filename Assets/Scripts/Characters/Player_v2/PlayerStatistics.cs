using System;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour, IDamagable
{
    Player_v2 player;
    public float CurrentMoveSpeed {get; private set;}
    public float CurrentSprintTime {get; private set;}
    private float lastSprintTime = -Mathf.Infinity;
    private float SprintTimeNormalized;

    public int CurrentHealth { get; private set; } = 0;

    public event EventHandler OnPlayerDamage; 

    private void Awake()
    {
        player = GetComponent<Player_v2>();
        CurrentHealth = player.PlayerData.maxHealth;
        CurrentSprintTime = player.PlayerData.sprintDuration;
    }

    public void ResetUI()
    {
        CurrentHealth = player.PlayerData.maxHealth;
    }

    

    public void TakeDamage(int healthDamage)
    {
        player.CallPlayerDamage();
        PlayHurtSound();
        CurrentHealth -= healthDamage;
        if (CurrentHealth <= 0)
        {
            HandleDeath();
            return;
        }
    }

    public void PlayerDie()
    {
        CurrentHealth = 0;
        player.CallPlayerDeath();
    }

    private void HandleDeath()
    {
        CurrentHealth = 0;
        // player.isDead = true;
        player.CallPlayerDeath();

    }

    public void SetCurrentHealth(int value)
    {
        CurrentHealth = value;
    }

    public void PlayerStatistic_Update(float delta)
    {
        HandleSprint();
        SprintTimeNormalized = (float)CurrentSprintTime / player.PlayerData.sprintDuration;
        player.sprintUIBar.fillAmount = SprintTimeNormalized;
    }

    private void HandleSprint()
    {
        bool sprint = player.InputHandler.SprintInput;
        if (sprint && CurrentSprintTime > 0f && Time.time - lastSprintTime > player.PlayerData.sprintCooldown)
        {
            CurrentMoveSpeed = player.PlayerData.sprintSpeed;
            CurrentSprintTime -= Time.deltaTime;
            player.Anim.SetFloat("moveVel", 1f); 
        }
        else
        {
            CurrentMoveSpeed = player.PlayerData.speed;
            player.Anim.SetFloat("moveVel", 0f);

            // Recover sprint stamina when not sprinting
            if (CurrentSprintTime < player.PlayerData.sprintDuration)
            {
                CurrentSprintTime += Time.deltaTime * player.PlayerData.sprintRechargeRate;
            }
        }

        // Store last sprint time if player runs out of stamina
        if (CurrentSprintTime <= 0f)
        {
            lastSprintTime = Time.time;
        }
    }


    public bool CanDash()
    {
        // Check if the player has enough stamina to dash
        return CurrentSprintTime >= player.PlayerData.dashStaminaCost;
    }

    public void UseDash()
    {
        // Reduce stamina when dashing
        CurrentSprintTime -= player.PlayerData.dashStaminaCost;
        if (CurrentSprintTime < 0)
        {
            CurrentSprintTime = 0;
        }
    }


    private void PlayHurtSound()
    {
        // Alternate between footstep sounds for variety
        int index = UnityEngine.Random.Range(0, player.PlayerData.hurt.Length);
        AudioManager.Instance.PlaySFX(player.PlayerData.hurt[index]);
    }

}
