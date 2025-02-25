using System;
using UnityEngine;

public class PlayerPhysicsInteraction : MonoBehaviour
{
    private Player_v2 _player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GetComponent<Player_v2>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb != null)
        {
            Vector3 forceDir = hit.gameObject.transform.position - transform.position;
            forceDir.y = 0;
            forceDir.Normalize();
            
            rb.AddForceAtPosition(forceDir * _player.PlayerData.pushForce, transform.position, ForceMode.Impulse);
        }
    }
}