using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
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
            return;
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
            activeQuest = questSO;
        }
        activeQuest.QuestSO_Update();

        if(activeQuest.isComplete) 
        {
            SaveManagerSystem.Instance.AutoSave();

            GameManager.Instance.MissionComplete();
            popupPanel.DisplayPopUpWindow(null, NoticeType.QuestCompleted, activeQuest);
        }
    }

    public void RestoreQuestProgress(List<SerializableQuestData> questDataList)
    {
        for(int i = 0; i < availableQuests.Count; i++)
        {
            QuestSO quest = availableQuests[i];
            SerializableQuestData questData = questDataList[i];

            if (quest.questTitle.Equals(questData.questName) != true)
            {
                continue;
            }
            questData.RestoreQuestValues(quest);
        }
    }
}
