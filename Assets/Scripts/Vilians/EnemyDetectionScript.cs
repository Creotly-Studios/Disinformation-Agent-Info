using UnityEngine;

public class EnemyDetectionScript : MonoBehaviour
{
    [Header("Parameters")]
    [field: SerializeField] public float ViewAngle { get; set; }
    [field: SerializeField] public float ViewRadius { get; set; }

    [Header("Layer Masks")]
    [field: SerializeField] public LayerMask TargetLayerMask { get; set; }
    [field: SerializeField] public LayerMask ObstacleLayerMask { get; set; }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (angleIsGlobal != true)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
