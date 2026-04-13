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
        QuestSO quest = QuestManager.Instance.ActiveQuest;
        puzzleObjective = quest.FindQuestObjective(ObjectiveType.Puzzle);
        combatObjective = quest.FindQuestObjective(ObjectiveType.FightBots);

        EventBus.Quest.OnActiveQuestChanged += OnQuestChanged;
        EventBus.Quest.OnNavigationRefreshNeeded += OnObjectiveCompleted;
        EventBus.Gameplay.OnNewSceneLoaded += (bool _) => RefreshRewards();
    }

    private void OnDisable()
    {
        EventBus.Quest.OnActiveQuestChanged -= OnQuestChanged;
        EventBus.Quest.OnNavigationRefreshNeeded -= OnObjectiveCompleted;
        EventBus.Gameplay.OnNewSceneLoaded -= (bool _) => RefreshRewards();
    }

    private void OnQuestChanged(bool _, QuestSO quest)
    {
        if (quest == null)
        {
            return;
        }
        RefreshRewards();
    }

    private void OnObjectiveCompleted(QuestObjective _) => RefreshRewards();

    private void RefreshRewards()
    {
        puzzleCompleteReward.SetActive(puzzleObjective != null && puzzleObjective.isDone);
        combatCompleteReward.SetActive(combatObjective != null && combatObjective.isDone);
    }
}