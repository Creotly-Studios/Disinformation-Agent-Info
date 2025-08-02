using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameOptionButton : MonoBehaviour
{
    public OptionBase Option { get; private set; }

    [Header("Parameters")]
    [SerializeField] private bool isCorrect;
    [field: SerializeField] public Button optionButton { get; private set; }
    [SerializeField] private TextMeshProUGUI optionText;

    public bool IsCorrect()
    {
        return isCorrect;
    }

    public void Initialize(OptionBase option, ObjectiveType objType)
    {
        Option = option;

        isCorrect = option.IsCorrectAnswer;
        if (objType != ObjectiveType.MiniGame_MalignInfluence)
        {
            optionButton.image.color = Color.white;
        }
        optionText.text = option.GetDisplayName();
    }
}
