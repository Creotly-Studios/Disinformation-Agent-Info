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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !Player_v2.Instance.IsPlayerDead())
        {
            if (killType == KillZoneType.InstaKill)
            {
                Player_v2.Instance.Damage.TakeDamage(100000);
                Debug.Log("Player has been killed by an insta-kill zone.");
            }
            else if (killType == KillZoneType.SlowKill)
            {
                StartCoroutine(SlowKill());
            }
        }
    }

    private IEnumerator SlowKill()
    {
        Player_v2 player = Player_v2.Instance;
        if (!player.IsPlayerDead())
        {
            PlayerDamageHandler damage = player.Damage;

            damage.TakeDamage(slowKillDamage);
            yield return new WaitForSeconds(slowKillRate);
            damage.TakeDamage(slowKillDamage);
            yield return new WaitForSeconds(slowKillRate);
            damage.TakeDamage(slowKillDamage);
            yield return new WaitForSeconds(slowKillRate);
        }

    }
}
