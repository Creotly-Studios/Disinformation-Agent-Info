using UnityEngine;
using UnityEngine.AI;

public class R_Robot : MonoBehaviour, IDamagable
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;

    [Header("Animation")]
    public Animator animator;
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Punch = Animator.StringToHash("Punch");

    [Header("Patroling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float moveSpeed = 2f;

    [Header("Combat")]
    public float timeBetweenAttacks = 1.5f;
    public float punchDamage = 20f;
    public float punchRadius = 1f;
    public Transform punchPoint;
    bool alreadyAttacked;
    
    [Header("Detection")]
    public float sightRange = 10f;
    public float attackRange = 2f;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = moveSpeed;
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        UpdateState();
    }

    private void UpdateState()
    {
        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }

        animator.SetBool(IsWalking, agent.velocity.magnitude > 0.1f);
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        Vector3 lookPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookPosition);

        if (!alreadyAttacked)
        {
            animator.SetTrigger(Punch);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public void OnPunchHit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(punchPoint.position, punchRadius, whatIsPlayer);
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<Player>(out Player playerHealth))
            {
                playerHealth.Damage();
                
                if (hitCollider.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    Vector3 impactDirection = (hitCollider.transform.position - transform.position).normalized;
                    rb.AddForce(impactDirection * 10f, ForceMode.Impulse);
                }
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float healthDamage, int damageAnimation)
    {
        health -= healthDamage;
        SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.sfxList.enemyHitEffect);
        if (health <= 0)
        {
            KillTracker.Instance.AddKill();
            SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.sfxList.enemyDieEffect);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        
        if (punchPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(punchPoint.position, punchRadius);
        }
    }
}