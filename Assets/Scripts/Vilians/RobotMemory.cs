using UnityEngine;
using System.Collections.Generic;

public class RobotMemory : MonoBehaviour
{
    [Header("Classes")]
    Robot robot;
    public EnemyMemoryHandlerScript memory = new EnemyMemoryHandlerScript();

    [Header("Scan Parameters")]
    public float memorySpan = 3.0f;

    [Header("Target Parameters")]
    public List<Player_v2> playerList = new();
    public VisualTarget currentVisualTarget;

    [Header("Total Score Adjust Weight")]
    [Range(0, 1)] public float ageWeight = 0.75f;
    [Range(0, 1)] public float angleWeight = 0.55f;
    [Range(0, 1)] public float distanceWeight = 0.6f;
    
    [Header("Private Fields")]
    [SerializeField] Collider[] characterColliders;

    private void Awake()
    {
        robot = GetComponent<Robot>();
        characterColliders = new Collider[10];
    }

    public void RobotMemory_Update()
    {
        DetectForTarget();
    }

    private void DetectForTarget()
    {
        VisuallyDetectTarget();
        memory.UpdateVisualTargets(robot);
        memory.ForgetVisualTarget(robot, memorySpan);

        EvaluateVisualTargetScore();
    }

    public void ForgetCurrentTarget()
    {
        if (robot.target != null)
        {
            robot.target.ClearDetails();
        }
    }

    bool isInSight(Transform potentialTarget)
    {
        Vector3 targetDirection = (potentialTarget.position - robot.transform.position).normalized;
        float ViewableAngle = Vector3.Angle(targetDirection, robot.transform.forward);

        if (ViewableAngle < (robot.enemyDetectionScript.ViewAngle / 2))
        {
            if (!Physics.Linecast
                (robot.transform.position, potentialTarget.position, robot.enemyDetectionScript.ObstacleLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    void VisuallyDetectTarget()
    {
        Physics.OverlapSphereNonAlloc(robot.transform.position, robot.enemyDetectionScript.ViewRadius,
            characterColliders, robot.enemyDetectionScript.TargetLayerMask);

        playerList.Clear();
        foreach (Collider characterCollider in characterColliders)
        {
            if (characterCollider == null)
            {
                continue;
            }

            Player_v2 player = characterCollider.GetComponentInParent<Player_v2>();
            if (player != null)
            {
                if (playerList.Contains(player) == true)
                {
                    continue;
                }

                if (isInSight(player.transform))
                {
                    playerList.Add(player);
                }
            }
        }
    }

    void EvaluateVisualTargetScore()
    {
        for (int i = 0; i < robot.potentialTargets.Count; i++)
        {
            VisualTarget target = robot.potentialTargets[i];
            target.targetScore = CalculateVisualTargetScore(target);

            if (robot.target == null || target.targetScore > currentVisualTarget.targetScore)
            {
                robot.target = target;
                currentVisualTarget = target;
                robot.SetCurrentTarget(target.Source);
            }
        }
    }

    float Normalize(float minValue, float maxValue)
    {
        return 1 - (minValue / maxValue);
    }

    float CalculateVisualTargetScore(VisualTarget target)
    {
        float ageScore = Normalize(target.Age, memorySpan) * ageWeight;
        float angleScore = Normalize(target.TargetAngle, robot.enemyDetectionScript.ViewAngle) * angleWeight;
        float distanceScore = Normalize(target.TargetDistance, robot.enemyDetectionScript.ViewRadius) * distanceWeight;

        float targetScore = distanceScore + angleScore + ageScore;
        return targetScore;
    }
}
