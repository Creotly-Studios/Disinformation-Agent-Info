using UnityEngine;
using UnityEngine.Events;

public class Throphy : MonoBehaviour
{
    [SerializeField] private UnityEvent pickUpEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pickUpEvent?.Invoke();
            QuestManager questManager = QuestManager.Instance;
            QuestObjectives objective = questManager.FindQuestObjective(ObjectiveType.Trophy);


            if (objective != null)
            {
                questManager.activeQuest.IncreaseQuestObjectiveProgressLevels(objective);
            }
            //GameManager.Instance.MissionComplete();
            //Destroy(gameObject);
        }
    }
}
