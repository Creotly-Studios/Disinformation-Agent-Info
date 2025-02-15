using UnityEngine;

public class RobotStatistics : MonoBehaviour, IDamagable
{
    Robot robot;

    [Header("Max Parameters")]
    [SerializeField] private int maxHealth = 3;

    public int CurrentHealth { get; private set; } = 0;

    private void Awake()
    {
        robot = GetComponent<Robot>();
    }

    public void ResetUI()
    {
        CurrentHealth = maxHealth;
        robot.healthBarUI.SetMaxValue(maxHealth);
        robot.healthBarUI.SetCurrentValue(CurrentHealth);
    }

    public void TakeDamage(int healthDamage)
    {
        CurrentHealth -= healthDamage;
        if (CurrentHealth <= 0.0f)
        {
            HandleDeath();
            return;
        }

        robot.healthBarUI.SetCurrentValue(CurrentHealth);
        // robot.robotAnimation.PlayTargetAnimation(damageAnimation, true);
    }

    private void HandleDeath()
    {
        CurrentHealth = 0;
        robot.isDead = true;

        robot.healthBarUI.SetCurrentValue(CurrentHealth);
        EnemyCombatControllerScript.Instance.RemoveEnemy(robot);
        robot.robotAnimation.PlayTargetAnimation(AnimatorHashing.deathAnimation, true);

        QuestSO quest = QuestManager.Instance.activeQuest;
        if(quest != null && quest.currentObjective.objectiveType == ObjectiveType.FightBots)
        {
            quest.IncreaseQuestObjectiveProgressLevels(quest.currentObjective);
        }
    }
}
