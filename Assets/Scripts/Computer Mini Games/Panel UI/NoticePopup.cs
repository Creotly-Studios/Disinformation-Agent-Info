using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoticePopup : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button[] progressButton;
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

    public void DisplayPopUpWindow(string text, NoticeType noticeType, QuestSO quest = null)
    {
        gameObject.SetActive(true);

        if(noticeType == NoticeType.QuestCompleted)
        {
            StartCoroutine(QuestCompletedNotice(quest));
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
            titleText.text = "Quest Completed";
            contentText.text = questSO.questTitle;
        }
        else
        {
            titleText.text = "Completed";
            contentText.text = questSO.currentObjective.description;
        }
        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }

    private void QuizNotice(string answer, NoticeType noticeType)
    {
        titleText.text = " ";
        contentText.text = " ";
        PrepButton("Continue", progressButton[0], buttonText[0], ContinueButton);

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

    //Button Functions
    private void PrepButton(string text, Button button, TextMeshProUGUI btnText, System.Action func)
    {
        foreach (Button btn in progressButton)
        {
            btn.gameObject.SetActive(false);
        }
        button.gameObject.SetActive(true);

        btnText.text = text;
        button.onClick.AddListener(() => func());
    }

    private void ContinueButton()
    {
        gameObject.SetActive(false);
    }
}
