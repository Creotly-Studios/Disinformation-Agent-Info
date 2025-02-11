using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    //Parameters
    private List<TextAsset> dialogueTexts = new();
    public TextAsset instructionDialogue;

    [field: Header("Parameters")]
    public bool allQuestCompleted { get; private set; }
    [field: SerializeField] public QuestSO activeQuest;
    [field: SerializeField] public List<QuestSO> availableQuests { get; private set; } = new List<QuestSO>();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for(int i = 0; i < availableQuests.Count; i++)
        {
            availableQuests[i] = Instantiate(availableQuests[i]);
        }
    }

    public void SetMiniGame(ComputerPanel_UI computerPanel)
    {
        
    }

    public void TriggerDialogue(DialogueCharacterInformation speaker, NPC npc = null, TextAsset textAsset = null)
    {
        if(speaker.speakerType == TypeOfSpeaker.Instructor)
        {
            DialogueManager.Instance.HandleDialogue(speaker, instructionDialogue);
            return;
        }
        TextAsset randomDialogue = textAsset;
        DialogueManager.Instance.HandleDialogue(speaker, randomDialogue, npc);
    }

    private TextAsset PickRandomDialogue()
    {
        int random = Random.Range(0, dialogueTexts.Count);
        return dialogueTexts[random];
    }

    public void Quest_Update()
    {
        if (DialogueManager.Instance.dialogueIsPlaying)
        {
            return;
        }

        if (allQuestCompleted)
        {
            return;
        }
        AssignQuests();
    }

    public void AssignQuests()
    {
        if(activeQuest == null || activeQuest.isComplete == true)
        {
            QuestSO questSO = availableQuests.Find(x => x.isComplete != true);
            if (questSO == null)
            {
                allQuestCompleted = true;
                return;
            }

            dialogueTexts.Clear();
            activeQuest = questSO;
            dialogueTexts.AddRange(activeQuest.dialogueTexts);
            instructionDialogue = activeQuest.instructionDialogue;
            return;
        }
        activeQuest.QuestSO_Update();
    }

    public TextAsset RandomDialogueText()
    {
        int random = Random.Range(0, dialogueTexts.Count);
        TextAsset textAsset = dialogueTexts[random];

        dialogueTexts.Remove(textAsset);
        return textAsset;
    }
}
