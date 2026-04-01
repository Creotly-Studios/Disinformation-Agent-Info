using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class TaskListManager : MonoBehaviour
{
    private readonly List<TaskSlotContent> contentList = new();

    [Header("Parameters")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TaskSlotContent prefab;
    [SerializeField] private Transform contentDrawer;
    [SerializeField] private TextMeshProUGUI titleText;

    private void Start()
    {
        EventBus.TaskList.OnRefreshTaskList += SetUpTaskList;
        EventBus.TaskList.OnUpdateTaskListValues += UpdateTaskProgressLevels;
    }

    private void OnDestroy()
    {
        EventBus.TaskList.OnRefreshTaskList -= SetUpTaskList;
        EventBus.TaskList.OnUpdateTaskListValues -= UpdateTaskProgressLevels;
    }

    private void SetUpTaskList(QuestSO quest)
    {
        ResetTaskList();
        titleText.text = quest.questTitle;
        foreach(var objective in quest.QuestObjectives)
        {
            TaskSlotContent content = Instantiate(prefab, contentDrawer);
            content.Initialize(objective);
            contentList.Add(content);
        }
    }

    public void UpdateTaskProgressLevels(QuestObjective objectives)
    {
        TaskSlotContent content = contentList.Find(x => x.questObjective == objectives);
        if(content != null)
        { 
            content.UpdateProgressLevels();
        }
    }

    private void ResetTaskList()
    {
        contentList.ForEach(x => Destroy(x.gameObject));
        contentList.Clear();
    }
}
