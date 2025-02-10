using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ComputerPanel_UI : MonoBehaviour
{
    private bool hasInitalized;

    [Header("User Buttons")]
    [SerializeField] private Button biasBingo_Btn;
    [SerializeField] private Button infoMatch_Btn;
    [SerializeField] private Button spotSource_Btn;
    [Space]
    [SerializeField] private Button exitButton;
    
    [Header("Popup Panels")]
    [SerializeField] private NoticePopup popupPanel;

    [Header("User Interface")]
    [SerializeField] private GameObject mainMenuPanel;
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
    }

    private void UnlockGames()
    {
        if (QuestManager.Instance == null) return;
         
        QuestSO activeQuest = QuestManager.Instance.activeQuest;

        if(activeQuest == null)
        {
            return;
        }

        if(activeQuest.currentObjective.objectiveType == ObjectiveType.BiasBingo)
        {
            biasBingo_Btn.interactable = true;
            infoMatch_Btn.interactable = false;
            spotSource_Btn.interactable = false;
            return;
        }
        else if(activeQuest.currentObjective.objectiveType == ObjectiveType.SpotTheSource)
        {
            spotSource_Btn.interactable = true;
            biasBingo_Btn.interactable = false;
            infoMatch_Btn.interactable = false;
            return;
        }
        infoMatch_Btn.interactable = true;
        biasBingo_Btn.interactable = false;
        spotSource_Btn.interactable = false;
    }
}
