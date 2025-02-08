using System;
using UnityEngine;

public class Player_v2 : MonoBehaviour
{
    public static Player_v2 Instance { get; private set; }
    public CharacterController controller { get; private set; }
    [SerializeField]
    private PlayerData playerData;
    public PlayerData PlayerData { get; private set; }

    #region Components
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerCombatSystem CombatSystem { get; private set; }
    public Animator Anim;
    #endregion

    #region Events
    public event EventHandler<GameObject> OnInteractObjectFind; 
    

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
    public PlayerDialogueState DialogueState { get; private set; }
    
    public PlayerInactiveState InactiveState { get; private set; }

    #endregion

    #region Checks
    public Transform checkTransform;
    #endregion
    
    #region UnityCallbackFunctions
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player (Instance) in the scene");
        }
        Instance = this;
        
        StateMachine = new PlayerStateMachine();
        PlayerData = playerData;

        IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "jump");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, playerData, "move");
        DashState = new PlayerDashState(this, StateMachine, playerData, "dash");
        InteractState = new PlayerInteractState(this, StateMachine, playerData, "interact");
        AttackState = new PlayerAttackState(this, StateMachine, playerData, "isAttacking");
        DialogueState = new PlayerDialogueState(this, StateMachine, playerData, "idle");
        InactiveState = new PlayerInactiveState(this, StateMachine, playerData, "idle");
    }

    void Start()
    {
        InputHandler = GetComponent<PlayerInputHandler>();
        CombatSystem = GetComponent<PlayerCombatSystem>();
        controller = GetComponent<CharacterController>();
        //initialize the state machine
        StateMachine.Initialize(IdleState);
    }

    void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
        InvokeIInteractableFoundEvent();
        OnInteractObjectFind?.Invoke(this, GetInteractableObject());
        Debug.Log(StateMachine.CurrentState);
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
        ApplyGravity();
    }

    #endregion

    #region Check Functions
    public bool IsGrounded() => controller.isGrounded;

    public GameObject GetInteractableObject()
    {
        RaycastHit[] hits = Physics.SphereCastAll(checkTransform.position, playerData.detectRadius, checkTransform.forward, playerData.detectRange);
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

    #endregion

    #region Other Functions

    public void Move(Vector3 velocity)
    {
        controller.Move(velocity);
    }

    public void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    public void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    private float _verticalVelocity;
    void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += playerData.gravity * Time.deltaTime;

            _verticalVelocity = Mathf.Max(_verticalVelocity, -53f); // Terminal velocity ~53 m/s
        }

        Move(new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
    }

    public void Jump()
    {
        if (controller.isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(playerData.jumpHeight * -2f * playerData.gravity);
        }
        Move(new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
    }

    public void DashForward()
    {
        if (controller.isGrounded)
        {

        }
        Move(transform.forward * playerData.dashForce);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb != null)
        {
            Vector3 forceDir = hit.gameObject.transform.position - transform.position;
            forceDir.y = 0;
            forceDir.Normalize();

            rb.AddForceAtPosition(forceDir * playerData.pushForce, transform.position, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        if (checkTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkTransform.position, playerData.attackSphereSize);
            Gizmos.DrawLine(checkTransform.position, checkTransform.position + checkTransform.forward * playerData.attackRange);
            Gizmos.DrawWireSphere(checkTransform.position + checkTransform.forward * playerData.attackRange, playerData.attackSphereSize);
        }
    }
    
    public bool hasInteractableObject { get; private set; }
    void InvokeIInteractableFoundEvent()
    {
        if (GetInteractableObject() != null)
        {
            //event
            hasInteractableObject = true;
            OnInteractObjectFind?.Invoke(this, GetInteractableObject());
            
        }
        else
        {
            //event
            hasInteractableObject = false;
            OnInteractObjectFind?.Invoke(this, GetInteractableObject());
        }
    }



    #endregion






}
