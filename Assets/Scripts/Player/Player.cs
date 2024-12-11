using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    
    public PlayerData playerData;
    private int _currentHealth;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentHealth = playerData.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int dmg)
    {
        _currentHealth -= dmg;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player Dead");
        //Do gae=me over stuff here
    }

    public float GetPlayerHealth()
    {
        return _currentHealth;
    }
}
