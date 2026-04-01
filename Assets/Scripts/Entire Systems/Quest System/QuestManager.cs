using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private int activeQuestIndex;
    public QuestSO ActiveQuest { get; private set; }
    public int CurrentLevel => activeQuestIndex;

    [field: SerializeField] public List<QuestSO> AvailableQuests { get; private set; } = new();

    [Header("Notification")]
    [SerializeField] private NoticePopup questPopup;

    // Quest banner auto-dismisses after 2 s. Gap of 0.5 s before next banner = 2.5 s total.
    private const float BannerChainDelay = 2.5f;
    // New quest banner has a 1 s lead-in, then 2 s display, then 0.5 s gap before first objective.
    private const float FirstObjectDelay = 3.5f;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < AvailableQuests.Count; i++)
            AvailableQuests[i] = Instantiate(AvailableQuests[i]);

        EventBus.Quest.OnQuestObjectiveCompleted += UpdateQuestData;
        EventBus.Quest.OnObjectiveCompleted += OnObjectiveCompleted;
        InitialAssignment();
    }

    private void OnDestroy()
    {
        EventBus.Quest.OnQuestObjectiveCompleted -= UpdateQuestData;
        EventBus.Quest.OnObjectiveCompleted -= OnObjectiveCompleted;
    }

    // ── Quest Flow ────────────────────────────────────────────────────────────

    private void InitialAssignment()
    {
        QuestSO quest = AvailableQuests.Find(x => !x.isComplete);
        if (quest == null) { EventBus.Gameplay.OnGameCompleted?.Invoke(); return; }
        AssignActiveQuest(quest);
    }

    private void AssignActiveQuest(QuestSO quest)
    {
        ActiveQuest = quest;
        EventBus.Quest.OnActiveQuestChanged?.Invoke(ActiveQuest);
        EventBus.TaskList.OnRefreshTaskList?.Invoke(ActiveQuest);

        Show(NotificationRequest.QuestBanner(1.0f, ActiveQuest));
        StartCoroutine(ShowFirstObjectiveAfterBanner(ActiveQuest));
    }

    private void UpdateQuestData(bool increase, bool multiple,
        ObjectiveType type, QuestObjectiveNavIdentifier identifier)
    {
        ActiveQuest.UpdateQuestObjectiveLevels(increase, multiple, type, identifier);
        if (!ActiveQuest.isComplete) return;

        activeQuestIndex++;
        if (activeQuestIndex >= AvailableQuests.Count)
        {
            EventBus.Gameplay.OnGameCompleted?.Invoke();
            return;
        }

        EventBus.Save.OnHandleAutoSave?.Invoke();
        Show(NotificationRequest.QuestBanner(0f, ActiveQuest));
        AssignActiveQuest(AvailableQuests[activeQuestIndex]);
    }

    // ── Objective Notification Chain ──────────────────────────────────────────

    private void OnObjectiveCompleted(QuestObjective completed) =>
        StartCoroutine(ChainObjectiveBanners(completed));

    // Shows "Objective Completed", waits for auto-dismiss, then shows "New Objective".
    private IEnumerator ChainObjectiveBanners(QuestObjective completed)
    {
        Show(NotificationRequest.ObjectiveBanner(0f, completed));
        yield return new WaitForSeconds(BannerChainDelay);

        QuestObjective next = ActiveQuest.FindNextObjective();
        if (next != null) Show(NotificationRequest.ObjectiveBanner(0f, next));
    }

    // After the new quest banner has fully displayed, show the first objective.
    private IEnumerator ShowFirstObjectiveAfterBanner(QuestSO quest)
    {
        yield return new WaitForSeconds(FirstObjectDelay);
        QuestObjective first = quest.FindNextObjective();
        if (first != null) Show(NotificationRequest.ObjectiveBanner(0f, first));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void Show(NotificationRequest request) =>
        EventBus.Notification.OnShow?.Invoke(questPopup, request);

    // ── Save / Load ───────────────────────────────────────────────────────────

    public void RestoreQuestProgress(List<SerializableQuestData> questDataList)
    {
        for (int i = 0; i < AvailableQuests.Count; i++)
        {
            if (i >= questDataList.Count) break;
            SerializableQuestData data = questDataList[i];
            if (AvailableQuests[i].questTitle.Equals(data.questName))
                data.RestoreQuestValues(AvailableQuests[i]);
        }
    }
}
