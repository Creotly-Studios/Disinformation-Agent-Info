using UnityEngine;

public class PlayerStatistics : MonoBehaviour, IDamagable
{
    Player_v2 player;

    [Header("Max Parameters")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxEndurance = 100f;

    [Header("Endurance Regenerator")]
    private bool canRegenerate;
    [SerializeField] private float enduranceTickTimer = 0f;
    [SerializeField] private float enduranceMultiplier = 2.0f;
    [SerializeField] private float enduranceRegenerateTimer = 0f;

    public float CurrentHealth { get; private set; } = 0f;
    public float CurrentEndurance { get; private set; } = 0f;

    private void Awake()
    {
        player = GetComponent<Player_v2>();
    }

    public void ResetUI()
    {
        CurrentHealth = maxHealth;
        CurrentEndurance = maxEndurance;
        
        player.enduranceBarUI.SetMaxValue(maxEndurance);
        
        player.enduranceBarUI.SetCurrentValue(CurrentEndurance);
    }

    public void TakeDamage(float healthDamage, int damageAnimation)
    {
        CurrentHealth -= healthDamage;
        if (CurrentHealth <= 0.0f)
        {
            HandleDeath();
            return;
        }
    }

    private void HandleDeath()
    {
        CurrentHealth = 0.0f;
        player.isDead = true;
        GameManager.instance.PlayerDie();
    }

    public void PlayerStatistic_Update(float delta)
    {
        RegenerateEndurance(delta);
        player.enduranceBarUI.SetCurrentValue(CurrentEndurance);
    }

    private void RegenerateEndurance(float delta)
    {
        if(player.sprintFlag || player.performingAction)
        {
            return;
        }

        if (CurrentEndurance < maxEndurance)
        {
            canRegenerate = true;
        }
        else if (CurrentEndurance >= maxEndurance)
        {
            CurrentEndurance = maxEndurance;
            canRegenerate = false;
        }

        if (canRegenerate == true)
        {
            enduranceRegenerateTimer += delta;
            if (enduranceRegenerateTimer >= 2f)
            {
                enduranceTickTimer += delta;

                if (enduranceTickTimer >= 0.1f)
                {
                    enduranceTickTimer = 0f;
                    CurrentEndurance += Mathf.RoundToInt(enduranceMultiplier);
                }
            }
            canRegenerate = false;
        }
        else if (canRegenerate != true)
        {
            enduranceRegenerateTimer = 0f;
        }
    }

    public void ReduceEndurancePeriodically(float floatToReduceBy, float delta)
    {
        CurrentEndurance -= floatToReduceBy * delta;
    }
}
