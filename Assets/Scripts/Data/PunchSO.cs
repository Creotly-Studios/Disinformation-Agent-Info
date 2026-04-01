using UnityEngine;

[CreateAssetMenu(fileName = "PunchSO", menuName = "Scriptable Objects/PunchSO")]
public class PunchSO : ScriptableObject
{
    private int attackHash;

    [Header("Parameters")]
    public int damage = 1;
    public string punchName;
    [SerializeField] private AudioClip audio;

    public void Initialize()
    {
        attackHash = Animator.StringToHash(punchName);
    }

    public void PerformAttackAction(bool isMirror, PlayerAnimationManager animationManager)
    {
        animationManager.PlayAttackAnimation(attackHash, isMirror);
        AudioManager.Instance.PlaySFX(audio);
    }
}
