using UnityEngine;

[CreateAssetMenu(fileName = "PunchSO", menuName = "Scriptable Objects/PunchSO")]
public class PunchSO : ScriptableObject
{
    public string punchName = "";
    public AnimatorOverrideController animatorOV;
    public int damage = 1;
    //vfx
    //float knockback
}
