using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestSO", menuName = "Scriptable Objects/QuestSO")]
public class QuestSO : ScriptableObject
{
    [Header("Status")]
    public bool isComplete;
    [field: SerializeField] public int questCode { get; private set; }
    [field: SerializeField] public int questLevelIndex { get; private set; }

    [field: Header("Quest Information")]
    public string questTitle;
    [TextArea] public string description;
    [field: SerializeField] public int QuestReward { get; private set; }
    [field: SerializeField] public List<QuestObjectives> questObjectives { get; private set; }

    public void QuestSO_Update()
    {
        CheckIfQuestIsComplete();
    }

    public void CheckIfQuestIsComplete()
    {
        QuestObjectives objective = questObjectives.Find(x => x.isDone != true);
        isComplete = (objective == null);
    }

    public QuestObjectives FindNextObjective()
    {
        return questObjectives.Find(x => x.isDone != true);
    }

    public QuestObjectives GetMiniGameObjetive()
    {
        for (int i = 0; i < questObjectives.Count; i++)
        {
            QuestObjectives potObjective = questObjectives[i];

            ObjectiveType type = potObjective.objectiveType;
            if (type == ObjectiveType.MiniGame_MalignInfluence || type == ObjectiveType.MiniGame_SpotTheSource || type == ObjectiveType.MiniGame_BiasBingo)
            {
                return potObjective;
            }
        }
        return null;
    }

    public QuestObjectives FindQuestObjective(ObjectiveType type, bool multipleInScene = false)
    {
        if (multipleInScene != true)
        {
            return questObjectives.Find(x => x.objectiveType == type);
        }
        return questObjectives.Find(x => x.objectiveType == type && x.isDone != true);
    }

    private void SetCompletedObjective(QuestObjectives questObjective, QuestObjectiveNavIdentifier identifier)
    {
        TaskListManager.Instance.UpdateTaskProgressLevels(questObjective);
        questObjective.isDone = (questObjective.progressValue >= questObjective.targetValue);
        if (questObjective.isDone != true)
        {
            return;
        }
        if (identifier != null) { identifier.MarkCompleted(); }
        Player_v2.Instance.PlayerNav.canResetFilterNavList = true;
        QuestManager.Instance.popupPanel.DisplayPopUpWindow(null, NoticeType.ObjectiveCompleted, null, questObjective);
    }

    public void DecreaseQuestObjectiveProgressLevels(QuestObjectives questObjective, QuestObjectiveNavIdentifier identifier)
    {
        questObjective.progressValue--;
        SetCompletedObjective(questObjective, identifier);
    }

    public void IncreaseQuestObjectiveProgressLevels(QuestObjectives questObjective, QuestObjectiveNavIdentifier identifier)
    {
        if(questObjective.isDone)
        {
            return;
        }
        questObjective.progressValue++;
        SetCompletedObjective(questObjective, identifier);
    }
}
