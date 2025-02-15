using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamagable
{
    public Transform Player {get; private set;}
    public LayerMask whatIsGround, whatIsPlayer;
    public int currentHealth;

    [Header("Enemy Settings")]
    public EnemyData e_data;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;

    [Header("Checks")]
    public Transform attackPoint;

    void Start()
    {
        Player = Player_v2.Instance.gameObject.transform;
        if (GetComponent<NavMeshAgent>() != null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (GetComponent<Animator>() != null)
        {
            animator = GetComponent<Animator>();
        }
        currentHealth = e_data.maxhealth;
    }

    void Update()
    {
        
    }

    public void TakeDamage(int healthDamage)
    {
        currentHealth -= healthDamage;
        if(currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }

    public bool PlayerInSightRange()
    {
        return Physics.CheckSphere(transform.position, e_data.detectRange, whatIsPlayer);
    }

    public bool PlayerInAttackRange()
    {
        return Physics.CheckSphere(transform.position, e_data.attackRange, whatIsPlayer);
    }







}
