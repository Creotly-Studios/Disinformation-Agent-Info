using UnityEngine;

[CreateAssetMenu(fileName = "DialogueCharacterInformation", menuName = "Creotly Studio/DialogueCharacterInformation")]
public class DialogueCharacterInformation : ScriptableObject
{
    [field: Header("Character Information")]
    [field: SerializeField] public string characterName { get; private set; }
    [field: SerializeField] public Sprite characterImage { get; private set; }
    [field: SerializeField] public TypeOfSpeaker speakerType { get; private set; }
    [field: SerializeField] public EmotionState currentEmotion { get; private set; }

    public void SetEmotionState(EmotionState state)
    {
        currentEmotion = state;
    }

    public void Initialize(string name, Sprite image, TypeOfSpeaker speakerType, EmotionState emotion)
    {
        characterName = name;
        characterImage = image;
        currentEmotion = emotion;
        this.speakerType = speakerType;
    }
}
