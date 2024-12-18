using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest")]
public class QuestData : ScriptableObject
{
    public string questName;       // Name of the quest
    public string description;     // Description of the quest
    public bool isCompleted;       // Tracks if the quest is done

    public void CompleteQuest()
    {
        isCompleted = true;
    }
}
