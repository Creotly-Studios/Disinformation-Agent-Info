using System;
using UnityEngine;

public class Player_v2 : MonoBehaviour, ISaveable
{
    public static Player_v2 Instance { get; private set; }

    public PlayerAnimationManager Animation { get; private set; }
    public PlayerLocomotionManager Locomotion { get; private set; }
    public PlayerCombatController Combat { get; private set; }
    public PlayerDamageHandler Damage { get; private set; }
    public PlayerNavigationSystem PlayerNav { get; private set; }

    public Animator Anim { get; private set; }
    public CharacterController Controller { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerInactiveState InactiveState { get; private set; }

    public PlayerLandState LandState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }

    [Header("Ability States")]
    [SerializeField] private Normal_AbilityState normalState;
    [SerializeField] private Combat_AbilityState combatState;
    [SerializeField] private Dashing_AbilityState dashingState;
    [SerializeField] private Jumping_AbilityState jumpingState;

    public Normal_AbilityState Normal { get; private set; }
    public Combat_AbilityState CombatState { get; private set; }
    public Dashing_AbilityState Dashing { get; private set; }
    public Jumping_AbilityState Jumping { get; private set; }

    [field: SerializeField] public AbilityState CurrentAbilityState { get; private set; }

    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool JustJumped;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool performingAction;
    [field: SerializeField] public PlayerData PlayerData { get; private set; }

    public event EventHandler<GameObject> OnInteractObjectFind;
    public event EventHandler OnCollectCoin;
    public event EventHandler OnPlayerDie;
    public event EventHandler OnPlayerDamage;

    // ── Inspector References ──────────────────────────────────────────────────
    [Header("References")]
    public Transform checkTransform;
    public GameObject dialogue_InactiveCamera;

    [Header("UI")]
    [field: SerializeField] public UnityEngine.UI.Image SprintUIBar { get; private set; }

    [Header("VFX")]
    [SerializeField] private ParticleSystem dashVFX;
    [SerializeField] private ParticleSystem attackVFX;

    [Header("Dialogue")]
    [SerializeField] private Sprite characterImage;
    [SerializeField] private DialogueCharacterInformation speakerInfo;

    private CharacterSaveData saveData;
    private RaycastHit[] detectionBuffer;

    public bool IsPlayerDead() => isDead;
    public ObjectSaveData GetSaveData() => saveData;

    public void PlayDashEffect() => dashVFX.Play();  
    public void PlayAttackEffect() => attackVFX.Play();
    public void SetActiveState() => StateMachine.ChangeState(IdleState);
    public void SetInactiveState() => StateMachine.ChangeState(InactiveState);

    public void CallPlayerDamage() => OnPlayerDamage?.Invoke(this, EventArgs.Empty);
    public void InvokeInteractableFoundEvent() => OnInteractObjectFind?.Invoke(this, GetInteractableObject());
    public bool CanUseMovementInput() => StateMachine.CurrentState != InactiveState && !GameManager.Instance.IsGamePaused();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Anim = GetComponentInChildren<Animator>();
        Controller = GetComponent<CharacterController>();
        InputHandler = GetComponent<PlayerInputHandler>();
        PlayerNav = GetComponent<PlayerNavigationSystem>();

        Damage = GetComponent<PlayerDamageHandler>();
        Combat = GetComponent<PlayerCombatController>();
        Animation = GetComponent<PlayerAnimationManager>();
        Locomotion = GetComponent<PlayerLocomotionManager>();

        StateMachine = new();
        BuildLocomotionStates();
    }

    private void Start()
    {
        detectionBuffer = new RaycastHit[20];
        saveData = new CharacterSaveData { name = name };
        
        Normal = Instantiate(normalState);
        Dashing = Instantiate(dashingState);
        Jumping = Instantiate(jumpingState);
        CombatState = Instantiate(combatState);
        EventBus.Save.OnRegisterSaveableAsset?.Invoke(this);

        StateMachine.Initialize(IdleState);
        CurrentAbilityState = Normal;

        Normal.OnEnter(this);
        Damage.Initialize();

        speakerInfo = Instantiate(speakerInfo);
        speakerInfo.Initialize("Agent Kim", characterImage, TypeOfSpeaker.Player);
        if (DialogueManager.Instance != null) DialogueManager.Instance.SetPlayerSpeaker(speakerInfo);
        dialogue_InactiveCamera.SetActive(false);
    }

    private void Update()
    {
        if (isDead) return;
        float delta = Time.deltaTime;
        Animation.Animation_Update();
        StateMachine.CurrentState.LogicUpdate();

        CurrentAbilityState.AbilityStateUpdater(this);
        Locomotion.Locomotion_Update(delta);
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        StateMachine.CurrentState.PhysicsUpdate();
    }

    private void OnDestroy() => Anim = null;

    // ── Locomotion State Construction ─────────────────────────────────────────

    private void BuildLocomotionStates()
    {
        IdleState = new PlayerIdleState(this, StateMachine, PlayerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, PlayerData, "move");
        LandState = new PlayerLandState(this, StateMachine, PlayerData);
        DeadState = new PlayerDeadState(this, StateMachine, PlayerData, "dead");
        InAirState = new PlayerInAirState(this, StateMachine, PlayerData, "inAir");
        InactiveState = new PlayerInactiveState(this, StateMachine, PlayerData, "idle");
    }

    public void SwitchAbilityState(AbilityState nextState)
    {
        CurrentAbilityState = CurrentAbilityState.SwitchState(nextState, this);
    }

    public GameObject GetInteractableObject()
    {
        int count = Physics.SphereCastNonAlloc(
            checkTransform.position, PlayerData.detectRadius,
            checkTransform.forward, detectionBuffer, PlayerData.detectRange);

        for (int i = 0; i < count; i++)
        {
            if (detectionBuffer[i].collider == null) continue;
            if (!detectionBuffer[i].collider.gameObject.TryGetComponent<IInteractable>(out _)) continue;

            Vector3 dir = (detectionBuffer[i].collider.transform.position - checkTransform.position).normalized;
            if (Vector3.Dot(checkTransform.forward, dir) > 0.5f)
                return detectionBuffer[i].collider.gameObject;
        }
        return null;
    }

    public void CallPlayerDeath()
    {
        isDead = true;
        StateMachine.ChangeState(DeadState);
        GameManager.Instance.PlayerDie();
        OnPlayerDie?.Invoke(this, EventArgs.Empty);
    }

    public void CallPlayerCoinPickup()
    {
        GameManager.Instance.PlayerCoinAdd();
        AudioManager.Instance.PlaySFX(PlayerData.coinPickup[0]);
        OnCollectCoin?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateSavedData()
    {
        saveData.UpdateSaveData(GameManager.Instance.PlayerCoinAmount, Damage.CurrentHealth,transform.position, transform.rotation);
    }

    public void ReloadDataFromSavedFile(ObjectSaveData data)
    {
        if (data is not CharacterSaveData c) return;
        GameManager.Instance.SetCoinAmount(c.coinAmount);
        Damage.SetCurrentHealth(c.healthCount);
        transform.SetPositionAndRotation(c.ObjectPosition, c.ObjectRotation);
    }

    private void OnDrawGizmos()
    {
        if (checkTransform == null || PlayerData == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkTransform.position, PlayerData.attackSphereSize);
        Gizmos.DrawLine(checkTransform.position,
            checkTransform.position + checkTransform.forward * PlayerData.attackRange);
        Gizmos.DrawWireSphere(
            checkTransform.position + checkTransform.forward * PlayerData.attackRange,
            PlayerData.attackSphereSize);
    }
}