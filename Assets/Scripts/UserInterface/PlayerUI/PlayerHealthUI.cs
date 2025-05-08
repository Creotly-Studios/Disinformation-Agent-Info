using System;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] GameObject healthIcon;
    [SerializeField] RectTransform healthIconHolder;
    int playerHealth; // Will be replaced with the actual player health
    public int iconAmount = 3;

    GameObject[] icons;

    private void Awake() {
    }

    void Start()
    {
        Player_v2.Instance.OnPlayerDamage += Player_OnHealthChange;
        playerHealth = Player_v2.Instance.PlayerData.maxHealth; // Start at max health
        icons = new GameObject[Player_v2.Instance.PlayerData.maxHealth]; // Initialize the array

        for (int i = 0; i < Player_v2.Instance.PlayerData.maxHealth; i++)
        {
            GameObject icon = Instantiate(healthIcon, healthIconHolder);
            icons[i] = icon; // Store reference
        }

        UpdateHealthUI(playerHealth);
    }

    private void Player_OnHealthChange(object sender, EventArgs e)
    {
        TakeDamage();
    }

    public void UpdateHealthUI(int currentHealth)
    {
        if (icons == null)
        {
            return;
        }

        if (currentHealth < 0 || currentHealth > Player_v2.Instance.PlayerData.maxHealth)
        {
            return;
        }

        for (int i = 0; i < icons.Length; i++)
        {
            if (icons[i] == null)
            {
                continue;
            }
            icons[i].SetActive(i < currentHealth); // Show/Hide icons based on health
        }
    }

     public void TakeDamage()
    {
        playerHealth = Mathf.Max(0, playerHealth - 1);
        UpdateHealthUI(playerHealth);
    }

    public void Heal()
    {
        playerHealth = Mathf.Min(Player_v2.Instance.PlayerData.maxHealth, playerHealth + 1);
        UpdateHealthUI(playerHealth);
    }
}
