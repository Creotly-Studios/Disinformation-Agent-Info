using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    private QuestObjective puzzleObjective;
    private QuestObjective combatObjective;
    public QuestObjectiveNavIdentifier Identifier { get; private set; }

    [Header("Rewards")]
    [SerializeField] private GameObject combatCompleteReward;
    [SerializeField] private GameObject puzzleCompleteReward;

    private void Awake() => Identifier = GetComponent<QuestObjectiveNavIdentifier>();

    private void OnEnable()
    {
        EventBus.Quest.OnActiveQuestChanged += OnQuestChanged;
        EventBus.Quest.OnNavigationRefreshNeeded += OnObjectiveCompleted;
    }

    private void OnDisable()
    {
        EventBus.Quest.OnActiveQuestChanged -= OnQuestChanged;
        EventBus.Quest.OnNavigationRefreshNeeded -= OnObjectiveCompleted;
    }

    private void OnQuestChanged(QuestSO quest)
    {
        if (quest == null)
        {
            return;
        }
        puzzleObjective = quest.FindQuestObjective(ObjectiveType.Puzzle);
        combatObjective = quest.FindQuestObjective(ObjectiveType.FightBots);
        RefreshRewards();
    }

    private void OnObjectiveCompleted(QuestObjective _) => RefreshRewards();

    private void RefreshRewards()
    {
        puzzleCompleteReward.SetActive(puzzleObjective != null && puzzleObjective.isDone);
        combatCompleteReward.SetActive(combatObjective != null && combatObjective.isDone);
    }
}