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

    private const float BannerChainDelay = 2.5f;
    private const float FirstObjectDelay = 3.5f;

    private void Awake()
    {
        if (Instance != null)
        { 
            Destroy(gameObject); 
            return;
        }

        Instance = this;
        questPopup.SubscribeEvents();
        for (int i = 0; i < AvailableQuests.Count; i++)
        {
            AvailableQuests[i] = Instantiate(AvailableQuests[i]);
        }
        EventBus.Quest.OnQuestObjectiveCompleted += UpdateQuestData;
        EventBus.Gameplay.OnNewSceneLoaded += InformSceneOfQuestChange;
        EventBus.Quest.OnObjectiveCompletedVisuals += OnObjectiveCompleted;
        InitialAssignment();
    }

    private void OnDestroy()
    {
        questPopup.UnSubscribeEvents();
        EventBus.Quest.OnQuestObjectiveCompleted -= UpdateQuestData;
        EventBus.Gameplay.OnNewSceneLoaded -= InformSceneOfQuestChange;
        EventBus.Quest.OnObjectiveCompletedVisuals -= OnObjectiveCompleted;
    }

    private void InitialAssignment()
    {
        QuestSO quest = AvailableQuests.Find(x => !x.isComplete);
        if (quest == null)
        { 
            EventBus.Gameplay.OnGameCompleted?.Invoke();
            return;
        }
        AssignActiveQuest(false, quest);
        activeQuestIndex = AvailableQuests.IndexOf(quest);
    }

    private void InformSceneOfQuestChange(bool shouldTransitionScreen)
    {
        EventBus.Quest.OnActiveQuestChanged?.Invoke(shouldTransitionScreen, ActiveQuest);
        EventBus.TaskList.OnRefreshTaskList?.Invoke(ActiveQuest);
    }

    private void AssignActiveQuest(bool shouldTransitionScreen, QuestSO quest)
    {
        ActiveQuest = quest;
        InformSceneOfQuestChange(shouldTransitionScreen);

        Show(NotificationRequest.QuestBanner(1.0f, ActiveQuest));
        StartCoroutine(ShowFirstObjectiveAfterBanner(ActiveQuest));
    }

    private void UpdateQuestData(bool increase, bool multiple, ObjectiveType type, QuestObjectiveNavIdentifier identifier)
    {
        ActiveQuest.UpdateQuestObjectiveLevels(increase, multiple, type, identifier);
        if (!ActiveQuest.isComplete)
        {
            return;
        }

        activeQuestIndex++;
        if (activeQuestIndex >= AvailableQuests.Count)
        {
            EventBus.Gameplay.OnGameCompleted?.Invoke();
            return;
        }
        EventBus.Save.OnHandleAutoSave?.Invoke();
        Show(NotificationRequest.QuestBanner(0f, ActiveQuest));
        AssignActiveQuest(true, AvailableQuests[activeQuestIndex]);
    }


    private void OnObjectiveCompleted(QuestObjective completed) => StartCoroutine(ChainObjectiveBanners(completed));

    private IEnumerator ChainObjectiveBanners(QuestObjective completed)
    {
        Show(NotificationRequest.ObjectiveBanner(0f, completed));
        yield return new WaitForSeconds(BannerChainDelay);

        QuestObjective next = ActiveQuest.FindNextObjective();
        if (next != null)
        {
            Show(NotificationRequest.ObjectiveBanner(0f, next));
        }
    }

    private IEnumerator ShowFirstObjectiveAfterBanner(QuestSO quest)
    {
        yield return new WaitForSeconds(FirstObjectDelay);
        QuestObjective first = quest.FindNextObjective();
        if (first != null)
        {
            Show(NotificationRequest.ObjectiveBanner(0f, first));
        }
    }

    private void Show(NotificationRequest request) => EventBus.Notification.OnShow?.Invoke(questPopup, request);

    public void RestoreQuestProgress(List<SerializableQuestData> questDataList)
    {
        for (int i = 0; i < AvailableQuests.Count; i++)
        {
            if (i >= questDataList.Count)
            {
                break;
            }

            SerializableQuestData data = questDataList[i];
            if (AvailableQuests[i].questTitle.Equals(data.questName))
            {
                data.RestoreQuestValues(AvailableQuests[i]);
            }
        }
        InitialAssignment();
    }
}
