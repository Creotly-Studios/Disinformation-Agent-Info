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

    public void SetMiniGame(ComputerPanel_UI computerPanel)
    {
        
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
        if(activeQuest.isComplete)
        {
            if (hasNotified != true)
            {
                hasNotified = true;
                popupPanel.DisplayPopUpWindow(null, NoticeType.QuestCompleted, activeQuest);
            }
        }

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
    }
}
