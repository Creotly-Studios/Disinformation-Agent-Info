using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    private NPC character;
    [field: SerializeField] public string interactText {get; set;}

    [Header("Parameters")]
    [SerializeField] private TextAsset inkText;
    [SerializeField] private Sprite characterImage;
    [SerializeField] private TypeOfSpeaker speakerType;
    public DialogueCharacterInformation characterInformation;

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
        return "jjj";
    }

    public void Interact(Player_v2 player)
    {
        PlayInteractNPCSound();
        QuestManager.Instance.TriggerDialogue(characterInformation, character, inkText);
    }

    public void PlayInteractNPCSound()
    {
        SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.sfxList.interactWithNpc);
    }
}
