using UnityEngine;
using Ink.Runtime;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    private bool pickedChoice;
    private WaitForSeconds exitPanelSeconds;
    public NPC NPCharacter { get; private set; }
    public static DialogueManager Instance { get; private set; }

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
        dialogueIsPlaying = false;
        dialogueUIPanel.ExitPanel();
        exitPanelSeconds = new WaitForSeconds(0.2f);
        OnDialogueStart.AddListener(() => ObserveInkVariable());
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        if(skipDialogue)
        {
            Debug.Log("A");
            StartCoroutine(ExitDialogueMode());
        }

        if (Player_v2.Instance.InputHandler.jumpPressed && canContinue && currentDialogueStory.currentChoices.Count == 0)
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

    public void ContinueDialogueStory()
    {
        if (currentDialogueStory == null)
        {
            return;
        }
        int safeCounter = 0;
        const int maxSkips = 10;

        while (currentDialogueStory.canContinue && safeCounter < maxSkips)
        {
            dialogueUIPanel.StopDisplayCoroutine();
            string text = currentDialogueStory.Continue();

            CheckWhoIsSpeaking(currentDialogueStory.currentTags);
            if (string.IsNullOrWhiteSpace(text))
            {
                safeCounter++;
                if (currentDialogueStory.currentChoices.Count > 0)
                {
                    break;
                }
                continue;
            }

            if (currentSpeaker != null)
            {
                dialogueUIPanel.DisplayChoicesUI(currentDialogueStory);
                dialogueUIPanel.DisplayText(currentSpeaker, text, NPCharacter?.Profile);
            }
            HandleConclusion();
            return;
        }

        if (!currentDialogueStory.canContinue)
        {
            Debug.Log("B");
            StartCoroutine(ExitDialogueMode());
        }
    }


    public void FastForwardTo(string knotName)
    {
        if (currentDialogueStory == null)
        {
            Debug.LogWarning("No active story to fast‑forward.");
            return;
        }
        currentDialogueStory.ChoosePathString(knotName);
        dialogueUIPanel.DisableUIChoices();
        ContinueDialogueStory();
    }

    private void ObserveInkVariable()
    {
        string baseValue = "baseValue";
        if (currentDialogueStory.variablesState.GlobalVariableExistsWithName(baseValue) != true)
        {
            return;
        }
        currentDialogueStory.ObserveVariable(baseValue, (variableName, newValue) => { EvaluateSpecialNPC_Choice(variableName); });
    }

    private void EvaluateSpecialNPC_Choice(string baseValueStr)
    {
        if (NPCharacter == null || NPCharacter.TypeOfNPC != NPCType.Special)
        {
            return;
        }
        int baseValue = (int)currentDialogueStory.variablesState[baseValueStr];
        int responseIndex = (int)currentDialogueStory.variablesState["responseIndex"];
        NPCharacter.Profile.Evaluate_AcceptanceValue(responseIndex, baseValue, NPCharacter.SliderUI, out float delta);
        currentDialogueStory.variablesState["lastDelta"] = delta;
    }

    private void HandleConclusion()
    {
        string currentPath = currentDialogueStory.state.currentPathString;
        if (currentPath != null && currentPath.ToLower().Contains("finalpush"))
        {
            float acceptanceValue = NPCharacter.Profile.AcceptanceValue;
            if (acceptanceValue >= 65)
            {
                FastForwardTo("Convinced");
                return;
            }
            else if (acceptanceValue < 30)
            {
                FastForwardTo("Rejected");
                return;
            }
        }
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return exitPanelSeconds;
        dialogueIsPlaying = false;

        currentDialogueStory = null;
        dialogueUIPanel.ExitPanel();

        print(skipDialogue);
        if (skipDialogue != true) 
        {
            Debug.Log(66);
            UpdateObective();
        }
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

        if(NPCharacter.hasCompletedDialogue || NPCharacter.TypeOfNPC != NPCType.Special)
        {
            return;
        }

        QuestManager questManager = QuestManager.Instance;
        float acceptanceValue = NPCharacter.Profile.AcceptanceValue;
        bool notFullyConvinced = acceptanceValue <= 65.0f && acceptanceValue > 25.0f;
        if (notFullyConvinced && CheckIfSpecialNPC())
        {
            questManager.popupPanel.DialoguePopup(Color.white, $"Failed To Convince {NPCharacter.name}, Try Again !!!");
            return;
        }

        QuestSO quest = questManager.activeQuest;
        if (quest != null)
        {
            QuestObjectives objective = quest.FindQuestObjective(ObjectiveType.ConvinceNPC, true);
            if (objective != null && objective.isDone != true)
            {
                NPCharacter.Identifier.MarkCompleted();
                NPCharacter.hasCompletedDialogue = true;
                quest.IncreaseQuestObjectiveProgressLevels(objective, NPCharacter.Identifier);
                questManager.popupPanel.DialoguePopup(Color.green, $"Convinced {NPCharacter.name} Succesfully");
            }
        }
    }

    private bool CheckIfSpecialNPC()
    {
        return (NPCharacter.dialogueTrigger != null) && NPCharacter.dialogueTrigger.SpeakerType.Equals(TypeOfSpeaker.NPC);
    }

    private void CheckWhoIsSpeaking(List<string> currentTag)
    {
        if(pickedChoice)
        {
            currentSpeakerType = SpeakerType.Player;
            currentSpeaker = playableCharacterSpeaker;
        }
        else
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
        pickedChoice = false;
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
        pickedChoice = true;
        currentDialogueStory.ChooseChoiceIndex(choiceIndex);
        ContinueDialogueStory();
    }
}
