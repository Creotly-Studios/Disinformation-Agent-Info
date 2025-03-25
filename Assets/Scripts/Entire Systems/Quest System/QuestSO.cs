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

    private void SetCompletedObjective(QuestObjectives questObjective, QuestObjectiveNavIdentifier identifier)
    {
        questObjective.isDone = (questObjective.progressValue >= questObjective.targetValue);

        if (questObjective.isDone != true)
        {
            return;
        }
        if (identifier != null) { identifier.MarkCompleted(); }
        QuestManager.Instance.popupPanel.DisplayPopUpWindow(null, NoticeType.ObjectiveCompleted, null, questObjective);
    }
}
