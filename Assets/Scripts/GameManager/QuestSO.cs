using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestSO", menuName = "Scriptable Objects/QuestSO")]
public class QuestSO : ScriptableObject
{
    [Header("Status")]
    public bool isComplete;
    public QuestObjectives currentObjective { get; private set; }

    [Header("Dialogue Texts")]
    public TextAsset instructionDialogue;
    public List<TextAsset> dialogueTexts = new();

    [field: Header("Quest Information")]
    public string questTitle;
    [TextArea] public string description;
    [field: SerializeField] public int QuestReward { get; private set; }
    [field: SerializeField] public List<QuestObjectives> questObjectives { get; private set; }

    public void QuestSO_Update()
    {
        AssignObjective();
    }

    private void AssignObjective()
    {
        if (currentObjective == null || currentObjective.isDone == true)
        {
            QuestObjectives objective = questObjectives.Find(x => x.isDone != true);
            if (objective != null)
            {
                currentObjective = objective;
                return;
            }
            isComplete = true;
            return;
        }
    }

    public void IncreaseQuestObjectiveProgressLevels(QuestObjectives questObjective)
    {
        if(questObjective.isDone)
        {
            return;
        }
        questObjective.progressValue++;
        questObjective.isDone = (questObjective.progressValue >= questObjective.targetValue);
    }
}
