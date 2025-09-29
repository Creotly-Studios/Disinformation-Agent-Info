using UnityEngine;
using Ink.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

//Optimization and Faster to Get Reasons, Rather than letting Unity Hash by itself, store hashed value and used accordingly
public static class AnimatorHashing
{
    #region States

    //Damage
    public static int deathAnimation;
    public static int damageAnimation;

    //Turning
    public static int turn_L_90;
    public static int turn_R_90;
    public static int turn_L_180;
    public static int turn_R_180;

    //Movement
    public static int stepBackHash;

    //Action States
    public static int attackingHash;

    #endregion

    #region Animator Parameters

    //Locomotion Parameter
    public static int movingHash;
    public static int canRotateHash;
    public static int isGroundedHash;

    //Action Parameters
    public static int isJumpingHash;
    public static int isAttackingHash;
    public static int rootMotionRotateHash;
    public static int isPerformingActionHash;

    #endregion

    public static int ConvertToHash(string parameterName)
    {
        return Animator.StringToHash(parameterName);
    }

    public static void StringToHash()
    {
        //State
        deathAnimation = Animator.StringToHash("Death");
        attackingHash = Animator.StringToHash("Attack");

        //Turning
        turn_L_90 = Animator.StringToHash("Turn_90_L");
        turn_R_90 = Animator.StringToHash("Turn_90_R");
        turn_L_180 = Animator.StringToHash("Turn_180_L");
        turn_R_180 = Animator.StringToHash("Turn_180_R");

        //Movement
        stepBackHash = Animator.StringToHash("StepBack");

        //Locomotion Param
        movingHash = Animator.StringToHash("isMoving");
        canRotateHash = Animator.StringToHash("canRotate");
        isGroundedHash = Animator.StringToHash("isGrounded");

        //Action Parameters
        isAttackingHash = Animator.StringToHash("isAttacking");
        isJumpingHash = Animator.StringToHash("isJumping");
        isPerformingActionHash = Animator.StringToHash("performingAction");
    }

    public static void PlayTargetAnimation(Animator animator, int targetAnimation, bool performAction, float transitionDuration = 0.1f)
    {
        animator.applyRootMotion = performAction;
        animator.SetBool(AnimatorHashing.isPerformingActionHash, performAction);
        animator.CrossFade(targetAnimation, transitionDuration);
    }
}

public static class Maths_PhysicsHelper
{
    public static float CalculateViewAngle(Vector3 forward, Vector3 targetDirection)
    {
        targetDirection.y = 0.0f;
        float viewAngle = Vector3.Angle(forward, targetDirection);
        Vector3 cross = Vector3.Cross(forward, targetDirection);

        if (cross.y < 0.0f)
        {
            viewAngle = -viewAngle;
        }
        return viewAngle;
    }
}

public static class Tools
{
    public static T RandomObjectWithExclude<T>(T exclude, T[] objectArray) where T : UnityEngine.Object
    {
        List<int> indexList = new();
        for(int i = 0; i < objectArray.Length; i++)
        {
            if(objectArray[i] == exclude)
            {
                continue;
            }
            indexList.Add(i);
        }
        if (indexList.Count < 0) { return exclude; }
        int rnd = Random.Range(0, indexList.Count);
        return objectArray[indexList[rnd]];
    }
}


public static class InkPathHelpers
{
    // Returns a HashSet of candidate path names found in the compiled story JSON
    public static List<string> GetAllPathsFromStory(Story story)
    {
        var outputs = new List<string>();
        var knots = story.mainContentContainer.namedContent.Keys;
        knots.ToList().ForEach((knot) =>
        {
            outputs.Add(knot);
        });
        return outputs;
    }

    // Check existence (case-sensitive)
    public static bool PathExistsInStory(Story story, string pathName)
    {
        if (story == null || string.IsNullOrEmpty(pathName)) return false;
        var set = GetAllPathsFromStory(story);
        return set.Contains(pathName);
    }
}

