using UnityEngine;
using UnityEngine.UI;

public class BarSliderUI : MonoBehaviour
{
    private Slider barSlider;

    private void Awake()
    {
        barSlider = GetComponent<Slider>();
    }

    public virtual void SetCurrentValue(float value)
    {
        barSlider.value = value;
    }

    public virtual void SetMaxValue(float value)
    {
        barSlider.value = value;
        barSlider.maxValue = value;
    }
}
