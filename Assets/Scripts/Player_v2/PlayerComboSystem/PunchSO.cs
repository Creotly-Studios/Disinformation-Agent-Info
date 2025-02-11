using UnityEngine;

[CreateAssetMenu(fileName = "PunchSO", menuName = "Scriptable Objects/PunchSO")]
public class PunchSO : ScriptableObject
{
    [Header("Parameters")]
    public int damage = 1;
    public string punchName = "";
    public AnimatorOverrideController animatorOV;

    [Header("AI Attack Status")]
    public int weight;
    public float recoveryTime = 1.5f;

    [Header("AI Attack Parameters")]
    public BoundaryFloat attackAngle = new BoundaryFloat(-180f, 180f);
    public BoundaryFloat distanceToAttack = new BoundaryFloat(0f, 2.5f);

    public void PerformAttackAction(Robot robot)
    {
        robot.animator.runtimeAnimatorController = animatorOV;
        robot.robotAnimation.PlayTargetAnimation(AnimatorHashing.attackingHash, true);
    }

    public void PerformAttackAction(Animator animator)
    {
        animator.runtimeAnimatorController = animatorOV;
        animator.Play("attack", 0, 0);
    }

    //vfx
    //float knockback
}
