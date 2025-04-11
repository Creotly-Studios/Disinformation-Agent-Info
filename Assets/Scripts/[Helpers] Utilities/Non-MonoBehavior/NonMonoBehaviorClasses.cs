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
public class QuestObjectives
{
    [Header("Status")]
    public bool isDone;

    [Header("Objective Information")]
    public int targetValue;
    public int progressValue;
    public ObjectiveType objectiveType;
    [TextArea] public string description;

    public void LoadProgressValue(int newValue)
    {
        progressValue = newValue;
        isDone = progressValue >= targetValue;
    }
}
