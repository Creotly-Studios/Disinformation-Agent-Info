using TMPro;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;

public class DialogueUIChoice : MonoBehaviour
{
    public Button choiceButton;
    private Choice dialoguechoice;
    public TextMeshProUGUI choiceText;

    public void Initialize(int choiceIndex, Choice choice, DialogueUIPanel uiPanel)
    {
        dialoguechoice = choice;
        choiceText.text = dialoguechoice.text;
        choiceButton.onClick.AddListener(() => MakeChoice(choiceIndex, uiPanel));
    }

    private void MakeChoice(int choiceIndex, DialogueUIPanel uiPanel)
    {
        DialogueManager.Instance.OnChoiceSelected(choiceIndex);
        uiPanel.DisableUIChoices();
    }
}
