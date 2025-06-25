using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    QuestObjectives puzzle_Objective;
    QuestObjectives combat_Objective;
    public QuestObjectiveNavIdentifier identifier { get; private set; }

    [Header("Rewards")]
    [SerializeField] private GameObject combatCompleteReward;
    [SerializeField] private GameObject puzzleCompleteReward;

    private void Awake()
    {
        puzzle_Objective = combat_Objective = new();
        identifier = GetComponent<QuestObjectiveNavIdentifier>();
    }

    private void Update()
    {
        if(puzzle_Objective.targetValue == 0 || combat_Objective.targetValue == 0)
        {
            QuestManager questManager = QuestManager.Instance;
            puzzle_Objective = questManager.FindQuestObjective(ObjectiveType.Puzzle);
            combat_Objective = questManager.FindQuestObjective(ObjectiveType.FightBots);
        }
        CheckPuzzleCompletion();
    }

    private void CheckPuzzleCompletion()
    {
        puzzleCompleteReward.SetActive(puzzle_Objective != null && puzzle_Objective.isDone);
        combatCompleteReward.SetActive(combat_Objective != null && combat_Objective.isDone);
    }
}
