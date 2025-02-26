using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    private bool hasNotified;
    public static QuestManager Instance;

    [field: Header("Parameters")]
    public bool allQuestCompleted { get; private set; }
    [field: SerializeField] public QuestSO activeQuest;
    [field: SerializeField] public NoticePopup popupPanel { get; private set; }
    [field: SerializeField] public List<QuestSO> availableQuests { get; private set; } = new List<QuestSO>();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        for(int i = 0; i < availableQuests.Count; i++)
        {
            availableQuests[i] = Instantiate(availableQuests[i]);
        }
    }

    public QuestObjectives GetObjective()
    {
        for(int i = 0; i < activeQuest.questObjectives.Count; i++)
        {
            QuestObjectives potObjective = activeQuest.questObjectives[i];

            ObjectiveType type = potObjective.objectiveType;
            if (type == ObjectiveType.MisInfoGames || type == ObjectiveType.SpotTheSource || type == ObjectiveType.BiasBingo )
            {
                return potObjective;
            }
        }
        return null;
    }

    public QuestObjectives FindQuestObjective(ObjectiveType type)
    {
        if(activeQuest == null)
        {
            return null;
        }
        return activeQuest.questObjectives.Find(x => x.objectiveType == type);
    }

    public void Quest_Update()
    {
        if (DialogueManager.Instance.dialogueIsPlaying)
        {
            return;
        }

        if (allQuestCompleted)
        {
            return;
        }
        AssignQuests();
    }

    public void AssignQuests()
    {
        if(activeQuest == null || activeQuest.isComplete == true)
        {
            QuestSO questSO = availableQuests.Find(x => x.isComplete != true);
            if (questSO == null)
            {
                allQuestCompleted = true;
                return;
            }

            hasNotified = false;
            activeQuest = questSO;
            return;
        }

        activeQuest.QuestSO_Update();
        if (activeQuest.isComplete)
        {
            print(activeQuest + " complete status is " + activeQuest.isComplete);
            if (hasNotified != true)
            {
                hasNotified = true;
                popupPanel.DisplayPopUpWindow(null, NoticeType.QuestCompleted, activeQuest);
            }
        }
    }
}
