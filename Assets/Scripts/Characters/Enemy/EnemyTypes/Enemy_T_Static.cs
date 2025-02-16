using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Enemy_T_Static : MonoBehaviour
{
    public enum StaticType { Stationary, Rotating, Tracking }
    public StaticType staticType;
    
    Enemy enemy;
    public float rotationSpeed = 50f;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (enemy.currentHealth <= 0) return;

        switch (staticType)
        {
            case StaticType.Stationary:
                // Do nothing, just stand still.
                break;

            case StaticType.Rotating:
                RotateContinuously();
                break;

            case StaticType.Tracking:
                TrackPlayer();
                break;
        }
    }

    void RotateContinuously()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    void TrackPlayer()
    {
        if (enemy.PlayerInSightRange())
        {
            Vector3 direction = (enemy.Player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
