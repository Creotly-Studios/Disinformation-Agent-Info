using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;

    [SerializeField] private Transform detectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        RaycastHit[] hits = Physics.SphereCastAll(detectTransform.position, playerData.detectRadius, detectTransform.forward, playerData.detectRange);
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
                    Debug.Log($"Interacting with {hit.collider.name} in front!");
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
            Gizmos.DrawWireSphere(detectTransform.position, playerData.detectRadius);
            Gizmos.DrawLine(detectTransform.position, detectTransform.position + detectTransform.forward * playerData.detectRange);
            Gizmos.DrawWireSphere(detectTransform.position + detectTransform.forward * playerData.detectRange, playerData.detectRadius);
        }
    }
}
