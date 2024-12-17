using UnityEngine;
using Unity.Cinemachine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    //Unity Components
    public Animator animator { get; private set; }
    public CharacterController CharacterController { get; private set; }

    //Created Components
    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerAnimation PlayerAnimation { get; private set; }
    public PlayerStatistics PlayerStatistics { get; private set; }
    public PlayerInteraction PlayerInteraction { get; private set; }
    [field: SerializeField] public PlayerData PlayerData { get; private set; }

    [Header("Player UI")]
    [field: SerializeField]
    public BarSliderUI healthBarUI { get; private set; }

    [field: SerializeField] public BarSliderUI enduranceBarUI { get; private set; }

    [Header("Player Info")]
    [SerializeField] private Sprite characterImage;
    [SerializeField] private DialogueCharacterInformation speakerInfo;

    //Status
    public bool isDead;
    public bool sprintFlag;
    public bool isAttacking;
    public bool performingAction;

    [Space] [SerializeField] public CinemachineImpulseSource cameraImpulseSource;

private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }   
        PlayerData = Instantiate(PlayerData);

        animator = GetComponentInChildren<Animator>();
        CharacterController = GetComponent<CharacterController>();

        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerAnimation = GetComponent<PlayerAnimation>();
        PlayerStatistics = GetComponent<PlayerStatistics>();
        PlayerInteraction = GetComponent<PlayerInteraction>();
    }

    private void Start()
    {
        speakerInfo = Instantiate(speakerInfo);
        
        PlayerStatistics.ResetUI();
        DialogueManager.Instance.SetPlayerSpeaker(speakerInfo);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }
        float delta = Time.deltaTime;

        HandleStatusChanges();
        PlayerMovement.PlayerMovement_Update(delta);
        PlayerStatistics.PlayerStatistic_Update(delta);
    }

    private void LateUpdate()
    {
        if (isDead)
        {
            return;
        }
        float delta = Time.deltaTime;
        performingAction = animator.GetBool(AnimatorHashing.isPerformingActionHash);
    }

    //Functionalities
    private void HandleStatusChanges()
    {
        //Sprint
        sprintFlag = (InputManager.instance.sprintPressed);
    }
}
