using System;
using UnityEngine;

[Serializable]
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

[Serializable]
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

[Serializable]
public class NPC_CharacterProfile
{
    public float AcceptanceValue { get; private set; } = 50.0f;

    [Header("Logical Parameters")]
    [Range(0f,1f)][SerializeField] public float topicInterestWeight;
    [Range(0f, 1f)][SerializeField] private float criticalThinkingWeight;

    [Header("Emotional Bias Parameters")]
    [Range(0f, 1f)][SerializeField] private float confirmationBiasWeight;
    [Range(0f, 1f)][SerializeField] private float emotionalSusceptibilityWeight;

    public void InitializeAcceptanceValue(float value, BarSliderUI slider)
    {
        AcceptanceValue = value;
        slider.SetMaxValue(100, 50.0f);
    }

    /// <summary>
    /// Called with the ink‐passed responseIndex (1–4) and baseValue (1–5).
    /// </summary>
    public void Evaluate_AcceptanceValue(int responseIndex, int baseValue, BarSliderUI slider, out float delta)
    {
        delta = Evaluate(responseIndex, baseValue);

        AcceptanceValue = Mathf.Clamp(AcceptanceValue + delta, 0f, 100f);
        slider.SetCurrentValue(AcceptanceValue);
        if(AcceptanceValue <= 25f)
        {
            QuestManager.Instance.popupPanel.DialoguePopup(Color.red, $"Has Completely Lost NPC's Trust");
            Player_v2.Instance.CallPlayerDeath();
        }
    }

    /// <summary>
    /// Returns the change in acceptanceValue based on which response style
    /// the player picked at choiceIndex.
    /// </summary>
    private float Evaluate(int responseIndex, int baseValue)
    {
        float rawDelta = 0.0f;
        float[] steps = new float[0];
        BoundaryFloat typeBound = new(0, 1);

        var type = (PlayerResponseStyle)responseIndex;
        float strength = Mathf.Clamp(baseValue / 4f, 0.25f, 1.0f);
        switch(type)
        {
            case PlayerResponseStyle.LogicalTone:
                typeBound = new(-0.10f, 0.90f);
                rawDelta = strength * topicInterestWeight
                    * (1f - confirmationBiasWeight) * (1f - criticalThinkingWeight) - 1.0f;
                steps = new float[] { -10f, 0f, 10f, 15f, 20f };
            break;
            case PlayerResponseStyle.ReservedTone:
                typeBound = new(-0.05f, 0.95f);
                rawDelta = strength * (1f - confirmationBiasWeight)
                    * (1f - criticalThinkingWeight) * topicInterestWeight - 0.05f;
                steps = new float[] { -10f, 0f, 10f };
            break;
            case PlayerResponseStyle.EmotionalTone:
                typeBound = new(-0.10f, 1.00f);
                rawDelta = strength * emotionalSusceptibilityWeight
                    * topicInterestWeight - (confirmationBiasWeight * 0.1f);
                steps = new float[] { -15f, -10f, 0f, 10f };
            break;
            case PlayerResponseStyle.ArgumentativeTone:
                typeBound = new(-1.00f, 0.10f);
                rawDelta = -strength * confirmationBiasWeight
                    * topicInterestWeight + strength * (1f - criticalThinkingWeight) * 0.1f;
                steps = new float[] { -20f, -15f, -10f, 0f, 10f, 15f };
            break;
        }
        return SnapToSteps(rawDelta, typeBound.lowerBound, typeBound.upperBound, steps);
    }

    private float SnapToSteps(float raw, float min, float max, float[] steps)
    {
        float t = (raw - min) / (max - min);
        t = Mathf.Clamp01(t);

        float floatIndex = t * (steps.Length-1);
        int idx = Mathf.RoundToInt(floatIndex);
        Debug.Log($"Raw: {raw/5} Min: {raw - min} and Max: {max - min} and T: {t} while index = {idx}");
        return steps[idx];
    }
}

[Serializable]
public abstract class OptionBase
{
    [field: Header("Parameters")]
    [field: SerializeField] public bool IsCorrectAnswer { get; protected set; }
    [field: SerializeField] [field: TextArea] public string Explanation { get; protected set; }

    public abstract string GetDisplayName();
}

[Serializable]
public class MalignOption : OptionBase
{
    [Header("Private Parameters")]
    [field: SerializeField] public MalignChecker Choice { get; private set; }
    public override string GetDisplayName() => Choice.ToString().Replace("_", " ");
}

[Serializable]
public class BiasOption : OptionBase
{
    [Header("Private Parameters")]
    [field: SerializeField] public BiasChecker Choice { get; private set; }
    public override string GetDisplayName()
    {
        string displayName = Choice.ToString().Replace("_", " ");
        displayName.Replace("X", " / ");

        return displayName;
    }
}

[Serializable]
public class SourceOption : OptionBase
{
    [Header("Private Parameters")]
    [field: SerializeField] public SourceChecker Choice { get; private set; }
    public override string GetDisplayName() => Choice.ToString().Replace("_", " ");
}
