using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    public List<QuestData> activeQuests; // Holds the active quests
    public UnityEvent onAllQuestsCompleted; // Events when all quests are done

    void Start()
    {
        // Reset all quests to not completed when the game starts
        foreach (var quest in activeQuests)
        {
            quest.isCompleted = false;
        }
    }

    public void UpdateQuest(string questName)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questName == questName && !quest.isCompleted)
            {
                quest.CompleteQuest();
                Debug.Log($"Quest Completed: {quest.questName}");
                CheckAllQuests();
                return;
            }
        }
    }

    private void CheckAllQuests()
    {
        foreach (var quest in activeQuests)
        {
            if (!quest.isCompleted) return; // If any quest is not completed, stop here
        }
        Debug.Log("All quests completed!");
        onAllQuestsCompleted?.Invoke(); // Invoke Unity Event for next actions
    }
}
