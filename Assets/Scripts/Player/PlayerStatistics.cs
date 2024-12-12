using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    Player player;

    [SerializeField] private float maxEndurance = 100f;

    [Header("Endurance Regenerator")]
    [SerializeField] private float enduranceTickTimer = 0f;
    [SerializeField] private float enduranceMultiplier = 2.25f;
    [SerializeField] private float enduranceRegenerateTimer = 0f;
    
    public float CurrentEndurance { get; private set; } = 0f;

    private void Awake()
    {
        player = GetComponent<Player>();
        CurrentEndurance = maxEndurance;
    }

    //Can Add UI To Display

    public void RegenerateEndurance(float delta)
    {
        if (player.isSprinting || player.performingAction)
        {
            return;
        }

        if(CurrentEndurance >= maxEndurance)
        {
            CurrentEndurance = maxEndurance;
            return;
        }

        enduranceRegenerateTimer += Time.deltaTime;
        if (enduranceRegenerateTimer >= 2f)
        {
            CurrentEndurance += delta * enduranceMultiplier;
        }
    }

    public void ReduceEndurancePeriodically(float floatToReduceBy, float delta)
    {
        CurrentEndurance -= floatToReduceBy * delta;
    }
}
