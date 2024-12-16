using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [field: SerializeField] public string interactText {get; set;}

    [Header("Parameters")]
    [SerializeField] private TextAsset inkText;
    [SerializeField] private Sprite characterImage;
    public DialogueCharacterInformation characterInformation;

    private void Start()
    {
        characterInformation = Instantiate(characterInformation);
        characterInformation.Initialize(name, characterImage, EmotionState.Calm);
    }

    public string GetInteractText()
    {
        return "jjj";
    }

    public void Interact()
    {
        DialogueManager.Instance.HandleDialogue(characterInformation, inkText);
    }
}
