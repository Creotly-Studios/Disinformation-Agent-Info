using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestSO", menuName = "Scriptable Objects/QuestSO")]
public class QuestSO : ScriptableObject
{
    public int CompletedObjectives { get; private set; }

    [Header("Status")]
    [ReadOnly] public bool isComplete;
    [field: SerializeField] public int QuestCode { get; private set; }
    [field: SerializeField] public int QuestLevelIndex { get; private set; }

    [field: Header("Quest Information")]
    public string questTitle;
    [TextArea] public string description;
    [field: SerializeField] public int QuestReward { get; private set; }
    [field: SerializeField] public List<QuestObjective> QuestObjectives { get; private set; }

    public QuestObjective FindNextObjective() => QuestObjectives.Find(x => !x.isDone);
    public QuestObjective FindQuestObjective(ObjectiveType t) => FindQuestObjective(t, false);

    public void CheckIfQuestIsComplete()
    {
        isComplete = QuestObjectives.Find(x => !x.isDone) == null;
    }

    public QuestObjective GetMiniGameObjetive()
    {
        foreach (QuestObjective obj in QuestObjectives)
        {
            if (obj.objectiveType == ObjectiveType.MiniGame_MalignInfluence
             || obj.objectiveType == ObjectiveType.MiniGame_SpotTheSource
             || obj.objectiveType == ObjectiveType.MiniGame_BiasBingo)
                return obj;
        }
        return null;
    }

    // ── Objective Update Pipeline ─────────────────────────────────────────────

    public void UpdateQuestObjectiveLevels(bool increase, bool multiple, ObjectiveType type, QuestObjectiveNavIdentifier identifier)
    {
        QuestObjective objective = FindQuestObjective(type, multiple);
        if (increase)
        {
            IncreaseQuestObjectiveProgressLevels(objective, identifier);
            return;
        }
        DecreaseQuestObjectiveProgressLevels(objective, identifier);
    }

    public void IncreaseQuestObjectiveProgressLevels(QuestObjective objective, QuestObjectiveNavIdentifier identifier)
    {
        if (objective == null || objective.isDone)
        {
            return;
        }
        CompletedObjectives++;
        objective.progressValue++;
        SetCompletedObjective(objective, identifier);
    }

    private void DecreaseQuestObjectiveProgressLevels(QuestObjective objective, QuestObjectiveNavIdentifier identifier)
    {
        if (objective == null)
        {
            return;
        }
        CompletedObjectives--;
        objective.progressValue--;
        SetCompletedObjective(objective, identifier);
    }

    private void SetCompletedObjective(QuestObjective objective, QuestObjectiveNavIdentifier identifier)
    {
        EventBus.TaskList.OnUpdateTaskListValues?.Invoke(objective);
        objective.isDone = objective.progressValue >= objective.targetValue;

        if (!objective.isDone)
        {
            return;
        }

        CheckIfQuestIsComplete();
        if(identifier != null)
        {
            identifier.MarkCompleted();
        }
        EventBus.Quest.OnObjectiveCompletedVisuals?.Invoke(objective);
        EventBus.Quest.OnNavigationRefreshNeeded?.Invoke(objective);
    }

    private QuestObjective FindQuestObjective(ObjectiveType type, bool multipleInQuest)
    {
        return multipleInQuest
            ? QuestObjectives.Find(x => x.objectiveType == type && !x.isDone)
            : QuestObjectives.Find(x => x.objectiveType == type);
    }
}
