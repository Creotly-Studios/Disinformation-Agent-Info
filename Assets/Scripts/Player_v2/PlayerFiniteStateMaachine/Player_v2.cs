using UnityEngine;

public class Player_v2 : MonoBehaviour
{
    public CharacterController controller { get; private set; }

    #region Components
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerCombatSystem CombatSystem { get; private set; }
    public Animator Anim;
    #endregion

    #region PlayerStates

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerInteractState InteractState { get; private set; }
    public PlayerAttackState AtttackState { get; private set; }
    public PlayerDialogueState DialogueState { get; private set; }

    #endregion

    #region PlayerFunction Components
    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerAnimation PlayerAnimation { get; private set; }
    public PlayerStatistics PlayerStatistics { get; private set; }

    [Header("Player Info")]
    [SerializeField] private Sprite characterImage;
    [SerializeField] private DialogueCharacterInformation speakerInfo;
    [field: SerializeField] public PlayerData PlayerData { get; private set; }

    [Header("Player UI")]
    [field: SerializeField] public BarSliderUI healthBarUI { get; private set; }
    [field: SerializeField] public BarSliderUI enduranceBarUI { get; private set; }
    #endregion

    #region Checks
    public Transform checkTransform;
    #endregion

    //Status
    public bool isDead;
    public bool sprintFlag;
    public bool isAttacking;
    public bool performingAction;

    #region UnityCallbackFunctions
    private void Awake()
    {
        PlayerData = Instantiate(PlayerData);
        StateMachine = new PlayerStateMachine();

        controller = GetComponent<CharacterController>();
        InputHandler = GetComponent<PlayerInputHandler>();
        CombatSystem = GetComponent<PlayerCombatSystem>();

        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerAnimation = GetComponent<PlayerAnimation>();
        PlayerStatistics = GetComponent<PlayerStatistics>();

        IdleState = new PlayerIdleState(this, StateMachine, PlayerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, PlayerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, PlayerData, "jump");
        InAirState = new PlayerInAirState(this, StateMachine, PlayerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, PlayerData, "move");
        DashState = new PlayerDashState(this, StateMachine, PlayerData, "dash");
        InteractState = new PlayerInteractState(this, StateMachine, PlayerData, "interact");
        AtttackState = new PlayerAttackState(this, StateMachine, PlayerData, "isAttacking");
        DialogueState = new PlayerDialogueState(this, StateMachine, PlayerData, "idle");
    }

    void Start()
    {
        //initialize the state machine
        StateMachine.Initialize(IdleState);
        speakerInfo = Instantiate(speakerInfo);
        speakerInfo.Initialize("Agent Kim", characterImage, TypeOfSpeaker.Player, EmotionState.Neutral);

        PlayerStatistics.ResetUI();
        DialogueManager.Instance.SetPlayerSpeaker(speakerInfo);
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        float delta = Time.deltaTime;
        Debug.Log(StateMachine.CurrentState);

        StateMachine.CurrentState.LogicUpdate();
        PlayerStatistics.PlayerStatistic_Update(delta);
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        StateMachine.CurrentState.PhysicsUpdate();
        ApplyGravity();
    }

    #endregion

    #region Other Functions

    public void Move(Vector3 velocity)
    {
        controller.Move(velocity);
    }

    private float _verticalVelocity;
    public void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    public void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += PlayerData.gravity * Time.deltaTime;

            _verticalVelocity = Mathf.Max(_verticalVelocity, -53f); // Terminal velocity ~53 m/s
        }

        Move(new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
    }

    public void Jump()
    {
        if (controller.isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(PlayerData.jumpHeight * -2f * PlayerData.gravity);
        }
        Move(new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
    }

    public void DashForward()
    {
        if (controller.isGrounded)
        {

        }
        Move(transform.forward * PlayerData.dashForce);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb != null)
        {
            Vector3 forceDir = hit.gameObject.transform.position - transform.position;
            forceDir.y = 0;
            forceDir.Normalize();

            rb.AddForceAtPosition(forceDir * PlayerData.pushForce, transform.position, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        if (checkTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkTransform.position, PlayerData.attackSphereSize);
            Gizmos.DrawLine(checkTransform.position, checkTransform.position + checkTransform.forward * PlayerData.attackRange);
            Gizmos.DrawWireSphere(checkTransform.position + checkTransform.forward * PlayerData.attackRange, PlayerData.attackSphereSize);
        }
    }



    #endregion






}
