using UnityEngine;
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private WaitForSeconds exitPanelSeconds;

    // Status
    public bool canContinue;
    public bool dialogueIsPlaying { get; private set; }
    public Story currentDialogueStory { get; private set; }
    public SpeakerType currentSpeakerType { get; private set; }

    //Tags
    private const string PLAYER_TAG = "Player";
    
    [Header("Dialogue Parameters")]
    [SerializeField] private DialogueUIPanel dialogueUIPanel;

    [Header("Active Speakers")]
    private DialogueCharacterInformation currentSpeaker;
    private DialogueCharacterInformation playableCharacterSpeaker;
    private List<DialogueCharacterInformation> activeSpeakers = new List<DialogueCharacterInformation>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Cannot have more than one DialogueManager. {Instance} already exists.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
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

        if(InputManager.instance.jumpPressed && canContinue && currentDialogueStory.currentChoices.Count == 0)
        {
            ContinueDialogueStory();
        }
    }

    public void SetPlayerSpeaker(DialogueCharacterInformation speaker)
    {
        playableCharacterSpeaker = speaker;
    }

    public void HandleDialogue(DialogueCharacterInformation speaker, TextAsset inkJsonStory)
    {
        if (!activeSpeakers.Contains(speaker))
        {
            activeSpeakers.Add(speaker);
        }

        dialogueIsPlaying = true;
        dialogueUIPanel.gameObject.SetActive(true);
        dialogueUIPanel.DisableUIChoices();
        currentDialogueStory = new Story(inkJsonStory.text);
        ContinueDialogueStory();
    }

    private void ContinueDialogueStory()
    {
        if (currentDialogueStory.canContinue)
        {
            dialogueUIPanel.StopDisplayCoroutine();

            string text = currentDialogueStory.Continue();
            CheckWhoIsSpeaking(currentDialogueStory.currentTags);

            if(text.Equals("") && currentDialogueStory.canContinue != true)
            {
                StartCoroutine(ExitDialogueMode());
            }

            if (currentSpeaker != null)
            {
                if(currentDialogueStory.currentTags.Contains("stage:Gameplay"))
                {
                    StartCoroutine(ExitDialogueMode());
                    //TriggerGamePlay();
                    return;
                }
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

        activeSpeakers.Clear();
        dialogueIsPlaying = false;

        currentDialogueStory = null;
        dialogueUIPanel.ExitPanel();
    }

    private void CheckWhoIsSpeaking(List<string> currentTag)
    {
        foreach (string tag in currentTag)
        {
            string[] splitTag = tag.Split(':');

            if(splitTag.Length != 2 )
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
        if(tagValue.Equals(PLAYER_TAG))
        {
            currentSpeakerType = SpeakerType.Player;
            currentSpeaker = playableCharacterSpeaker;
            return;
        }
        currentSpeaker = PickRandomSpeaker();
        currentSpeakerType = SpeakerType.Other;
    }

    private DialogueCharacterInformation PickRandomSpeaker()
    {
        if (activeSpeakers.Count > 0)
        {
            int randomIndex = Random.Range(0, activeSpeakers.Count);
            return activeSpeakers[randomIndex];
        }
        return null;
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        currentDialogueStory.ChooseChoiceIndex(choiceIndex);
        ContinueDialogueStory();
    }
}
