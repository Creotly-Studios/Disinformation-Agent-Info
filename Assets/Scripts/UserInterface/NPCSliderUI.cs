using UnityEngine;
using UnityEngine.UI;

public class NPCSliderUI : BarSliderUI
{
    [Header("SerializeField")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient warmingUpIndicator;

    protected override void Awake()
    {
        base.Awake();
        fillImage = barSlider.fillRect.GetComponent<Image>();
    }

    public override void SetCurrentValue(float value)
    {
        barSlider.value = value;
        fillImage.color = warmingUpIndicator.Evaluate(barSlider.normalizedValue);
    }

    public override void SetMaxValue(float value)
    {
        barSlider.value = value;
        barSlider.maxValue = value;
        fillImage.color = warmingUpIndicator.Evaluate(1f);
    }
}
