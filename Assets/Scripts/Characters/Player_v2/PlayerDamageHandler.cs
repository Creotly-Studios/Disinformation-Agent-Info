using UnityEngine;

// Owns health tracking and damage processing for the player.
// Mirrors PlayerDamageHandler from the reference architecture.
// Replaces the health/damage portion of PlayerStatistics.
//
// Sprint and dash stamina remain in PlayerLocomotionManager,
// which is the appropriate owner since they gate locomotion abilities.
[RequireComponent(typeof(Player_v2))]
public class PlayerDamageHandler : MonoBehaviour, IDamagable
{
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }

    private Player_v2 player;

    private void Awake() => player = GetComponent<Player_v2>();

    // ── Called from Player_v2.Start ───────────────────────────────────────────

    public void Initialize()
    {
        MaxHealth = player.PlayerData.maxHealth;
        CurrentHealth = MaxHealth;
    }

    // ── IDamagable ────────────────────────────────────────────────────────────

    public void TakeDamage(int damage)
    {
        if (player.isDead) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        player.CallPlayerDamage();
        PlayHurtSound();

        if (CurrentHealth <= 0)
            player.CallPlayerDeath();
    }

    // ── Save / Load ───────────────────────────────────────────────────────────

    public void SetCurrentHealth(int value) =>
        CurrentHealth = Mathf.Clamp(value, 0, MaxHealth);

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void PlayHurtSound()
    {
        int index = Random.Range(0, player.PlayerData.hurt.Length);
        AudioManager.Instance.PlaySFX(player.PlayerData.hurt[index]);
    }
}