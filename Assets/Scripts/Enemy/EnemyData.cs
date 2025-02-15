using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int maxhealth = 3;

    [Header("Player Detection")]
    public float detectRange;
    public float attackRange;

    [Space]
    public int damage = 1;
    public GameObject deathEffect;
    [Space]

    [Header("Chaser Enemies")]
    public float moveSpeed;

    [Header("Shooter Enemies")]
    public GameObject projectile;
    public float projectileSpeed;
    public float shootRate;

    [Header("Melle Enemies")]
    public float damageRange;
    public float attackRate;
}
