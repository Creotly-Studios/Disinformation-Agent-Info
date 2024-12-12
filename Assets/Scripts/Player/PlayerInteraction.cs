using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Player _player;
    public PlayerData _playerData;

    [SerializeField] private Transform detectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GetComponent<Player>();
        _playerData = _player.playerData;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.interactPressed)
        {
            Interact();
        }
    }

    void Interact()
    {
        RaycastHit[] hits = Physics.SphereCastAll(detectTransform.position, _playerData.detectRadius,
            detectTransform.forward, _playerData.detectRange);
        foreach (RaycastHit hit in hits)
        {
            // Check if the object hit has an enemy tag or component
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // Check if the enemy is in front of the player
                Vector3 directionToEnemy = (hit.collider.transform.position - detectTransform.position).normalized;
                float dotProduct = Vector3.Dot(detectTransform.forward, directionToEnemy);

                if (dotProduct > 0.5f) // Adjust threshold to control front-facing precision
                {
                    interactable.Interact();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (detectTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectTransform.position, _playerData.detectRadius);
            Gizmos.DrawLine(detectTransform.position,
                detectTransform.position + detectTransform.forward * _playerData.detectRange);
            Gizmos.DrawWireSphere(detectTransform.position + detectTransform.forward * _playerData.detectRange,
                _playerData.detectRadius);
        }
    }

    public GameObject GetInteractableObject()
    {
        RaycastHit[] hits = Physics.SphereCastAll(detectTransform.position, _playerData.detectRadius, detectTransform.forward, _playerData.detectRange);
        foreach (RaycastHit hit in hits)
        {
            GameObject inter = hit.collider.gameObject;
            IInteractable interactable = inter.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Vector3 directionToEnemy = (hit.collider.transform.position - detectTransform.position).normalized;
                float dotProduct = Vector3.Dot(detectTransform.forward, directionToEnemy);

                if (dotProduct > 0.5f) // Adjust threshold to control front-facing precision
                {
                    return inter;
                }
            }
        }
        return null;
    }

}