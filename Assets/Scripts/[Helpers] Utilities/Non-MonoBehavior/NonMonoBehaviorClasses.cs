using System;
using UnityEngine;
using Action = System.Action;

[Serializable]
public struct BoundaryFloat
{
    public float lowerBound;
    public float upperBound;
    public BoundaryFloat(float min, float max) { lowerBound = min; upperBound = max; }
}

[Serializable]
public class QuestObjective
{
    [Header("Status")]
    [ReadOnly] public bool isDone;

    [Header("Objective Information")]
    public int targetValue;
    [ReadOnly] public int progressValue;
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
    public float AcceptanceValue { get; private set; } = 50f;

    [Header("Logical Parameters")]
    [Range(0f, 1f)][SerializeField] public float topicInterestWeight;
    [Range(0f, 1f)][SerializeField] private float criticalThinkingWeight;

    [Header("Emotional Bias Parameters")]
    [Range(0f, 1f)][SerializeField] private float confirmationBiasWeight;
    [Range(0f, 1f)][SerializeField] private float emotionalSusceptibilityWeight;

    public void InitializeAcceptanceValue(float value, BarSliderUI slider)
    {
        AcceptanceValue = value;
        slider.SetMaxValue(100, 50f);
    }

    /// <summary>
    /// Evaluates a dialogue choice and updates acceptance value.
    /// Fires EventBus.CharacterStat.OnPlayerTrustLost when acceptance drops to or below 25
    /// instead of calling QuestManager.Instance / Player_v2.Instance directly.
    /// A plain [Serializable] data class must never reach into scene singletons.
    /// </summary>
    public void Evaluate_AcceptanceValue(int responseIndex, int baseValue, BarSliderUI slider, out float delta)
    {
        delta = Evaluate(responseIndex, baseValue);
        AcceptanceValue = Mathf.Clamp(AcceptanceValue + delta, 0f, 100f);
        slider.SetCurrentValue(AcceptanceValue);

        if (AcceptanceValue <= 25f)
            EventBus.CharacterStat.OnPlayerTrustLost?.Invoke();
    }

    private float Evaluate(int responseIndex, int baseValue)
    {
        float rawDelta = 0f;
        float[] steps = Array.Empty<float>();
        var typeBound = new BoundaryFloat(0f, 1f);
        float strength = Mathf.Clamp(baseValue / 4f, 0.25f, 1f);

        switch ((PlayerResponseStyle)responseIndex)
        {
            case PlayerResponseStyle.LogicalTone:
                typeBound = new(-0.10f, 0.90f);
                rawDelta = strength * topicInterestWeight
                          * (1f - confirmationBiasWeight) * (1f - criticalThinkingWeight) - 1f;
                steps = new[] { -10f, 0f, 10f, 15f, 20f };
                break;
            case PlayerResponseStyle.ReservedTone:
                typeBound = new(-0.05f, 0.95f);
                rawDelta = strength * (1f - confirmationBiasWeight)
                          * (1f - criticalThinkingWeight) * topicInterestWeight - 0.05f;
                steps = new[] { -10f, 0f, 10f };
                break;
            case PlayerResponseStyle.EmotionalTone:
                typeBound = new(-0.10f, 1.00f);
                rawDelta = strength * emotionalSusceptibilityWeight
                          * topicInterestWeight - (confirmationBiasWeight * 0.1f);
                steps = new[] { -15f, -10f, 0f, 10f };
                break;
            case PlayerResponseStyle.ArgumentativeTone:
                typeBound = new(-1.00f, 0.10f);
                rawDelta = -strength * confirmationBiasWeight * topicInterestWeight
                          + strength * (1f - criticalThinkingWeight) * 0.1f;
                steps = new[] { -20f, -15f, -10f, 0f, 10f, 15f };
                break;
        }
        return SnapToSteps(rawDelta, typeBound.lowerBound, typeBound.upperBound, steps);
    }

    private float SnapToSteps(float raw, float min, float max, float[] steps)
    {
        if (steps.Length == 0) return 0f;
        float t = Mathf.Clamp01((raw - min) / (max - min));
        int idx = Mathf.RoundToInt(t * (steps.Length - 1));
        return steps[idx];
    }
}

[Serializable]
public abstract class OptionBase
{
    [field: Header("Parameters")]
    [field: SerializeField] public bool IsCorrectAnswer { get; protected set; }
    [field: SerializeField, TextArea] public string Explanation { get; protected set; }
    public abstract string GetDisplayName();
}

[Serializable]
public class MalignOption : OptionBase
{
    [field: SerializeField] public MalignChecker Choice { get; private set; }
    public override string GetDisplayName() => Choice.ToString().Replace("_", " ");
}

[Serializable]
public class BiasOption : OptionBase
{
    [field: SerializeField] public BiasChecker Choice { get; private set; }
    public override string GetDisplayName() =>
        Choice.ToString().Replace("_", " ").Replace("X", " / ");
}

[Serializable]
public class SourceOption : OptionBase
{
    [field: SerializeField] public SourceChecker Choice { get; private set; }
    public override string GetDisplayName() => Choice.ToString().Replace("_", " ");
}

// ── NotificationRequest ───────────────────────────────────────────────────────
// Typed notification data. All systems fire EventBus.Notification.OnShow with one
// of these — NoticePopup is the sole subscriber and handles all rendering.

public class NotificationRequest
{
    public NoticeType Type { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public Color TextColor { get; private set; } = Color.white;
    public float Duration { get; private set; }
    public QuestSO Quest { get; private set; }
    public QuestObjective Objective { get; private set; }
    public Action PrimaryAction { get; private set; }
    public Action SecondaryAction { get; private set; }
    public string PrimaryLabel { get; private set; }
    public string SecondaryLabel { get; private set; }
    public int CoinCost { get; private set; }

    private NotificationRequest() { }

    public static NotificationRequest QuestBanner(float delay, QuestSO quest) => new()
    {
        Type = NoticeType.QuestCompleted,
        Quest = quest,
        Duration = delay
    };

    public static NotificationRequest ObjectiveBanner(float delay, QuestObjective obj) => new()
    {
        Type = NoticeType.ObjectiveCompleted,
        Objective = obj,
        Duration = delay
    };

    public static NotificationRequest Dialogue(Color color, string body) => new()
    {
        Type = NoticeType.Dialogue,
        Body = body,
        TextColor = color
    };

    public static NotificationRequest QuizResult(NoticeType resultType, string explanation) => new()
    {
        Type = resultType,
        Body = explanation
    };

    public static NotificationRequest Hint(string hintText) => new()
    {
        Type = NoticeType.Hint,
        Body = hintText
    };

    public static NotificationRequest MiniGameOver(string reason, Action restart, Action quit) => new()
    {
        Type = NoticeType.GameOver,
        Body = reason,
        PrimaryAction = restart,
        PrimaryLabel = "Restart",
        SecondaryAction = quit,
        SecondaryLabel = "Quit Game"
    };

    public static NotificationRequest Payment(int cost, string body, Action onPaid) => new()
    {
        Type = NoticeType.Payment,
        Body = body,
        CoinCost = cost,
        PrimaryAction = onPaid,
        PrimaryLabel = $"Buy For {cost}",
        SecondaryLabel = "Exit"
    };

    public static NotificationRequest Confirm(string body, Action accept, Action reject,
        string acceptLabel = "Confirm", string rejectLabel = "Cancel") => new()
        {
            Type = NoticeType.Confirm,
            Body = body,
            PrimaryAction = accept,
            PrimaryLabel = acceptLabel,
            SecondaryAction = reject,
            SecondaryLabel = rejectLabel
        };
}

public class ReadOnlyAttribute : PropertyAttribute
{

}