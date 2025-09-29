using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Action = System.Action;

public class NoticePopup : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button[] progressButton;
    [SerializeField] private GameObject buttonsContainer;
    [SerializeField] private TextMeshProUGUI[] buttonText;

    [Header("Parameters")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;

    private void OnDisable()
    {
        foreach(Button btn in progressButton)
        {
            btn.onClick.RemoveAllListeners();
        }
    }

    public void DisplayPopUpWindow(string text, NoticeType noticeType, QuestSO quest = null, QuestObjectives objective = null)
    {
        gameObject.SetActive(true);

        if(noticeType == NoticeType.Hint)
        {
            HandleHint(text);
        }
        if(noticeType == NoticeType.QuestCompleted)
        {
            StartCoroutine(QuestCompletedNotice(quest));
        }
        if(noticeType == NoticeType.ObjectiveCompleted)
        {
            StartCoroutine(ObjectiveCompletedNotice(objective));
        }
        if (noticeType == NoticeType.Correct || noticeType == NoticeType.Wrong)
        {
            QuizNotice(text, noticeType);
        }
    }

    private IEnumerator QuestCompletedNotice(QuestSO questSO)
    {
        if(questSO.isComplete)
        {
            contentText.color = Color.green;

            titleText.text = "Quest Completed";
            contentText.text = questSO.questTitle;
        }
        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }

    public void DialoguePopup(Color color, string content)
    {
        gameObject.SetActive(true);
        StartCoroutine(HandleDialoguePopup(color, content));
    }

    private IEnumerator HandleDialoguePopup(Color color, string content)
    {
        contentText.color = color;

        contentText.text = content;
        titleText.text = "Quest Not Complete";
        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }

    private IEnumerator ObjectiveCompletedNotice(QuestObjectives objective)
    {
        if(objective.isDone)
        {
            titleText.text = "Objective Completed";
            contentText.text = objective.description;
        }
        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }

    private void QuizNotice(string answer, NoticeType noticeType)
    {
        titleText.text = " ";
        contentText.text = " ";
        PrepButton("Continue", progressButton[0], buttonText[0], () => ContinueButton());

        if(noticeType == NoticeType.Wrong)
        {
            HandleText("You are Incorrect", Color.red);
        }
        else if(noticeType == NoticeType.Correct)
        {
            HandleText("You are Correct", Color.green);
        }
        contentText.text = answer;
    }

    private void HandleText(string title, Color textColor)
    {
        titleText.color = textColor;
        contentText.color = textColor;
        titleText.text = title;
    }

    private void HandleHint(string hintText)
    {
        contentText.text = " ";

        HandleText(" ", Color.white);
        PrepButton("Continue", progressButton[0], buttonText[0], () => ContinueButton());
        contentText.text = hintText;
    }

    public void HandleSimplePopup(string body, Action acceptFunc, Action rejectFunc)
    {
        print(5);
        contentText.text = body;
        gameObject.SetActive(true);

        progressButton[0].onClick.AddListener(() => acceptFunc());
        progressButton[1].onClick.AddListener(() => rejectFunc());
    }

    public void HandleMini_GameOver(string text, Action restart, Action quit)
    {
        HandleText("GAME OVER !!!", Color.red);

        contentText.color = Color.red;
        contentText.text = text;

        PrepButton("Quit Game", progressButton[1], buttonText[1], quit);
        PrepButton("Restart", progressButton[0], buttonText[0], restart);
    }

    //Button Functions
    private void PrepButton(string text, Button button, TextMeshProUGUI btnText, Action func)
    {
        foreach (Button btn in progressButton)
        {
            btn.gameObject.SetActive(false);
        }
        button.gameObject.SetActive(true);

        btnText.text = text;
        button.onClick.AddListener(() => func());
    }

    private void ContinueButton(Action func = null)
    {
        gameObject.SetActive(false);
        func?.Invoke();
    }
}
