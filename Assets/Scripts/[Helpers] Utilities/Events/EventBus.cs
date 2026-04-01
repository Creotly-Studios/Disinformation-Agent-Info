using UnityEngine.Events;

public static class EventBus
{
    public static readonly SaveEvents Save = new();
    public static readonly QuestEvents Quest = new();
    public static readonly TaskListEvents TaskList = new();
    public static readonly GameplayEvents Gameplay = new();
    public static readonly NotificationEvents Notification = new();
    public static readonly CharacterStatEvents CharacterStat = new();

    public class SaveEvents
    {
        public UnityAction OnHandleAutoSave;
        public UnityAction OnDisplaySaveMenu;
        public UnityAction<bool> OnSetSceneAutoSave;
        public UnityAction<ISaveable> OnRegisterSaveableAsset;
    }

    public class TaskListEvents
    {
        public UnityAction<QuestSO> OnRefreshTaskList;
        public UnityAction<QuestObjective> OnUpdateTaskListValues;
    }

    public class QuestEvents
    {
        // Fired by QuestManager when the active quest changes.
        public UnityAction<QuestSO> OnActiveQuestChanged;

        // Fired by QuestSO when an objective is marked done.
        // QuestManager subscribes to chain "Completed → Next Objective" notifications.
        public UnityAction<QuestObjective> OnObjectiveCompleted;

        // Fired by QuestSO to signal nav system to refilter identifiers.
        public UnityAction<QuestObjective> OnNavigationRefreshNeeded;

        // General progress event consumed by QuestManager → QuestSO.
        public UnityAction<bool, bool, ObjectiveType, QuestObjectiveNavIdentifier>
                                             OnQuestObjectiveCompleted;
    }

    public class GameplayEvents
    {
        public UnityAction OnGameCompleted;
        public UnityAction OnCoinCollected;
    }

    public class CharacterStatEvents
    {
        // Fired by NPC_CharacterProfile when trust falls to or below 25.
        // DialogueManager subscribes and handles consequences.
        public UnityAction OnPlayerTrustLost;
    }

    public class NotificationEvents
    {
        // Both events carry the target NoticePopup so multiple panels can coexist
        // without cross-triggering. Each NoticePopup filters by object reference.
        public UnityAction<NoticePopup, NotificationRequest> OnShow;
        public UnityAction<NoticePopup> OnDismiss;
    }
}
