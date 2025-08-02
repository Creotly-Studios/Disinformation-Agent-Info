using UnityEngine;
using UnityEngine.Events;

public class FactroyLevelGenerator : MonoBehaviour, IDamagable
{
    public GameObject explosionEffect;
    public int maxHealth = 3;
    private int currentHealth;
    public UnityEvent onDestroyed;

    [Space]
    public UnityEngine.UI.Image healtBarFill;
    float healthNormalized;

    public void TakeDamage(int healthDamage)
    {
        currentHealth -= healthDamage;
        UpdateHealthBar();
        if (currentHealth <= 0)
        {
           Die();
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Die()
    {
        onDestroyed?.Invoke();
        if (explosionEffect != null)
        {
            GameObject _ = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(_, 1.25f);    
        }

        QuestSO quest = QuestManager.Instance.activeQuest;
        if (quest != null)
        {
            QuestObjectives objective = quest.FindQuestObjective(ObjectiveType.Generator);
            if (objective != null && objective.isDone != true)
            {
                quest.IncreaseQuestObjectiveProgressLevels(objective, null);
            }
        }
        gameObject.SetActive(false); //or destroy idk
    }

    void UpdateHealthBar()
    {
        healthNormalized = (float)currentHealth / maxHealth;
        healtBarFill.fillAmount = healthNormalized;
    }
}
