using UnityEngine;
using Ink.Runtime;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private bool pickedChoice;
    private WaitForSeconds exitDelay;

    public NPC NPCharacter { get; private set; }
    public bool CanContinue { get; set; }
    public bool SkipDialogue { get; set; }
    public bool DialogueIsPlaying { get; private set; }
    public Story CurrentDialogueStory { get; private set; }
    public SpeakerType CurrentSpeakerType { get; private set; }
    public DialogueCharacterInformation CurrentSpeaker { get; private set; }

    private const string PLAYER_TAG = "Player";

    [Header("Dialogue Parameters")]
    [SerializeField] private DialogueUIPanel dialogueUIPanel;

    [Header("Notification")]
    [SerializeField] private NoticePopup dialoguePopup;

    private DialogueCharacterInformation activeSpeakers;
    private DialogueCharacterInformation playerSpeaker;

    [Header("Events")]
    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        { 
            Destroy(gameObject);
            return;
        }
        Instance = this;
        dialoguePopup.SubscribeEvents();
    }

    private void OnDestroy()
    {
        if (dialoguePopup != null)
        {
            dialoguePopup.UnSubscribeEvents();
        }
    }

    private void Start()
    {
        DialogueIsPlaying = false;
        dialogueUIPanel.ExitPanel();
        exitDelay = new WaitForSeconds(0.2f);
        OnDialogueStart.AddListener(ObserveInkVariable);
    }

    private void OnEnable() => EventBus.CharacterStat.OnPlayerTrustLost += OnPlayerTrustLost;
    private void OnDisable() => EventBus.CharacterStat.OnPlayerTrustLost -= OnPlayerTrustLost;

    private void Update()
    {
        if (!DialogueIsPlaying) return;
        if (SkipDialogue) { StartCoroutine(ExitDialogueMode()); return; }
        if (Player_v2.Instance.InputHandler.spaceBarPressed
            && CanContinue
            && CurrentDialogueStory.currentChoices.Count == 0)
            ContinueDialogueStory();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void SetPlayerSpeaker(DialogueCharacterInformation speaker) => playerSpeaker = speaker;
    public void EnableDialoguePanel(bool status) => dialogueUIPanel.gameObject.SetActive(status);

    public void HandleDialogue(DialogueCharacterInformation speaker, TextAsset script, NPC npc = null)
    {
        activeSpeakers = speaker;
        if (npc != null) NPCharacter = npc;

        DialogueIsPlaying = true;
        dialogueUIPanel.gameObject.SetActive(true);
        dialogueUIPanel.DisableUIChoices();
        CurrentDialogueStory = new Story(script.text);
        OnDialogueStart?.Invoke();
        ContinueDialogueStory();
    }

    public void ContinueDialogueStory()
    {
        if (CurrentDialogueStory == null) return;

        int safeGuard = 0;
        while (CurrentDialogueStory.canContinue && safeGuard < 10)
        {
            dialogueUIPanel.StopDisplayCoroutine();
            string text = CurrentDialogueStory.Continue();
            CheckWhoIsSpeaking(CurrentDialogueStory.currentTags);

            if (string.IsNullOrWhiteSpace(text))
            {
                safeGuard++;
                if (CurrentDialogueStory.currentChoices.Count > 0) break;
                continue;
            }

            if (CurrentSpeaker != null)
            {
                dialogueUIPanel.DisplayChoicesUI(CurrentDialogueStory);
                dialogueUIPanel.DisplayText(CurrentSpeaker, text, NPCharacter.Profile);
            }
            HandleConclusion();
            return;
        }

        if (!CurrentDialogueStory.canContinue) StartCoroutine(ExitDialogueMode());
    }

    public void FastForwardTo(string knotName)
    {
        if (CurrentDialogueStory == null) { Debug.LogWarning("No active story."); return; }
        CurrentDialogueStory.ChoosePathString(knotName);
        dialogueUIPanel.DisableUIChoices();
        ContinueDialogueStory();
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        pickedChoice = true;
        CurrentDialogueStory.ChooseChoiceIndex(choiceIndex);
        ContinueDialogueStory();
    }

    // ── Ink Variables ─────────────────────────────────────────────────────────

    private void ObserveInkVariable()
    {
        const string key = "baseValue";
        if (!CurrentDialogueStory.variablesState.GlobalVariableExistsWithName(key)) return;
        CurrentDialogueStory.ObserveVariable(key,
            (varName, _) => EvaluateSpecialNPC_Choice(varName));
    }

    private void EvaluateSpecialNPC_Choice(string baseValueKey)
    {
        if (NPCharacter.TypeOfNPC != NPCType.Special)
        {
            return;
        }
        int baseValue = (int)CurrentDialogueStory.variablesState[baseValueKey];
        int responseIndex = (int)CurrentDialogueStory.variablesState["responseIndex"];
        NPCharacter.Profile.Evaluate_AcceptanceValue(responseIndex, baseValue, NPCharacter.SliderUI, out float delta);
        CurrentDialogueStory.variablesState["lastDelta"] = delta;
    }

    private void HandleConclusion()
    {
        string path = CurrentDialogueStory.state.currentPathString;
        if (path == null || !path.ToLower().Contains("finalpush")) return;
        float v = NPCharacter.Profile.AcceptanceValue;
        if (v >= 65) FastForwardTo("Convinced");
        else if (v < 30) FastForwardTo("Rejected");
    }

    // ── Exit & Objective ──────────────────────────────────────────────────────

    private IEnumerator ExitDialogueMode()
    {
        yield return exitDelay;
        DialogueIsPlaying = false;
        CurrentDialogueStory = null;
        dialogueUIPanel.ExitPanel();
        if (!SkipDialogue) UpdateObjective();
        OnDialogueEnd?.Invoke();
        Player_v2.Instance.SetActiveState();
        yield return null;
        SkipDialogue = false;
    }

    private void UpdateObjective()
    {
        if (NPCharacter == null) return;
        if (NPCharacter.hasCompletedDialogue || NPCharacter.TypeOfNPC != NPCType.Special) return;

        float acceptance = NPCharacter.Profile.AcceptanceValue;
        bool notFullyConvinced = acceptance is <= 65f and > 25f;

        if (notFullyConvinced && IsNPCInteractable())
        {
            Notify(Color.white, $"Failed To Convince {NPCharacter.name} — Try Again!");
            return;
        }

        QuestSO quest = QuestManager.Instance.ActiveQuest;
        if (quest == null) return;

        EventBus.Quest.OnQuestObjectiveCompleted?.Invoke(true, true, ObjectiveType.ConvinceNPC, null);

        QuestObjective objective = quest.FindQuestObjective(ObjectiveType.ConvinceNPC);
        if (objective != null && !objective.isDone)
        {
            NPCharacter.Identifier.MarkCompleted();
            NPCharacter.hasCompletedDialogue = true;
            Notify(Color.green, $"Convinced {NPCharacter.name} Successfully!");
        }
    }

    private void OnPlayerTrustLost()
    {
        if (NPCharacter == null) return;
        Notify(Color.red, "Has Completely Lost NPC's Trust");
        Player_v2.Instance.CallPlayerDeath();
    }

    private bool IsNPCInteractable() =>
        NPCharacter.dialogueTrigger != null
        && NPCharacter.dialogueTrigger.SpeakerType == TypeOfSpeaker.NPC;

    private void Notify(Color color, string body) =>
        EventBus.Notification.OnShow?.Invoke(dialoguePopup, NotificationRequest.Dialogue(color, body));

    // ── Speaker Resolution ────────────────────────────────────────────────────

    private void CheckWhoIsSpeaking(List<string> tags)
    {
        if (pickedChoice)
        {
            CurrentSpeakerType = SpeakerType.Player;
            CurrentSpeaker = playerSpeaker;
            pickedChoice = false;
            return;
        }

        foreach (string tag in tags)
        {
            string[] parts = tag.Split(':');
            if (parts.Length != 2) { Debug.LogError("Malformed dialogue tag: " + tag); return; }
            SetSpeaker(parts[1].Trim());
        }
        pickedChoice = false;
    }

    private void SetSpeaker(string tagValue)
    {
        if (tagValue.Equals(PLAYER_TAG))
        {
            CurrentSpeakerType = SpeakerType.Player;
            CurrentSpeaker = playerSpeaker;
            return;
        }
        CurrentSpeaker = activeSpeakers;
        CurrentSpeakerType = SpeakerType.Other;
    }
}
