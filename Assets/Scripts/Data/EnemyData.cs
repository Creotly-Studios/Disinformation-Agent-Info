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
    public float destroyTime = 1f;
    [Space]

    [Header("Chaser Enemies")]
    public float moveSpeed = 3f;
    public float stopDistance = 2f;

    [Header("Shooter Enemies")]
    public GameObject projectile;
    public float projectileShelfLife = 5f;
    public float projectileSpeed = 5f;
    public float shootRate = 2.5f;

    [Header("Melle Enemies")]
    public float attackRate = 3f;
}
