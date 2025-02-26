using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestSO", menuName = "Scriptable Objects/QuestSO")]
public class QuestSO : ScriptableObject
{
    [Header("Status")]
    public bool isComplete;

    [field: Header("Quest Information")]
    public string questTitle;
    [TextArea] public string description;
    [field: SerializeField] public int QuestReward { get; private set; }
    [field: SerializeField] public List<QuestObjectives> questObjectives { get; private set; }

    public void QuestSO_Update()
    {
        CheckIfQuestIsComplete();
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
