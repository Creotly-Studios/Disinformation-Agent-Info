using UnityEngine;

public class Enemy_Kamikaze : MonoBehaviour, IDamagable
{
    private bool _hasAttacked = false;
    public EnemyData e_data;
    public int currentHealth;


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !_hasAttacked)
        {
            Explode();
        }
    }

    private void Explode()
    {
        _hasAttacked = true; // Mark as exploded to prevent multiple attacks

        // Damage player
        IDamagable damagable = Player_v2.Instance.GetComponent<IDamagable>();
        damagable?.TakeDamage(e_data.damage);

        // Play explosion effect if assigned
        if (e_data.deathEffect != null)
        {
            Instantiate(e_data.deathEffect, transform.position, Quaternion.identity);
        }

        // Destroy the enemy (simulate explosion)
        TakeDamage(1000);
    }

    public void TakeDamage(int healthDamage)
    {
        currentHealth -= healthDamage;
        Vector3 knockbackDirection = (transform.position - Player_v2.Instance.transform.position).normalized;// Apply knockback when taking damage

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        QuestSO quest = QuestManager.Instance.activeQuest;
        if(quest != null)
        {
            QuestObjectives objective = quest.FindQuestObjective(ObjectiveType.FightBots);
            if (objective != null && objective.isDone != true)
            {
                quest.IncreaseQuestObjectiveProgressLevels(objective, null);
            }
        }
        Destroy(gameObject, e_data.destroyTime);
    }
}
