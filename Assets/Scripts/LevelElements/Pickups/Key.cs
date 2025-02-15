using UnityEngine;
using UnityEngine.Events;

public class Key : MonoBehaviour
{
    [SerializeField] private UnityEvent pickUpEvent;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            pickUpEvent?.Invoke();
            Destroy(gameObject);
        }
    }
}
