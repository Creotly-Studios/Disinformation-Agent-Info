using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class TeleporterUI_Panel : MonoBehaviour
{
    private Teleporter teleporter;
    private WaitForSeconds waitForSeconds;
    
    [Header("UI")] 
    [SerializeField] private Button submitButton;
    [SerializeField] private Button closePanelButton;
    [SerializeField] private TMP_InputField codeInputField;
    [Space] [SerializeField] private GameObject teleporterUiPanel;

    [Header("Notice Panel")]
    [SerializeField] private GameObject noticePanel;
    [SerializeField] private float panelDisableSecond;
    [SerializeField] private TextMeshProUGUI noticeTextUI;

    [Space]
    [SerializeField] private UnityEvent onCancelTeleport;

    private void Awake()
    {
        teleporter = GetComponent<Teleporter>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        teleporterUiPanel.SetActive(false);
        submitButton.onClick.AddListener(() =>
        {
            OnSubmitButtonClick();
        });
        closePanelButton.onClick.AddListener(() =>
        {
            teleporterUiPanel.SetActive(false);
            onCancelTeleport?.Invoke();
        });

        waitForSeconds = new WaitForSeconds(panelDisableSecond);
    }
    
    private void OnSubmitButtonClick()
    {
        QuestManager questManager = QuestManager.Instance;
        QuestSO quest = questManager.activeQuest;

        //If Completed All Quests Player Can Teleport To Any Scene
        if (quest == null)
        {
            QuestSO teleportQuest = questManager.availableQuests.Find(x => x.questCode.ToString() == codeInputField.text);
            if (teleportQuest == null)
            {
                InCorrectCode(true);
                return;
            }
            Teleport(teleportQuest);
            return;
        }

        if (quest.questCode.ToString() != codeInputField.text)
        {
            InCorrectCode(false);
            return;
        }
        Teleport(quest);
    }

    private void Teleport(QuestSO quest)
    {
        codeInputField.text = "";
        teleporterUiPanel.SetActive(false);
        teleporter.identifier.SetActive(false);
        LevelLoader.LoadLevel(quest.questLevelIndex);
    }

    private void InCorrectCode(bool completedQuest)
    {
        codeInputField.text = "";
        string noticeText = (completedQuest) ? "In-Correct Code, Please Try Again" : "Cannot Play Level At The Time";
        StartCoroutine(DisableNoticePanel(noticeText));
    }

    private IEnumerator DisableNoticePanel(string text)
    {
        noticeTextUI.text = text;
        noticePanel.SetActive(true);

        yield return waitForSeconds;
        noticePanel.SetActive(false);
    }
}
