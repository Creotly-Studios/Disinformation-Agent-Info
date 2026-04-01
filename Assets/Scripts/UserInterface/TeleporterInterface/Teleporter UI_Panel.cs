using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

// The inline wrong-code notice (noticePanel) is intentionally kept as a local
// text overlay — it's a brief, button-free flash that doesn't need the full
// NoticePopup system. The payment confirmation uses the central popup.
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
    }

    // ── Submit Handler ────────────────────────────────────────────────────────

    private void OnSubmitButtonClick()
    {
        QuestManager questManager = QuestManager.Instance;
        QuestSO activeQuest = questManager.ActiveQuest;

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

    // ── Teleport ──────────────────────────────────────────────────────────────

    private void Teleport(QuestSO quest)
    {
        codeInputField.text = string.Empty;
        teleporterUiPanel.SetActive(false);
        teleporter.identifier.SetActive(false);
        LevelLoader.LoadLevel(quest.QuestLevelIndex);
    }

    // ── Inline Notice (no buttons, auto-hides) ────────────────────────────────

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
