using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Enemy_T_Static : MonoBehaviour
{
    public enum StaticType { Stationary, Rotating, Tracking }
    public StaticType staticType;
    
    Enemy enemy;
    public float rotationSpeed = 50f;
    [SerializeField] private Transform rotateTransform;

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
        if (rotateTransform == null)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        } else {
            rotateTransform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }

    void TrackPlayer()
    {
        if (enemy.PlayerInSightRange())
        {
            Vector3 direction = (enemy.Player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            if (rotateTransform == null)
            {
                transform.LookAt(enemy.Player);
                // transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            } else {
                rotateTransform.LookAt(enemy.Player);
                // rotateTransform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
