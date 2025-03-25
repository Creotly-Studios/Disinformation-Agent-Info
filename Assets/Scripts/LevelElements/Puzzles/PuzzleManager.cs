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
        identifier = GetComponent<QuestObjectiveNavIdentifier>();
    }

    private void Update()
    {
        CheckPuzzleCompletion();
    }

    private void CheckPuzzleCompletion()
    {
        puzzle_Objective = QuestManager.Instance.FindQuestObjective(ObjectiveType.Puzzle);
        puzzleCompleteReward.SetActive(puzzle_Objective != null && puzzle_Objective.isDone);

        combat_Objective = QuestManager.Instance.FindQuestObjective(ObjectiveType.FightBots);
        combatCompleteReward.SetActive(combat_Objective != null && combat_Objective.isDone);
    }
}
