using UnityEngine;
using UnityEngine.Events;
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private WaitForSeconds exitPanelSeconds;
    public NPC NPCharacter { get; private set; }
    
    // Status
    public bool canContinue;
    public bool skipDialogue;
    public bool dialogueIsPlaying { get; private set; }
    public Story currentDialogueStory { get; private set; }
    public SpeakerType currentSpeakerType { get; private set; }

    // Tags
    private const string PLAYER_TAG = "Player";

    [Header("Dialogue Parameters")]
    [SerializeField] private DialogueUIPanel dialogueUIPanel;

    [Header("Active Speakers")]
    private DialogueCharacterInformation activeSpeakers;
    private DialogueCharacterInformation playableCharacterSpeaker;
    public DialogueCharacterInformation currentSpeaker {get; private set;}

    [Header("Events")]
    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;

    //Player
    Player_v2 player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("A duplicate DialogueManager was found and destroyed.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        player = GameObject.Find("Player_v2").GetComponent<Player_v2>();
        dialogueIsPlaying = false;
        dialogueUIPanel.ExitPanel();
        exitPanelSeconds = new WaitForSeconds(0.2f);
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        if(skipDialogue)
        {
            StartCoroutine(ExitDialogueMode());
        }

        if (InputManager.instance.jumpPressed && canContinue && currentDialogueStory.currentChoices.Count == 0)
        {
            ContinueDialogueStory();
        }
    }

    public void EnableDialoguePanel(bool status)
    {
        dialogueUIPanel.gameObject.SetActive(status);
    }

    public void SetPlayerSpeaker(DialogueCharacterInformation speaker)
    {
        playableCharacterSpeaker = speaker;
    }

    public void HandleDialogue(DialogueCharacterInformation speaker, TextAsset inkJsonStory, NPC npc = null)
    {
        activeSpeakers = speaker;
        if(npc != null)
        {
            NPCharacter = npc;
        }

        dialogueIsPlaying = true;
        dialogueUIPanel.gameObject.SetActive(true);
        dialogueUIPanel.DisableUIChoices();
        currentDialogueStory = new Story(inkJsonStory.text);

        // Trigger the "On Dialogue Start" UnityEvent
        OnDialogueStart?.Invoke();

        ContinueDialogueStory();
    }

    private void ContinueDialogueStory()
    {
        if (currentDialogueStory.canContinue)
        {
            dialogueUIPanel.StopDisplayCoroutine();

            string text = currentDialogueStory.Continue();
            CheckWhoIsSpeaking(currentDialogueStory.currentTags);

            if (text.Equals("") && !currentDialogueStory.canContinue)
            {
                StartCoroutine(ExitDialogueMode());
            }

            if (currentSpeaker != null)
            {
                currentDialogueStory.variablesState["npcEmotion"] = currentSpeaker.currentEmotion.ToString();
                dialogueUIPanel.DisplayChoicesUI(currentDialogueStory);
                dialogueUIPanel.DisplayText(currentSpeaker, text);
            }
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return exitPanelSeconds;
        dialogueIsPlaying = false;

        currentDialogueStory = null;
        dialogueUIPanel.ExitPanel();
        if (skipDialogue != true) { UpdateObective(); }

        // Trigger the "On Dialogue End" UnityEvent
        OnDialogueEnd?.Invoke();
        
        if (Player_v2.Instance != null)
        {
            Player_v2.Instance.SetActiveState();
        }

        yield return null;
        skipDialogue = false;
    }

    private void UpdateObective()
    {
        if(NPCharacter == null)
        {
            return;
        }

        if(NPCharacter.hasCompletedDialogue || NPCharacter.npcType != NPCType.Special)
        {
            return;
        }

        QuestObjectives objective = QuestManager.Instance.FindQuestObjective(ObjectiveType.ConvinceNPC);
        if (objective != null && objective.isDone != true)
        {
            NPCharacter.hasCompletedDialogue = true;
            QuestSO quest = QuestManager.Instance.activeQuest;
            quest.IncreaseQuestObjectiveProgressLevels(objective, NPCharacter.identifier);
        }
    }

    private void CheckWhoIsSpeaking(List<string> currentTag)
    {
        foreach (string tag in currentTag)
        {
            string[] splitTag = tag.Split(':');

            if (splitTag.Length != 2)
            {
                Debug.LogError("Error Parsing Tag: " + tag);
                return;
            }
            string tagValue = splitTag[1].Trim();
            SetCharacter(tagValue);
        }
    }

    private void SetCharacter(string tagValue)
    {
        if (tagValue.Equals(PLAYER_TAG))
        {
            currentSpeakerType = SpeakerType.Player;
            currentSpeaker = playableCharacterSpeaker;
            return;
        }
        currentSpeaker = activeSpeakers;
        currentSpeakerType = SpeakerType.Other;
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        currentDialogueStory.ChooseChoiceIndex(choiceIndex);
        ContinueDialogueStory();
    }
}
