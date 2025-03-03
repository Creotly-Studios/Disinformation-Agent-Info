using UnityEngine;

[CreateAssetMenu(fileName = "PunchSO", menuName = "Scriptable Objects/PunchSO")]
public class PunchSO : ScriptableObject
{
    private int attackHash;
    private int performActHash;

    [Header("Parameters")]
    public int damage = 1;
    public string punchName;
    public AnimatorOverrideController animation;

    [Header("AI Attack Status")]
    public int weight;
    public float recoveryTime = 1.5f;

    [Header("AI Attack Parameters")]
    public BoundaryFloat attackAngle = new BoundaryFloat(-180f, 180f);
    public BoundaryFloat distanceToAttack = new BoundaryFloat(0f, 2.5f);

    public void Initialize()
    {
        attackHash = Animator.StringToHash(punchName);
        performActHash = Animator.StringToHash("performingAction");
    }

    public void PerformAttackAction(Animator animator)
    {
        PlayTargetAnimation(animator, true);
    }

    private void PlayTargetAnimation(Animator animator, bool performAction, float transitionDuration = 0.2f)
    {
        animator.applyRootMotion = performAction;

        animator.SetBool(performActHash, performAction);
        animator.CrossFade(attackHash, transitionDuration);
    }

    //vfx
    //float knockback
}
