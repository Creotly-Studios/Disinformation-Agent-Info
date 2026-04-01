using UnityEngine;

[RequireComponent(typeof(Player_v2))]
public class PlayerAnimationManager : MonoBehaviour
{
    private Player_v2 player;
    private Animator  animator;
    private static readonly int MOVE_VEL_HASH = Animator.StringToHash("moveVel"); 

    public void SetBool(int hash, bool value) => animator.SetBool(hash, value);
    public AnimatorStateInfo GetCurrentStateInfo(int layer = 0) => animator.GetCurrentAnimatorStateInfo(layer);
    public void SetSprintBlend(float value, float delta) => animator.SetFloat(MOVE_VEL_HASH, value, 0.1f, delta);

    private void Awake()
    {
        player   = GetComponent<Player_v2>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Animation_Update()
    {
        player.isAttacking = animator.GetBool(AnimatorHashing.ISATTACKING_HASH);
        player.performingAction = animator.GetBool(AnimatorHashing.ISPERFORMING_HASH);
    }

    public void SetMovementBlend(float moveVel, float delta)
    {
        animator.SetFloat(MOVE_VEL_HASH, moveVel, 0.1f, delta);
    }

    public void SetVerticalVelocityBlend(float yVel, float delta)
    {
        animator.SetFloat(AnimatorHashing.Y_VEL_HASH, yVel, 0.1f, delta);
    }

    public void PlayAttackAnimation(int animatorHash, bool isMirror)
    {
        animator.SetBool(AnimatorHashing.ISATTACKING_HASH, true);
        animator.SetBool(AnimatorHashing.ISPERFORMING_HASH, true);
        animator.SetBool(AnimatorHashing.ISMIRROR_HASH, isMirror);

        animator.CrossFade(animatorHash, 0.1f);
    }
}
