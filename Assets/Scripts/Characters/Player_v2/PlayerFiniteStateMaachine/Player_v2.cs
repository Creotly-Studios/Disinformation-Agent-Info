using System;
using UnityEngine;
using UnityEngine.UI;

public class Player_v2 : MonoBehaviour
{
    public static Player_v2 Instance { get; private set; }
    public PlayerCombat playerCombat { get; private set; }
    public CharacterController controller { get; private set; }
    public PlayerNavigationSystem PlayerNav { get; private set; }

    #region Components
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerCombatSystem CombatSystem { get; private set; }
    public Animator Anim;
    #endregion

    [field: SerializeField] public Button PauseButton { get; private set; }

    #region Events
    public event EventHandler<GameObject> OnInteractObjectFind;
    public event EventHandler OnCollectCoin;
    public event EventHandler OnPlayerDie;
    public event EventHandler OnPlayerDamage;


    #endregion

    #region PlayerStates

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerInteractState InteractState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }

    public PlayerInactiveState InactiveState { get; private set; }

    public PlayerDeadState DeadState { get; private set; }

    #endregion

    #region PlayerFunction Components

    public PlayerStatistics PlayerStatistics { get; private set; }

    [Header("Player Info")]
    [SerializeField] private Sprite characterImage;
    [SerializeField] private DialogueCharacterInformation speakerInfo;
    [field: SerializeField] public PlayerData PlayerData { get; private set; }

    [Header("Player UI")]
    [field: SerializeField] public Image sprintUIBar { get; private set; }
    #endregion

    #region Checks
    public Transform checkTransform;
    public GameObject dialogue_InactiveCamera;
    #endregion

    [Header("Status")]
    public bool sprintFlag;
    public bool isAttacking;
    public bool performingAction;


    //testing new shit
    public Vector3 WorkSpace { get; set; }
    public Vector3 CurrentVelocity { get; private set; }

    #region UnityCallbackFunctions
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player (Instance) in the scene");
        }
        Instance = this;
        Anim = GetComponentInChildren<Animator>();

        StateMachine = new PlayerStateMachine();
        controller = GetComponent<CharacterController>();
        InputHandler = GetComponent<PlayerInputHandler>();
        CombatSystem = GetComponent<PlayerCombatSystem>();

        playerCombat = GetComponent<PlayerCombat>();
        PlayerNav = GetComponent<PlayerNavigationSystem>();
        PlayerStatistics = GetComponent<PlayerStatistics>();

        IdleState = new PlayerIdleState(this, StateMachine, PlayerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, PlayerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, PlayerData, "jump");
        LandState = new PlayerLandState(this, StateMachine, PlayerData, "move");
        DashState = new PlayerDashState(this, StateMachine, PlayerData, "dash");
        InAirState = new PlayerInAirState(this, StateMachine, PlayerData, "inAir");

        InteractState = new PlayerInteractState(this, StateMachine, PlayerData, "interact");
        AttackState = new PlayerAttackState(this, StateMachine, PlayerData, "attack");
        InactiveState = new PlayerInactiveState(this, StateMachine, PlayerData, "idle");
        DeadState = new PlayerDeadState(this, StateMachine, PlayerData, "dead");
    }

    void Start()
    {
        //initialize the state machine
        StateMachine.Initialize(IdleState);
        speakerInfo = Instantiate(speakerInfo);
        speakerInfo.Initialize("Agent Kim", characterImage, TypeOfSpeaker.Player, EmotionState.Neutral);

        PlayerStatistics.ResetUI();
        if (DialogueManager.Instance != null) { DialogueManager.Instance.SetPlayerSpeaker(speakerInfo); }
        if (GameManager.Instance != null) { PauseButton.onClick.AddListener(GameManager.Instance.TogglePause); }

        dialogue_InactiveCamera.SetActive(false);
    }

    void Update()
    {
        if (IsPlayerDead())
        {
            return;
        }
        float delta = Time.deltaTime;

        playerCombat.PlayerCombat_Updater(this);
        StateMachine.CurrentState.LogicUpdate();
        PlayerStatistics.PlayerStatistic_Update(delta);

        CurrentVelocity = controller.velocity;
        ApplyGravity();
    }

    private void FixedUpdate()
    {
        if (IsPlayerDead())
        {
            return;
        }

        StateMachine.CurrentState.PhysicsUpdate();
    }

    #endregion

    #region Check Functions
    public bool IsGrounded() => controller.isGrounded;

    public bool CanUseMovementInput()
    {
        return StateMachine.CurrentState != InactiveState && !GameManager.Instance.IsGamePaused();
    }
    public GameObject GetInteractableObject()
    {
        RaycastHit[] hits = Physics.SphereCastAll(checkTransform.position, PlayerData.detectRadius, checkTransform.forward, PlayerData.detectRange);
        foreach (RaycastHit hit in hits)
        {
            GameObject inter = hit.collider.gameObject;
            IInteractable interactable = inter.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Vector3 directionToEnemy = (hit.collider.transform.position - checkTransform.position).normalized;
                float dotProduct = Vector3.Dot(checkTransform.forward, directionToEnemy);

                if (dotProduct > 0.5f) // Adjust threshold to control front-facing precision
                {
                    return inter;
                }
            }
        }
        return null;
    }

    public bool IsPlayerDead()
    {
        return StateMachine.CurrentState == DeadState;
    }
    public bool IsPlayerAttacking()
    {
        return StateMachine.CurrentState == AttackState;
    }

    #endregion

    #region Other Functions

    public void Move(Vector3 velocity)
    {
        controller.Move(velocity);
    }

    public void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    public void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    public float _verticalVelocity;
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

    public bool HasInteractableObject { get; private set; }
    public void InvokeIInteractableFoundEvent()
    {
        if (GetInteractableObject() != null)
        {
            //event
            HasInteractableObject = true;
            OnInteractObjectFind?.Invoke(this, GetInteractableObject());

        }
        else
        {
            //event
            HasInteractableObject = false;
            OnInteractObjectFind?.Invoke(this, GetInteractableObject());
        }
    }

    public void SetInactiveState() => StateMachine.ChangeState(InactiveState);

    public void SetActiveState() => StateMachine.ChangeState(IdleState);

    #endregion

    #region PlayerEvents

    public void CallPlayerDeath()
    {
        StateMachine.ChangeState(DeadState);
        GameManager.Instance.PlayerDie();
        OnPlayerDie?.Invoke(this, EventArgs.Empty);
    }

    public void CallPlayerDamage()
    {
        OnPlayerDamage?.Invoke(this, EventArgs.Empty);
    }

    public void CallPlayerCoinPickup()
    {
        GameManager.Instance.PlayerCoinAdd();
        AudioManager.Instance.PlaySFX(PlayerData.coinPickup[0]);
        OnCollectCoin?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    public void DisplayPauseButton(bool status)
    {
        PauseButton.gameObject.SetActive(status);
    }

    void OnDestroy()
    {
        Anim = null;
    }

}