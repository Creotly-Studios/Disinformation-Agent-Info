using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ComputerPanel_UI : MonoBehaviour
{
    private bool hasInitalized;
    private QuestObjectives objective;

    [Header("User Buttons")]
    [SerializeField] private Button biasBingo_Btn;
    [SerializeField] private Button infoMatch_Btn;
    [SerializeField] private Button spotSource_Btn;
    [Space]
    [SerializeField] private Button exitButton;

    [Header("User Interface")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private MissionCodeUI missionCodeUI;
    [SerializeField] private BiasBingoPanel biasBingoPanel;
    [SerializeField] private MisinformationPanel infoMatchPanel;
    [SerializeField] public SpotTheSourcePanel spotTheSourcePanel;

    [field: Header("Popup Panels")]
    [field: SerializeField] public NoticePopup popupPanel { get; private set; }

    [Space]
    [SerializeField] private UnityEvent onExitComputer;
    
    private void OnEnable()
    {
        if(hasInitalized == true)
        {
            return;
        }

        hasInitalized = true;
        biasBingo_Btn.onClick.AddListener(() => DisplayPanel(biasBingoPanel.gameObject));
        infoMatch_Btn.onClick.AddListener(() => DisplayPanel(infoMatchPanel.gameObject));
        spotSource_Btn.onClick.AddListener(() => DisplayPanel(spotTheSourcePanel.gameObject));
        exitButton.onClick.AddListener(() =>
        {
            onExitComputer?.Invoke();
        });
    }

    private void OnDisable()
    {
        if(hasInitalized != true)
        {
            return;
        }

        hasInitalized = false;
        biasBingo_Btn.onClick.RemoveListener(() => DisplayPanel(biasBingoPanel.gameObject));
        infoMatch_Btn.onClick.RemoveListener(() => DisplayPanel(infoMatchPanel.gameObject));
        spotSource_Btn.onClick.RemoveListener(() => DisplayPanel(spotTheSourcePanel.gameObject));
    }

    public void DisablePanels()
    {
        mainMenuPanel.SetActive(true);

        biasBingoPanel.gameObject.SetActive(false);
        infoMatchPanel.gameObject.SetActive(false);
        spotTheSourcePanel.gameObject.SetActive(false);
        popupPanel.gameObject.SetActive(false);
    }

    private void DisplayPanel(GameObject panel)
    {
        panel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    private void Start()
    {
        DisablePanels();
    }

    private void Update()
    {
        UnlockGames();

        biasBingoPanel.BiasBingPanel_Update();
        spotTheSourcePanel.SpotSource_Update();
        infoMatchPanel.Misinformation_Update();

        UpdateMissionQuestLevel();
    }

    private void UpdateMissionQuestLevel()
    {
        QuestManager questManager = QuestManager.Instance;

        if(objective != null)
        {
            missionCodeUI.SetParameters(objective.isDone, questManager.activeQuest);
        }
    }

    private void UnlockGames()
    {
        QuestManager questManager = QuestManager.Instance;
        if (questManager == null)
        {
            return;
        }
        
        QuestSO activeQuest = questManager.activeQuest;
        if (activeQuest == null)
        {
            SetMiniGame();
            return;
        }

        objective = questManager.GetObjective();
        if(objective == null)
        {
            SetMiniGame();
            return;
        }
        SetMiniGame(objective.objectiveType);
    }

    private void SetMiniGame()
    {
        biasBingo_Btn.interactable = true;
        infoMatch_Btn.interactable = true;
        spotSource_Btn.interactable = true;
    }

    private void SetMiniGame(ObjectiveType type)
    {
        biasBingo_Btn.interactable = (type == ObjectiveType.BiasBingo);
        infoMatch_Btn.interactable = (type == ObjectiveType.MisInfoGames);
        spotSource_Btn.interactable = (type == ObjectiveType.SpotTheSource);
    }
}
