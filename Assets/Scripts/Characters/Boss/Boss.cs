using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Boss : MonoBehaviour
{
    public Transform Player { get; protected set; }
    public LayerMask whatIsGround, whatIsPlayer;
    public int maxhealth;
    [SerializeField] private float detectRange;
    [SerializeField] private float attackRange;

    [Header("Enemy Settings")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;

    private int currentHealth;
    bool isDead;

    [Header("Checks")]
    public Transform attackPoint;

    //boss fight
    public BossState state;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    protected void Start()
    {
        SetBossState(BossState.State_one);
        Player = Player_v2.Instance.gameObject.transform;
        currentHealth = maxhealth;
        isDead = false;
    }

    void Update()
    {
        HandleStateMachine();
    }


    public void TakeDamage(int healthDamage)
    {
        if(isDead) return;
        
        currentHealth -= healthDamage;
        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        if (!isDead)
        {
            PlayDeadAnim();
            isDead = true;
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool PlayerInSightRange()
    {
        return Physics.CheckSphere(transform.position, detectRange, whatIsPlayer);
    }

    public bool PlayerInAttackRange()
    {
        return Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
    }

    protected void OnDrawGizmosSelected()
    {
        // Draw detection range in yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // Draw attack range in red
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    //handling animations
    public void PlayIdleAnim()
    {
        if (animator)
        {
            animator.SetBool("idle", true);
            animator.SetBool("isWalking", false);

        }
    }


    public void PlayAttackAnim()
    {
        if (animator)
        {
            animator.SetBool("isWalking", false);
            animator.SetTrigger("attack");

        }
    }

    public void PlayDeadAnim()
    {
        animator.SetBool("dead", true);
    }

    void HandleStateMachine()
    {
        switch (state)
        {
            case BossState.State_one:
            //roaming and shooting smaller enemies at the player
            break;

            case BossState.State_two:
            //actually attacking the player and shooting projectiles a them
            break;

            case BossState.Dead:
            //check if all smaller enemies are dead, if true, game manager mission complete
            break;
        }
    }

    void SetBossState(BossState _)
    {
        state = _;
    }

}
public enum BossState {State_one, State_two, Dead}
