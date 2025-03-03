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

    private bool HasMiniGameObjective(QuestObjectives objective)
    {
        objective = questObjectives.Find(x => x.objectiveType == ObjectiveType.BiasBingo);
        if (objective != null) { return true; }

        objective = questObjectives.Find(x => x.objectiveType == ObjectiveType.MisInfoGames);
        if (objective != null) { return true; }

        objective = questObjectives.Find(x => x.objectiveType == ObjectiveType.SpotTheSource);
        if (objective != null) { return true; }
        return false;
    }

    private void CheckIfQuestIsComplete()
    {
        QuestObjectives objective = questObjectives.Find(x => x.isDone != true);
        isComplete = (objective == null);
    }

    public void DecreaseQuestObjectiveProgressLevels(QuestObjectives questObjective)
    {
        questObjective.progressValue--;
        questObjective.isDone = (questObjective.progressValue >= questObjective.targetValue);
        if (questObjective.isDone) { QuestManager.Instance.popupPanel.DisplayPopUpWindow(null, NoticeType.ObjectiveCompleted, null, questObjective); }
    }

    public void IncreaseQuestObjectiveProgressLevels(QuestObjectives questObjective)
    {
        if(questObjective.isDone)
        {
            return;
        }
        questObjective.progressValue++;
        questObjective.isDone = (questObjective.progressValue >= questObjective.targetValue);
        if(questObjective.isDone) { QuestManager.Instance.popupPanel.DisplayPopUpWindow(null, NoticeType.ObjectiveCompleted, null, questObjective); }
    }
}
