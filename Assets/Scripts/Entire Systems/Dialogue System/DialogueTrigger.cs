using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    private NPC character;
    public TextAsset currentDialogueText;
    [SerializeField] private string interactText = "Talk to Agency Cheif";

    [Header("Parameters")]
    [SerializeField] private Sprite characterImage;
    [SerializeField] private TypeOfSpeaker speakerType;
    public DialogueCharacterInformation characterInformation;
    [SerializeField] private List<TextAsset> dialogueTexts = new();

    private void Awake()
    {
        character = GetComponent<NPC>();
    }

    private void Start()
    {
        characterInformation = Instantiate(characterInformation);
        characterInformation.Initialize(name, characterImage, speakerType, EmotionState.Calm);
    }

    public string GetInteractText()
    {
        return interactText;
    }

    public void Interact(Player_v2 player)
    {
        PlayInteractNPCSound();
        TriggerDialogue(character, dialogueTexts[0]);
    }

    public void PlayInteractNPCSound()
    {

    }

    private void TriggerDialogue(NPC npc = null, TextAsset textAsset = null)
    {
        if (characterInformation.speakerType == TypeOfSpeaker.Instructor)
        {
            QuestManager questManager = QuestManager.Instance;

            int i = questManager.availableQuests.IndexOf(questManager.activeQuest);
            print(i + " character trigger is " + this);
            DialogueManager.Instance.HandleDialogue(characterInformation, dialogueTexts[i]);
            return;
        }
        TextAsset randomDialogue = textAsset;
        DialogueManager.Instance.HandleDialogue(characterInformation, randomDialogue, npc);
    }
}
