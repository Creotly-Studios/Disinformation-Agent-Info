using TMPro;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DialogueUIPanel : MonoBehaviour
{
    [Header("Panel")]
    public Transform dialoguePanel;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button continueButton;

    [Header("Speaker Panel")]
    public Image speakerImage;
    public GameObject speakerObject;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI speakerTextDialogue;

    [Header("Choices")]
    [SerializeField] private Color playerTextColor;
    [SerializeField] private Transform choicesDrawer;
    [SerializeField] private List<DialogueUIChoice> choicesUIList;

    private Coroutine typingCoroutine;
    private WaitForSeconds typingSpeed;
    private static DialogueUIPanel Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("A duplicate DialogueManager was found and destroyed.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        typingSpeed = new WaitForSeconds(0.0035f);

        if(skipButton != null) skipButton.onClick.AddListener(HandleSkip);
    }

    public void HandleSkip()
    {
        DialogueManager.Instance.skipDialogue = true;
    }

    public void StopDisplayCoroutine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
    }

    public void DisplayText(DialogueCharacterInformation speaker, string dialogueText, NPC_CharacterProfile profile)
    {
        DialogueManager dialogueManager = DialogueManager.Instance;
        SpeakerType currentSpeakerType = dialogueManager.currentSpeakerType;

        speakerObject.SetActive(true);
        speakerTextDialogue.color = (currentSpeakerType != SpeakerType.Player) ? TextColor(profile) : playerTextColor;

        speakerName.text = speaker.characterName;
        if(dialogueManager.currentSpeaker.speakerType == TypeOfSpeaker.Instructor)
        {
            speakerTextDialogue.color = Color.green;
        }
        HandleTextTyping(dialogueText);
    }

    public Color TextColor(NPC_CharacterProfile profile)
    {
        if(profile.AcceptanceValue < 50.0f)
        {
            return Color.red;
        }
        else if(profile.AcceptanceValue > 50.0f)
        {
            return Color.green;
        }
        return Color.white;
    }

    public void DisableUIChoices()
    {
        foreach(var choicesUI in choicesUIList)
        {
            choicesUI.gameObject.SetActive(false);
        }
    }

    public void DisplayChoicesUI(Story story)
    {
        DisableUIChoices();
        if (story.currentChoices.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < story.currentChoices.Count; i++)
        {
            Choice choice = story.currentChoices[i];
            DialogueUIChoice choiceUI = choicesUIList[i];

            choiceUI.Initialize(i, choice, this);
            choiceUI.gameObject.SetActive(true);
        }
        StartCoroutine(SelectFirstChoice());
    }

    public void ExitPanel()
    {
        speakerTextDialogue.text = "";
        gameObject.SetActive(false);
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choicesUIList[0].gameObject);
    }

    private void HandleTextTyping(string text)
    {
        DialogueManager.Instance.canContinue = false;
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string text)
    {
        speakerTextDialogue.text = "";
        bool textFullyRevealed = false;
        PlayerInputHandler inputManager = Player_v2.Instance.InputHandler;
        DialogueManager dialogueManager = DialogueManager.Instance;

        inputManager.jumpPressed = false;
        foreach (char c in text)
        {
            if (inputManager.jumpPressed && textFullyRevealed != true)
            {
                speakerTextDialogue.text = text;
                textFullyRevealed = true;
                break;
            }
            speakerTextDialogue.text += c;
            yield return typingSpeed;
        }
        yield return new WaitUntil(() => inputManager.jumpPressed != true);
        dialogueManager.canContinue = true;
    }
}
