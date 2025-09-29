using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    //States
    private PatrolState currentState;

    //InBuilt Components
    public NavMeshPath navMeshPath;
    public Animator animator { get; private set; }
    public NavMeshAgent navMeshAgent {  get; private set; }
    public CharacterController characterController { get; private set; }

    //Added Components
    public NPCFunctions npcFunctions { get; private set; }
    public DialogueTrigger dialogueTrigger { get; private set; }
    public QuestObjectiveNavIdentifier Identifier { get; private set; }

    [Header("Profile")]
    [field: SerializeField] public BarSliderUI SliderUI { get; private set; }
    [field: SerializeField] public NPC_CharacterProfile Profile { get; private set; } = new();

    [Header("NPC Details")]
    public bool hasCompletedDialogue;
    [field: SerializeField] public NPCType TypeOfNPC { get; private set; } = NPCType.Generic;

    [Header("NPC Parameters")]
    public bool canMove = true;
    [field: SerializeField] public PatrolState NPCState { get; private set; }

    [Header("Target Private Parameters")]
    public float targetAngle;
    public float targetDistance;
    public Vector3 targetPosition;
    public Vector3 targetDirection;

    //Status
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool canRotate;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool performingAction;

    [Header("For NPC Emote")]
    [SerializeField] public Emotions emotion;
    [SerializeField] public NPC_Emote emote;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        dialogueTrigger = GetComponent<DialogueTrigger>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();

        npcFunctions = GetComponent<NPCFunctions>();
        Identifier = GetComponent<QuestObjectiveNavIdentifier>();
    }

    private void Start()
    {
        NPCState = Instantiate(NPCState);
        if(SliderUI != null)
        {
            Profile.InitializeAcceptanceValue(50.0f, SliderUI);
        }

        if (navMeshAgent == null)
        {
            GameObject aiObject = new();
            aiObject.transform.SetParent(transform);

            navMeshAgent = aiObject.AddComponent<NavMeshAgent>();
            navMeshAgent.stoppingDistance = 1.0f;
        }
        navMeshAgent.enabled = false;
        currentState = NPCState.RobotState_Update(this);
    }

    private void Update()
    {
        SetAnimatorBool();
        float delta = Time.deltaTime;
        
        HandleStateChange();
        npcFunctions.NPCFunctions_Update(delta);
        if (emote != null) emote.SetCurrentEmotion(emotion); //move to start during production build
    }

    private void HandleStateChange()
    {
        if (DialogueManager.Instance.dialogueIsPlaying || canMove != true)
        {
            isMoving = false;
            return;
        }

        if (currentState != null)
        {
            var nextState = NPCState.RobotState_Update(this);
            if (nextState != null)
            {
                currentState = nextState;
            }
        }
        navMeshAgent.transform.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        CheckIfMoving();
    }

    private void CheckIfMoving()
    { 
        if (navMeshAgent.enabled == false)
        {
            isMoving = false;
            return;
        }
        isMoving = SetMoving();
    }

    public void SetAnimatorBool()
    {
        animator.SetBool(AnimatorHashing.movingHash, isMoving);

        canRotate = animator.GetBool(AnimatorHashing.canRotateHash);
        performingAction = animator.GetBool(AnimatorHashing.isPerformingActionHash);
    }

    public void SetPersonalTargetDetails(Vector3 targetPosition)
    {
        targetDirection = (transform.position - targetPosition);

        targetDistance = targetDirection.magnitude;
        targetDirection = targetDirection.normalized;
        targetAngle = Maths_PhysicsHelper.CalculateViewAngle(transform.forward, targetDirection);
    }

    private bool SetMoving()
    {
        if (targetDistance > navMeshAgent.stoppingDistance)
        {
            return true;
        }

        if (NPCState.patrolMode == PatrolMode.Walk)
        {
            return true;
        }
        return false;
    }
}
