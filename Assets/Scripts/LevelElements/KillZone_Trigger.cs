using UnityEngine;
using System.Collections;

public class KillZone_Trigger : MonoBehaviour
{
    public enum KillZoneType
    {
        SlowKill, InstaKill
    }

    public KillZoneType killType;
    public float slowKillRate = 1.0f; // How often health decreases (in seconds)
    public int slowKillDamage = 1; // How much health is removed per tick

    private bool isPlayerInZone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (killType == KillZoneType.InstaKill)
            {
                Player_v2.Instance.PlayerStatistics.TakeDamage(1000);
            }
            else if (killType == KillZoneType.SlowKill)
            {
                isPlayerInZone = true;
                StartCoroutine(SlowKill());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
        }
    }

    private IEnumerator SlowKill()
    {
        while (isPlayerInZone)
        {
            Player_v2.Instance.PlayerStatistics.TakeDamage(slowKillDamage);
            yield return new WaitForSeconds(slowKillRate); // Wait before dealing damage again
        }
    }
}
