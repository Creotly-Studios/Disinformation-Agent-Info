using TMPro;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DialogueUIPanel : MonoBehaviour
{
    private bool skipFlag;
    private bool isTyping;

    [Header("Panel")]
    public Transform dialoguePanel;
    [SerializeField] private Button skipButton;

    [Header("Player Speaker Panel")]
    public Image playerImage;
    public GameObject playerObject;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerTextDialogue;

    [Header("Player Speaker Panel")]
    public Image speakerImage;
    public GameObject speakerObject;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI speakerTextDialogue;

    [Header("Choices")]
    [SerializeField] private Transform choicesDrawer;
    [SerializeField] private List<DialogueUIChoice> choicesUIList;

    private Coroutine typingCoroutine;
    private WaitForSeconds typingSpeed;

    private void Awake()
    {
        typingSpeed = new WaitForSeconds(0.05f);
        skipButton.onClick.AddListener(() => HandleSkip());
    }

    public void StopDisplayCoroutine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
    }

    public void DisplayText(DialogueCharacterInformation speaker, string dialogueText)
    {
        if(DialogueManager.Instance.currentSpeakerType == SpeakerType.Player)
        {
            speakerObject.gameObject.SetActive(false);
            playerObject.gameObject.SetActive(true);

            speakerName.text = speaker.characterName;
            speakerImage.sprite = speaker.characterImage;
        }
        else
        {
            playerObject.gameObject.SetActive(false);
            speakerObject.gameObject.SetActive(true);

            speakerName.text = speaker.characterName;
            speakerImage.sprite = speaker.characterImage;

            switch (speaker.currentEmotion)
            {
                case EmotionState.Angry:
                    speakerTextDialogue.color = Color.red;
                    break;
                case EmotionState.Calm:
                    speakerTextDialogue.color = Color.green;
                    break;
                default:
                    speakerTextDialogue.color = Color.white;
                    break;
            }
        }
        typingCoroutine = StartCoroutine(StartTypingText(dialogueText));
    }

    private void HandleSkip()
    {
        if(isTyping == false)
        {
            return;
        }
        skipFlag = true;
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
        playerTextDialogue.text = "";
        speakerTextDialogue.text = "";
        gameObject.SetActive(false);
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choicesUIList[0].gameObject);
    }

    private IEnumerator StartTypingText(string text)
    {
        isTyping = true;
        DialogueManager.Instance.canContinue = false;
        if(DialogueManager.Instance.currentSpeakerType == SpeakerType.Player)
        {
            playerTextDialogue.text = "";
            foreach (char c in text)
            {
                if(skipFlag == true)
                {
                    playerTextDialogue.text = text;
                    break;
                }
                playerTextDialogue.text += c;
                yield return typingSpeed;
            }
        }
        else
        {
            speakerTextDialogue.text = "";
            foreach (char c in text)
            {
                if (skipFlag == true)
                {
                    speakerTextDialogue.text = text;
                    break;
                }
                speakerTextDialogue.text += c;
                yield return typingSpeed;
            }
        }
        isTyping = false;
        skipFlag = false;
        DialogueManager.Instance.canContinue = true;
    }
}
