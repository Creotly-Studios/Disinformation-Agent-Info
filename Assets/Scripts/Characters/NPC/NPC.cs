using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    //States
    private NPCStates currentState;
    private DialogueCharacterInformation charInfo;

    //InBuilt Components
    public NavMeshPath navMeshPath;
    public Animator animator { get; private set; }
    public NavMeshAgent navMeshAgent {  get; private set; }
    public CharacterController characterController { get; private set; }

    //Added Components
    public NPCFunctions npcFunctions { get; private set; }
    public BarSliderUI warmingUpRadarUI { get; private set; }

    [Header("Profile")]
    public CharacterProfile profile;

    [Header("NPC Details")]
    public bool canInteract;
    public bool haveDialogue;

    [Header("NPC Parameters")]
    public bool canMove = true;
    public float warmingUpRadar;
    public BoundaryFloat angleLimit;
    [field: SerializeField] public NPCStates npcState { get; private set; }
    public CharacterType characterType { get; private set; } = CharacterType.NPC;

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

    private void Awake()
    {
        profile.Initialize(this);
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();

        npcFunctions = GetComponent<NPCFunctions>();
        warmingUpRadarUI = GetComponentInChildren<BarSliderUI>();
    }

    private void Start()
    {
        npcState = Instantiate(npcState);
        charInfo = GetComponent<DialogueTrigger>().characterInformation;

        navMeshAgent.enabled = false;
        currentState = npcState.RobotState_Update(this);

        warmingUpRadar = 55;
        // warmingUpRadarUI.SetMaxValue(100);
        // warmingUpRadarUI.SetCurrentValue(warmingUpRadar);
    }

    private void Update()
    {
        SetAnimatorBool();
        float delta = Time.deltaTime;
        
        HandleStateChange();
        npcFunctions.NPCFunctions_Update(delta);
    }

    public void UpdateWarmRadar(Response response)
    {
        warmingUpRadar += response.Evaluate(profile);

        QuestObjectives objective = QuestManager.Instance.FindQuestObjective(ObjectiveType.ConvinceNPC);
        if (warmingUpRadar >= 75.0f && objective != null)
        {
            QuestSO quest = QuestManager.Instance.activeQuest;
            quest.IncreaseQuestObjectiveProgressLevels(objective);
        }
        ChangeEmotion();
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
            var nextState = npcState.RobotState_Update(this);
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

        if (npcState.patrolMode == PatrolMode.Walk)
        {
            return true;
        }
        return false;
    }

    private void ChangeEmotion()
    {
        if(warmingUpRadar < 36)
        {
            charInfo.SetEmotionState(EmotionState.Angry);
            return;
        }
        else if(warmingUpRadar >= 36 && warmingUpRadar < 66)
        {
            charInfo.SetEmotionState(EmotionState.Neutral);
            return;
        }
        charInfo.SetEmotionState(EmotionState.Calm);
    }
}
