using System;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] GameObject healthIcon;
    [SerializeField] RectTransform healthIconHolder;
    int playerHealth = 3; // Will be replaced with the actual player health

    GameObject[] icons;

    void Start()
    {
        icons = new GameObject[playerHealth]; // Initialize the array

        for (int i = 0; i < playerHealth; i++)
        {
            GameObject icon = Instantiate(healthIcon, healthIconHolder);
            icons[i] = icon; // Store reference in the array
        }
    }

    private void Player_OnHealthChange(object sender, EventArgs e)
    {
        UpdateHealthUI(Player_v2.Instance.PlayerData.maxHealth);
    }

    public void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            if (i < currentHealth)
                icons[i].SetActive(true); // Show icon if within health range
            else
                icons[i].SetActive(false); // Hide icon if outside health range
        }
    }
}
