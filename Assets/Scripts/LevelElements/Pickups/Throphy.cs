using UnityEngine;
using UnityEngine.Events;

public class Throphy : MonoBehaviour
{
    [SerializeField] private UnityEvent pickUpEvent;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            pickUpEvent?.Invoke();
            GameManager.Instance.MissionComplete();
            Destroy(gameObject);
        }
    }
}
