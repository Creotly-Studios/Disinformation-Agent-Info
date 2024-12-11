using UnityEngine;

[CreateAssetMenu(fileName = "Punch", menuName = "Scriptable Objects/Punch")]
public class PunchSO : ScriptableObject
{
    public int damage;
    public RuntimeAnimatorController animatorOV;
}
