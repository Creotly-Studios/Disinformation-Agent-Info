using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    public QuestManager questManager;  // Link to QuestManager
    public string questToComplete;     // Name of the quest to complete

    public void CompleteQuest()
    {
        questManager.UpdateQuest(questToComplete);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // When the player enters the trigger
        {
            CompleteQuest();
        }
    }
}
