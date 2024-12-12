using UnityEngine;

//Optimization and Faster to Get Reasons, Rather than letting Unity Hash by itself, store hashed value and used accordingly
public static class AnimatorHashing
{
    public static int deathAnimation;
    public static int jumpingAnimatorHash;
    public static int performingActionHash;

    public static void StringToHash()
    {
        deathAnimation = Animator.StringToHash("Death");

        //Parameters
        jumpingAnimatorHash = Animator.StringToHash("isJumping");
        performingActionHash = Animator.StringToHash("PerformingAction");
    }
}
