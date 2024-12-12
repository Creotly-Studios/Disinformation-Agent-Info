using System;
using UnityEngine;

public class PlayerPhysicsIInteraction : MonoBehaviour
{
    private Player _player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GetComponent<Player>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb != null)
        {
            Vector3 forceDir = hit.gameObject.transform.position - transform.position;
            forceDir.y = 0;
            forceDir.Normalize();
            
            rb.AddForceAtPosition(forceDir * _player.playerData.pushForce, transform.position, ForceMode.Impulse);
        }
    }
}
