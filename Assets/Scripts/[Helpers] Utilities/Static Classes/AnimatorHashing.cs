using UnityEngine;
using Ink.Runtime;
using System.Linq;
using System.Collections.Generic;

//Optimization and Faster to Get Reasons, Rather than letting Unity Hash by itself, store hashed value and used accordingly
public static class AnimatorHashing
{
    public static int ISMIRROR_HASH;
    public static int ISATTACKING_HASH;
    public static int ISPERFORMING_HASH;

    //Locomotion Parameter
    public static int Y_VEL_HASH;
    public static int ISMOVING_HASH;
    public static int CANROTATE_HASH;
    public static int ISGROUNDED_HASH;

    public static int ConvertToHash(string parameterName)
    {
        return Animator.StringToHash(parameterName);
    }

    public static void StringToHash()
    {
        ISMIRROR_HASH = Animator.StringToHash("isMirror");
        ISATTACKING_HASH = Animator.StringToHash("isAttacking");
        ISPERFORMING_HASH = Animator.StringToHash("performingAction");

        Y_VEL_HASH = Animator.StringToHash("yVelocity");
        ISMOVING_HASH = Animator.StringToHash("isMoving");
        CANROTATE_HASH = Animator.StringToHash("canRotate");
        ISGROUNDED_HASH = Animator.StringToHash("isGrounded");
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

