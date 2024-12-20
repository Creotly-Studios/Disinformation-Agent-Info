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
public struct Response
{
    public ResponseType responseType;

    public float Evaluate(CharacterProfile profile)
    {
        float baseImpact = CalculateBaseImpact();
        float impact = baseImpact;

        impact += profile.ignorant * 0.4f;
        impact += profile.loyaltyToSource * 0.2f;

        impact -= profile.factLover * baseImpact * 0.5f;
        impact -= profile.echoChamber * baseImpact * 0.6f;
        impact -= profile.susceptibleToAnger * baseImpact * 0.5f;

        return Mathf.Clamp(impact, -15f, 15f);
    }

    private float CalculateBaseImpact()
    {
        float baseImpact = 0.0f;

        switch(responseType)
        {
            case ResponseType.Logical:
                baseImpact = 7f;
                break;
            case ResponseType.Reserved:
                baseImpact = 2.5f;
                break;
            case ResponseType.Emotional:
                baseImpact = -3f;
                break;
            case ResponseType.Argumentative:
                baseImpact = -2.0f;
                break;
        }
        return baseImpact;
    }
}

[System.Serializable]
public class QuestObjectives
{
    public bool isDone;
    public int targetValue;
    public int progressValue;
    public ObjectiveType objectiveType;
    [TextArea] public string description;
}

[System.Serializable]
public class CharacterProfile
{
    NPC character;

    [Header("Topic Stans")]
    [Range(1, 10)] public int ignorant;
    [Range(1, 10)] public int factLover;

    [Header("Traits")]
    [Range(1, 10)] public int echoChamber;
    [Range(1, 10)] public int loyaltyToSource;
    [Range(1, 10)] public int susceptibleToAnger;

    public void Initialize(NPC character)
    {
        this.character = character;
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
