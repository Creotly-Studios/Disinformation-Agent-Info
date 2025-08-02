using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class ComputerPanel_UI : MonoBehaviour
{
    private bool hasInitalized;
    [SerializeField] private SocialMediaComputer smComputer;

    private GamePanels activePanel;
    private QuestObjectives objective;
    private WaitForSeconds secondsDelay;

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
    [SerializeField] private Button continuePlaying_Btn, stopPlaying_Btn;
    [field: SerializeField] public NoticePopup popupPanel { get; private set; }
    [field: SerializeField] public GameObject objectiveCompletePanel { get; private set; }

    [Space]
    [SerializeField] private UnityEvent onExitComputer;
    
    private void OnEnable()
    {
        if(hasInitalized == true)
        {
            return;
        }

        hasInitalized = true;
        secondsDelay = new WaitForSeconds(1.2f);
        stopPlaying_Btn.onClick.AddListener(() => DisplayPanel_Objective(true));
        continuePlaying_Btn.onClick.AddListener(() => DisplayPanel_Objective(false));

        biasBingo_Btn.onClick.AddListener(() => DisplayPanel(biasBingoPanel));
        infoMatch_Btn.onClick.AddListener(() => DisplayPanel(infoMatchPanel));
        spotSource_Btn.onClick.AddListener(() => DisplayPanel(spotTheSourcePanel));

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
        biasBingo_Btn.onClick.RemoveListener(() => DisplayPanel(biasBingoPanel));
        infoMatch_Btn.onClick.RemoveListener(() => DisplayPanel(infoMatchPanel));
        spotSource_Btn.onClick.RemoveListener(() => DisplayPanel(spotTheSourcePanel));
    }

    public void DisablePanels()
    {
        mainMenuPanel.SetActive(true);

        biasBingoPanel.gameObject.SetActive(false);
        infoMatchPanel.gameObject.SetActive(false);
        spotTheSourcePanel.gameObject.SetActive(false);
        popupPanel.gameObject.SetActive(false);
    }

    private void DisplayPanel(GamePanels panel)
    {
        activePanel = panel;
        panel.gameObject.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    private void DisplayPanel_Objective(bool status)
    {
        if(status == true)
        {
            mainMenuPanel.SetActive(true);
            if (activePanel != null) { activePanel.gameObject.SetActive(false); }
            activePanel = null;
        }
        objectiveCompletePanel.SetActive(false);
    }

    private void Start()
    {
        DisablePanels();
    }

    private void Update()
    {
        UnlockGames();
        if(activePanel != null)
        {
            activePanel.GamePanel_Update();
        }
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

        objective = questManager.activeQuest.GetMiniGameObjetive();
        if(objective == null)
        {
            SetMiniGame();
            return;
        } 
        SetMiniGame(objective.objectiveType);
        smComputer.identifier.SetObjectiveType(objective.objectiveType);
    }

    public IEnumerator DisplayObjectiveCompletedPopup()
    {
        yield return secondsDelay;
        objectiveCompletePanel.SetActive(true);
    }

    private void SetMiniGame()
    {
        biasBingo_Btn.interactable = true;
        infoMatch_Btn.interactable = true;
        spotSource_Btn.interactable = true;
    }

    private void SetMiniGame(ObjectiveType type)
    {
        biasBingo_Btn.interactable = (type == ObjectiveType.MiniGame_BiasBingo);
        infoMatch_Btn.interactable = (type == ObjectiveType.MiniGame_MalignInfluence);
        spotSource_Btn.interactable = (type == ObjectiveType.MiniGame_SpotTheSource);
    }
}
