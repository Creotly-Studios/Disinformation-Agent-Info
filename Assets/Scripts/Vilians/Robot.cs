using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Robot : MonoBehaviour
{
    //In Built
    public NavMeshPath navMeshPath;
    public Animator animator { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public CharacterController characterController { get; private set; }

    //Created Components
    public RobotCombat robotCombat { get; private set; }
    public RobotMemory robotMemory { get; private set; }
    public RobotMovement robotMovement { get; private set; }
    public RobotAnimation robotAnimation { get; private set; }
    public RobotStatistics robotStatistics { get; private set; }
    public EnemyDetectionScript enemyDetectionScript { get; private set; }

    //Target Private Parameters
    public float AngleOfTarget { get; private set; }
    public float DistanceToTarget { get; private set; }
    public Vector3 DirectionToTarget { get; private set; }

    //Status
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool dontMove;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool canRotate;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool inAttackRange;
    [HideInInspector] public bool performingAction;
    [HideInInspector] public bool isRotatingWithRootMotion;

    //Combat Status
    public bool isStunned;
    public bool isRetreating;

    [Header("Target Properties")]
    public VisualTarget target;
    public Player_v2 currentVisualTarget { get; private set; }
    public List<VisualTarget> potentialTargets = new List<VisualTarget>();

    [field: Header("Player UI")]
    [field: SerializeField] public BarSliderUI healthBarUI { get; private set; }

    [Header("Finite State Machine")]
    public RobotStates currentState;
    public RobotStates_Idle idleState;
    public RobotStates_Pursue pursueState;
    public RobotStates_Combat combatState;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();

        robotCombat = GetComponent<RobotCombat>();
        robotMemory = GetComponent<RobotMemory>();
        robotMovement = GetComponent<RobotMovement>();
        robotAnimation = GetComponent<RobotAnimation>();
        robotStatistics = GetComponent<RobotStatistics>();
        enemyDetectionScript = GetComponent<EnemyDetectionScript>();
    }

    private void Start()
    {
        agent.enabled = false;
        robotStatistics.ResetUI();

        idleState = Instantiate(idleState);
        pursueState = Instantiate(pursueState);
        combatState = Instantiate(combatState);

        EnemyCombatControllerScript.Instance.AddEnemy(this);
        currentState = idleState.SwitchState(idleState, this);
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        SetAnimatorBools();
        float delta = Time.deltaTime;

        robotMovement.RobotMovement_Update(delta);

        GetTarget();
        HandleStateChange();
        robotCombat.RobotCombat_Updater(delta, this);
    }

    //Functionalities

    private void GetTarget()
    {
        robotMemory.RobotMemory_Update();
        SetCurrentTargetDetails();
    }

    public void SetCurrentTarget(Player_v2 target)
    {
        currentVisualTarget = target;
    }

    private void SetAnimatorBools()
    {
        canRotate = animator.GetBool(AnimatorHashing.canRotateHash);
        performingAction = animator.GetBool(AnimatorHashing.isPerformingActionHash);
        isRotatingWithRootMotion = animator.GetBool(AnimatorHashing.rootMotionRotateHash);

        animator.SetBool(AnimatorHashing.movingHash, isMoving);
    }

    private void HandleStateChange()
    {
        if (DialogueManager.Instance.dialogueIsPlaying || dontMove == true)
        {
            isMoving = false;
            agent.enabled = false;
            return;
        }

        if (currentState != null)
        {
            var nextState = currentState.RobotState_Update(this);
            if (nextState != null)
            {
                currentState = nextState;
            }
        }
        agent.transform.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        CheckIfMoving();
    }

    private void CheckIfMoving()
    {
        if (currentState == combatState)
        {
            return;
        }

        if (agent.enabled == false)
        {
            isMoving = false;
            return;
        }
        isMoving = SetMoving();
    }

    private bool SetMoving()
    {
        if (DistanceToTarget > agent.stoppingDistance)
        {
            return true;
        }

        if (currentState == idleState)
        {
            if (idleState.patrolMode == PatrolMode.Walk)
            {
                return true;
            }
        }
        return false;
    }

    private void SetCurrentTargetDetails()
    {
        if (currentVisualTarget == null)
        {
            return;
        }
        target.UpdateTargetInformation(transform);

        AngleOfTarget = target.TargetAngle;
        DistanceToTarget = target.TargetDistance;
    }

    public void SetPersonalTargetDetails(Vector3 targetPosition)
    {
        if (currentVisualTarget != null)
        {
            return;
        }
        DirectionToTarget = (transform.position - targetPosition);

        DistanceToTarget = DirectionToTarget.magnitude;
        AngleOfTarget = Maths_PhysicsHelper.CalculateViewAngle(transform.forward, DirectionToTarget);
    }
}
