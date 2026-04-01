using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    private NPC character;
    public TextAsset currentDialogueText;

    [SerializeField] private string interactText = "Talk to Agency Chief";

    [Header("Parameters")]
    [SerializeField] private Sprite characterImage;
    [SerializeField] private TypeOfSpeaker speakerType;
    [SerializeField] private List<TextAsset> dialogueTexts = new();

    public DialogueCharacterInformation characterInformation;
    public TypeOfSpeaker SpeakerType => speakerType;

    private void Awake() => character = GetComponent<NPC>();

    private void Start()
    {
        characterInformation = Instantiate(characterInformation);
        characterInformation.Initialize(name, characterImage, speakerType);
    }

    public string GetInteractText() => interactText;

    public void Interact(Player_v2 player)
    {
        PlayInteractNPCSound();
        TriggerDialogue(character);
        player.SetInactiveState();
    }

    public void PlayInteractNPCSound() { }

    private void TriggerDialogue(NPC npc = null)
    {
        if (characterInformation.speakerType == TypeOfSpeaker.Instructor)
        {
            QuestManager questManager = QuestManager.Instance;
            // Fix: was questManager.activeQuest (private [SerializeField]) → ActiveQuest property.
            int i = questManager.AvailableQuests.IndexOf(questManager.ActiveQuest);
            DialogueManager.Instance.HandleDialogue(characterInformation, dialogueTexts[i], npc);
            return;
        }
        currentDialogueText = dialogueTexts[Random.Range(0, dialogueTexts.Count)];
        DialogueManager.Instance.HandleDialogue(characterInformation, currentDialogueText, npc);
    }
}