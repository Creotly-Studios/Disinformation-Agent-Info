using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class ComputerPanel_UI : MonoBehaviour
{
    private bool hasInitialized;
    private GamePanels activePanel;
    private QuestObjective currentMiniGameObjective;
    private WaitForSeconds objectivePanelDelay;

    [SerializeField] private SocialMediaComputer smComputer;

    [Header("Notification")]
    [field: SerializeField] public NoticePopup Popup { get; private set; }

    // Queried by GamePanels.GamePanel_Update() to pause the game timer.
    public bool IsPopupActive => _isPopupActive;
    private bool _isPopupActive;

    // Cached button actions — ensures RemoveListener matches the stored delegate.
    private UnityAction showBiasBingo;
    private UnityAction showInfoMatch;
    private UnityAction showSpotSource;
    private UnityAction onStopPlaying;
    private UnityAction onContinuePlaying;

    [Header("Navigation Buttons")]
    [SerializeField] private Button biasBingo_Btn;
    [SerializeField] private Button infoMatch_Btn;
    [SerializeField] private Button spotSource_Btn;
    [SerializeField] private Button exitButton;

    [Header("Sub-Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private MissionCodeUI missionCodeUI;
    [SerializeField] private BiasBingoPanel biasBingoPanel;
    [SerializeField] private MisinformationPanel infoMatchPanel;
    [field: SerializeField] public SpotTheSourcePanel SpotTheSourcePanel { get; private set; }

    [Header("Objective Complete")]
    [SerializeField] private Button continuePlaying_Btn;
    [SerializeField] private Button stopPlaying_Btn;
    [field: SerializeField] public GameObject ObjectiveCompletePanel { get; private set; }

    [SerializeField] private UnityEvent onExitComputer;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        showBiasBingo = () => DisplayPanel(biasBingoPanel);
        showInfoMatch = () => DisplayPanel(infoMatchPanel);
        showSpotSource = () => DisplayPanel(SpotTheSourcePanel);
        onStopPlaying = () => DisplayPanel_Objective(true);
        onContinuePlaying = () => DisplayPanel_Objective(false);
    }

    private void OnEnable()
    {
        if (hasInitialized)
        {
            return;
        }
        hasInitialized = true;
        objectivePanelDelay = new WaitForSeconds(1.2f);

        stopPlaying_Btn.onClick.AddListener(onStopPlaying);
        continuePlaying_Btn.onClick.AddListener(onContinuePlaying);
        biasBingo_Btn.onClick.AddListener(showBiasBingo);
        infoMatch_Btn.onClick.AddListener(showInfoMatch);
        spotSource_Btn.onClick.AddListener(showSpotSource);
        exitButton.onClick.AddListener(() => onExitComputer?.Invoke());

        EventBus.Quest.OnActiveQuestChanged += OnActiveQuestChanged;
        EventBus.Notification.OnShow += OnNotificationShown;
        EventBus.Notification.OnDismiss += OnNotificationDismissed;
        Popup.SubscribeEvents();
    }

    private void OnDisable()
    {
        if (!hasInitialized) return;
        hasInitialized = false;

        stopPlaying_Btn.onClick.RemoveListener(onStopPlaying);
        continuePlaying_Btn.onClick.RemoveListener(onContinuePlaying);
        biasBingo_Btn.onClick.RemoveListener(showBiasBingo);
        infoMatch_Btn.onClick.RemoveListener(showInfoMatch);
        spotSource_Btn.onClick.RemoveListener(showSpotSource);

        EventBus.Quest.OnActiveQuestChanged -= OnActiveQuestChanged;
        EventBus.Notification.OnShow -= OnNotificationShown;
        EventBus.Notification.OnDismiss -= OnNotificationDismissed;
        Popup.UnSubscribeEvents();
    }

    private void Start() => DisablePanels();

    // ── EventBus Handlers ─────────────────────────────────────────────────────

    private void OnActiveQuestChanged(bool _, QuestSO quest)
    {
        Debug.Log(quest);
        if (quest == null)
        { 
            UnlockAllMiniGames();
            return;
        }

        currentMiniGameObjective = quest.GetMiniGameObjetive();
        if (currentMiniGameObjective == null) 
        { 
            UnlockAllMiniGames();
            return;
        }
        SetMiniGameInteractability(currentMiniGameObjective.objectiveType);
        smComputer.identifier.SetObjectiveType(currentMiniGameObjective.objectiveType);
    }

    private void OnNotificationShown(NoticePopup popup, NotificationRequest _)
    {
        _isPopupActive = popup == Popup || _isPopupActive;
    }

    private void OnNotificationDismissed(NoticePopup popup)
    {
        if (popup == Popup) _isPopupActive = false;
    }

    // ── Update ────────────────────────────────────────────────────────────────

    private void Update()
    {
        if(activePanel == null)
        {
            return;
        }
        activePanel.GamePanel_Update();
    }

    // ── Panel Control ─────────────────────────────────────────────────────────

    public void DisablePanels()
    {
        EnableMainMenu();
        biasBingoPanel.gameObject.SetActive(false);
        infoMatchPanel.gameObject.SetActive(false);
        SpotTheSourcePanel.gameObject.SetActive(false);
    }

    private void EnableMainMenu()
    {
        if(currentMiniGameObjective != null)
        {
            missionCodeUI.SetParameters(currentMiniGameObjective.isDone);
        }   
        mainMenuPanel.SetActive(true);
    }

    private void DisplayPanel(GamePanels panel)
    {
        activePanel = panel;
        panel.gameObject.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    private void DisplayPanel_Objective(bool stopPlaying)
    {
        if (stopPlaying)
        {
            EnableMainMenu();
            if (activePanel != null)
            {
                activePanel.gameObject.SetActive(false);
                activePanel = null;
            }
        }
        ObjectiveCompletePanel.SetActive(false);
    }

    // ── Mini-Game Interactability ─────────────────────────────────────────────

    private void UnlockAllMiniGames()
    {
        biasBingo_Btn.interactable = true;
        infoMatch_Btn.interactable = true;
        spotSource_Btn.interactable = true;
    }

    private void SetMiniGameInteractability(ObjectiveType type)
    {
        biasBingo_Btn.interactable = type == ObjectiveType.MiniGame_BiasBingo;
        infoMatch_Btn.interactable = type == ObjectiveType.MiniGame_MalignInfluence;
        spotSource_Btn.interactable = type == ObjectiveType.MiniGame_SpotTheSource;
    }

    public IEnumerator DisplayObjectiveCompletedPopup()
    {
        yield return objectivePanelDelay;
        ObjectiveCompletePanel.SetActive(true);
    }
}
