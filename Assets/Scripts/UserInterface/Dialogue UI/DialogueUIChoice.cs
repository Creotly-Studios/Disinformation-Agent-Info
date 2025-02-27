using TMPro;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;

public class DialogueUIChoice : MonoBehaviour
{
    public Response response;
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

        NPC npc = DialogueManager.Instance.NPCharacter;
        if(npc != null) { npc.UpdateWarmRadar(response); }
        uiPanel.DisableUIChoices();
    }
}
