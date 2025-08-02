using UnityEngine;
using UnityEngine.UI;

public class BarSliderUI : MonoBehaviour
{
    private Slider barSlider;
    [SerializeField] private bool isNPCSlider;

    [Header("Slider Visualizers")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient warmingUpIndicator;

    protected void Awake()
    {
        barSlider = GetComponent<Slider>();
        fillImage = barSlider.fillRect.GetComponent<Image>();
    }

    public void SetCurrentValue(float value)
    {
        barSlider.value = value;
        if (isNPCSlider)
        {
            fillImage.color = warmingUpIndicator.Evaluate(barSlider.normalizedValue);
        }
    }

    /// <summary>
    /// Set Max And Starting Value, best used for only regressing sliders, e.g Health Bars
    /// </summary>
    /// <param name="value">Value to Initialize as Max and Current Value</param>
    public void SetMaxValue(float value)
    {
        barSlider.value = value;
        barSlider.maxValue = value;
    }

    /// <summary>
    /// Set Starting Value and Set Max Value
    /// </summary>
    /// <param name="max">Max Value On Slider</param>
    /// <param name="value">Current Or Starting Value For Slider</param>
    public void SetMaxValue(float max, float value)
    {
        barSlider.maxValue = max;
        SetCurrentValue(value);
    }
}
