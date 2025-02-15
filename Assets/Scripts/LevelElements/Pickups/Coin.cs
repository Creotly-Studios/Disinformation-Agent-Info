using UnityEngine;
using UnityEngine.Events;

public class Coin : MonoBehaviour
{
    [SerializeField] private UnityEvent pickUpEvent;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player_v2>().CallPlayerCoinPickup();
            pickUpEvent?.Invoke();
            Destroy(gameObject);
        }
    }

}
