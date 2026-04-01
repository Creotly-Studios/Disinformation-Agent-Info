using UnityEngine;

public class KillPlayerOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !Player_v2.Instance.IsPlayerDead())
        {
            Player_v2.Instance.Damage.TakeDamage(1000);
        }
    }
}
