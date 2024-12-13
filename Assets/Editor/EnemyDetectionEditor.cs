using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyDetectionScript))]
public class EnemyDetectionEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyDetectionScript fov = (EnemyDetectionScript)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.ViewRadius);
        Vector3 viewAngleA = fov.DirectionFromAngle(-fov.ViewAngle / 2, false);
        Vector3 viewAngleB = fov.DirectionFromAngle(fov.ViewAngle / 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.ViewRadius);
    }
}

