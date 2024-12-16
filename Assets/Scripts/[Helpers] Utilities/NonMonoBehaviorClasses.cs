using UnityEngine;

[System.Serializable]
public struct BoundaryFloat
{
    public float lowerBound;
    public float upperBound;

    public BoundaryFloat(float min, float max)
    {
        lowerBound = min;
        upperBound = max;
    }
}

[System.Serializable]
public class CharacterProfile
{
    NPC character;

    [Header("Topic Stans")]
    [Range(1, 10)] public int proScience;
    [Range(1, 10)] public int economicsFan;
    [Range(1, 10)] public int governmentRating;

    [Header("Traits")]
    [Range(1, 10)] public int suscpiciousLevels;
    [Range(1, 10)] public int susceptibleToAnger;

    public void Initialize(NPC character)
    {
        this.character = character;
    }

    public void ApplyChoiceEffect(int choiceImpact)
    {
        character.warmingUpRadar += choiceImpact;
        character.ChangeEmotion();
    }

    public void Reset()
    {
        character.warmingUpRadar = 50;
        character.ChangeEmotion();
    }
}

[System.Serializable]
public class VisualTarget
{
    private float lastDetected;
    private Vector3 targetDirection;
    public float Age { get { return Time.time - lastDetected; } }

    [Header("Target")]
    [field: SerializeField] public Player Target { get; private set; }

    [field: Header("Target Information")]
    [field: SerializeField] public float TargetAngle { get; private set; }
    [field: SerializeField] public float TargetDistance { get; private set; }

    [Header("Target Score")]
    public float targetScore;

    public VisualTarget(Player potentialTarget)
    {
        SetTarget(potentialTarget);
    }

    public void SetTarget(Player potentialTarget)
    {
        Target = potentialTarget;
    }

    public void UpdateTargetInformation(Transform source)
    {
        if(Target == null)
        {
            ClearDetails();
            return;
        }
        Transform targetTransform = Target.transform;

        lastDetected = Time.time;
        targetDirection = (source.position - targetTransform.position);

        TargetDistance = targetDirection.magnitude;
        targetDirection = targetDirection.normalized;
        TargetAngle = Maths_PhysicsHelper.CalculateViewAngle(source.forward, targetDirection);
    }

    public void ClearDetails()
    {
        targetScore = 0.0f;
        TargetAngle = 0.0f;
        lastDetected = 0.0f;

        TargetDistance = 0.0f;
        targetDirection = Vector3.zero;
    }
}

public class EnemyMemoryHandlerScript
{
    public void UpdateVisualTargets(Robot robot)
    {
        for (int i = 0; i < robot.robotMemory.playerList.Count; i++)
        {
            Player target = robot.robotMemory.playerList[i];
            RefreshVisualTarget(robot, target);
        }
    }

    private void RefreshVisualTarget(Robot robot, Player target)
    {
        VisualTarget visualTarget = FetchVisualTarget(robot, target);

        visualTarget.SetTarget(target);
        visualTarget.UpdateTargetInformation(robot.transform);
    }

    private VisualTarget FetchVisualTarget(Robot robot, Player target)
    {
        VisualTarget visualTarget = robot.potentialTargets.Find(x => x.Target == target);
        if (visualTarget == null)
        {
            visualTarget = new VisualTarget(target);
            robot.potentialTargets.Add(visualTarget);
        }
        return visualTarget;
    }

    public void ForgetVisualTarget(Robot robot, float olderThan)
    {
        robot.potentialTargets.RemoveAll(x => x.Age > olderThan);
        robot.potentialTargets.RemoveAll(x => x.Target == null);
        robot.potentialTargets.RemoveAll(x => x.Target?.isDead == true);
    }
}
