using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class TaskListManager : MonoBehaviour
{
    private List<TaskSlotContent> contentList = new();
    public static TaskListManager Instance { get; private set; }

    [Header("Parameters")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TaskSlotContent prefab;
    [SerializeField] private Transform contentDrawer;
    [SerializeField] private TextMeshProUGUI titleText;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetUpTaskList(QuestSO quest)
    {
        ResetTaskList();

        titleText.text = quest.questTitle;
        foreach(var objective in quest.questObjectives)
        {
            TaskSlotContent content = Instantiate(prefab, contentDrawer);

            content.Initialize(objective);
            contentList.Add(content);
        }
    }

    public void UpdateTaskProgressLevels(QuestObjectives objectives)
    {
        TaskSlotContent content =contentList.Find(x => x.questObjective == objectives);
        if(content != null) { content.UpdateProgressLevels(); }
    }

    private void ResetTaskList()
    {
        contentList.ForEach(x => Destroy(x.gameObject));
        contentList.Clear();
    }
}
