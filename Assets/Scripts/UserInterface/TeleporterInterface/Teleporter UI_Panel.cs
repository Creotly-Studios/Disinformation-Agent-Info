using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class TeleporterUI_Panel : MonoBehaviour
{
    private Teleporter teleporter;
    private WaitForSeconds noticeDuration;

    [Header("UI")]
    [SerializeField] private Button submitButton;
    [SerializeField] private Button closePanelButton;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private GameObject teleporterUiPanel;

    [Header("Inline Wrong-Code Notice")]
    [SerializeField] private GameObject noticePanel;
    [SerializeField] private float noticeDurationSeconds;
    [SerializeField] private TextMeshProUGUI noticeTextUI;

    [Header("Notification")]
    [SerializeField] private NoticePopup teleporterPopup;
    [SerializeField] private UnityEvent onCancelTeleport;

    private void Awake() => teleporter = GetComponent<Teleporter>();

    private void Start()
    {
        teleporterUiPanel.SetActive(false);
        noticeDuration = new WaitForSeconds(noticeDurationSeconds);

        submitButton.onClick.AddListener(OnSubmitButtonClick);
        closePanelButton.onClick.AddListener(() =>
        {
            teleporterUiPanel.SetActive(false);
            onCancelTeleport?.Invoke();
        });
        teleporterPopup.SubscribeEvents();
    }

    private void OnDestroy() => teleporterPopup.UnSubscribeEvents();

    private void OnSubmitButtonClick()
    {
        QuestManager questManager = QuestManager.Instance;
        QuestSO activeQuest = questManager.ActiveQuest;

        if(CheckIfCanTeleport(activeQuest, out string message) != true)
        {
            codeInputField.text = string.Empty;
            StartCoroutine(FlashNotice(message));
            return;
        }

        if (activeQuest == null)
        {
            QuestSO target = questManager.AvailableQuests.Find(x => x.QuestCode.ToString() == codeInputField.text);
            if (target == null)
            { 
                ShowInlineNotice(wrongCode: true);
                return;
            }
            Teleport(target);
            return;
        }

        if (activeQuest.QuestCode.ToString() != codeInputField.text)
        {
            ShowInlineNotice(wrongCode: false);
            return;
        }
        EventBus.Notification.OnShow?.Invoke(teleporterPopup,
            NotificationRequest.Payment(3, "Pay 3 Coins To Teleport", () => Teleport(activeQuest)));
    }

    private void Teleport(QuestSO quest)
    {
        codeInputField.text = string.Empty;
        teleporterUiPanel.SetActive(false);
        teleporter.identifier.SetActive(false);
        LevelLoader.LoadLevel(quest.QuestLevelIndex);
    }

    private bool CheckIfCanTeleport(QuestSO quest, out string message)
    {
        bool canTeleport = quest.QuestObjectives[0].isDone;
        message = "Get Instructions From Agency Boss Before Teleportation";
        if (canTeleport)
        {
            message = "Complete Mini Game Objective before Teleportation";
            canTeleport = quest.GetMiniGameObjetive().isDone;
        }
        return canTeleport;
    }

    private void ShowInlineNotice(bool wrongCode)
    {
        codeInputField.text = string.Empty;
        string text = wrongCode ? "Incorrect Code — Please Try Again" : "Cannot Play This Level Yet";
        StartCoroutine(FlashNotice(text));
    }

    private IEnumerator FlashNotice(string text)
    {
        noticeTextUI.text = text;
        noticePanel.SetActive(true);
        yield return noticeDuration;
        noticePanel.SetActive(false);
    }
}
