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

        choiceButton.onClick.RemoveAllListeners();
        choiceButton.onClick.AddListener(() => MakeChoice(choiceIndex, uiPanel));
    }

    private void MakeChoice(int choiceIndex, DialogueUIPanel uiPanel)
    {
        DialogueManager dm = DialogueManager.Instance;
        dm.OnChoiceSelected(choiceIndex);

        uiPanel.DisableUIChoices();
        dm.canContinue = true;
    }
}
