using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskSlotContent : MonoBehaviour
{
    public QuestObjectives questObjective { get; private set; }

    [Header("Parameters")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void Initialize(QuestObjectives objective)
    {
        questObjective = objective;
        progressBar.maxValue = questObjective.targetValue;
        progressBar.value = questObjective.progressValue;

        descriptionText.text = questObjective.description;
        titleText.text = questObjective.objectiveType.ToString();
    }

    public void UpdateProgressLevels()
    {
        progressBar.value = questObjective.progressValue;
    }
}
