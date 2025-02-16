using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SpotTheSourcePanel : MonoBehaviour
{
    //Status
    private bool hasSet;
    private int currentScore;
    private bool hasInitialized;
    private float remainingTime;
    private bool isGameOver;
    private string selectedAnswer = "";
    private WaitForSeconds waitForSeconds;

    //Parameters
    private SourcePostSO currentPost;
    private ComputerPanel_UI computerPanelUI;
    private List<SourcePostSO> dynamicContentList = new();

    [Header("Properties")]
    [SerializeField] private float maxTime;
    [SerializeField] private SourcePostSO[] contentArray;

    [Header("Idenifer Buttons")]
    [SerializeField] private List<DialogueUIChoice> uiButton = new();

    [Header("Post Properties")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI authorName;
    [SerializeField] private TextMeshProUGUI postContent;

    [Header("UI Properties")]
    [SerializeField] private Button hintButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI scoreCount;
    [SerializeField] private TextMeshProUGUI countDownTimer;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private GameObject postPanel;
    [SerializeField] private GameObject answersPanel;

    private void Awake()
    {
        waitForSeconds = new WaitForSeconds(1.5f);
        computerPanelUI = GetComponentInParent<ComputerPanel_UI>();
    }

    private void OnEnable()
    {
        if(hasInitialized)
        {
            return;
        }
        currentScore = 0;
        remainingTime = maxTime;
        isGameOver = false;

        InitalizePosts();
        SelectPostSO();
        scoreCount.text = currentScore.ToString();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        hintButton.onClick.AddListener(() => HintButton());
        exitButton.onClick.AddListener(() => SubmitButton());

        uiButton[0].choiceButton.onClick.AddListener(() => InitializeButton(0));
        uiButton[1].choiceButton.onClick.AddListener(() => InitializeButton(1));
        uiButton[2].choiceButton.onClick.AddListener(() => InitializeButton(2));
        uiButton[3].choiceButton.onClick.AddListener(() => InitializeButton(3));

        hasInitialized = true;
        ShowPanel();
    }

    private void OnDisable()
    {
        if (hasInitialized != true)
        {
            return;
        }
        dynamicContentList.Clear();
        hintButton.onClick.RemoveListener(() => HintButton());
        exitButton.onClick.RemoveListener(() => SubmitButton());

        uiButton[0].choiceButton.onClick.RemoveListener(() => InitializeButton(0));
        uiButton[1].choiceButton.onClick.RemoveListener(() => InitializeButton(1));
        uiButton[2].choiceButton.onClick.RemoveListener(() => InitializeButton(2));
        uiButton[3].choiceButton.onClick.RemoveListener(() => InitializeButton(3));

        hasInitialized = false;
    }

    public void SpotSource_Update()
    {
        if (gameObject.activeSelf != true || isGameOver)
        {
            return;
        }

        TimerCountdown(Time.deltaTime);
        if(selectedAnswer.Equals(""))
        {
            return;
        }
        StartCoroutine(ResetCurrentPost());
    }

    private IEnumerator ResetCurrentPost()
    {
        hasSet = false;
        selectedAnswer = "";
        yield return waitForSeconds;

        currentPost.hasChecked = true;
        if(currentPost == null || currentPost.hasChecked)
        {
            SelectPostSO();
            uiButton.ForEach(x => x.choiceButton.interactable = true);
        }
    }

    private void InitializeButton(int i)
    {
        if (isGameOver) return;

        selectedAnswer = uiButton[i].choiceText.text;
        uiButton[i].choiceButton.interactable = false;

        DialogueUIChoice pickedAnswer = uiButton.Find(x => x.choiceText.text == selectedAnswer);
        DialogueUIChoice correctAnswer = uiButton.Find(x => x.choiceText.text == currentPost.correctAnswer);
        if(hasSet == true)
        {
            return;
        }

        hasSet = true;
        if (selectedAnswer.Equals(currentPost.correctAnswer))
        {
            currentScore++;
            scoreCount.text = currentScore.ToString();
            correctAnswer.choiceButton.image.color = Color.green;

            QuestSO quest = QuestManager.Instance.activeQuest;
            if (quest != null && quest.currentObjective.objectiveType == ObjectiveType.SpotTheSource)
            {
                quest.IncreaseQuestObjectiveProgressLevels(quest.currentObjective);
            }
            computerPanelUI.popupPanel.DisplayPopUpWindow(currentPost.answerExplanation, NoticeType.Correct);
            return;
        }
        if(pickedAnswer != null ) { pickedAnswer.choiceButton.image.color = Color.red; }
        if(correctAnswer != null ) { correctAnswer.choiceButton.image.color = Color.green; }
        computerPanelUI.popupPanel.DisplayPopUpWindow(currentPost.answerExplanation, NoticeType.Wrong);
    }

    private SourcePostSO GetPostSO()
    {
        int random = Random.Range(0, dynamicContentList.Count);
        return dynamicContentList[random];
    }

    private void InitalizePosts()
    {
        dynamicContentList = contentArray.ToList();
        for (int i = 0; i < dynamicContentList.Count; i++)
        {
            dynamicContentList[i] = Instantiate(dynamicContentList[i]);
        }
    }

    private void SelectPostSO()
    {
        if(dynamicContentList.Count <= 0)
        {
            EndGame("Congratulations! You've completed all posts!");
            return;
        }

        currentPost = GetPostSO();
        InitializePostContents(currentPost);
        dynamicContentList.Remove(currentPost);
    }
    
    private void InitializePostContents(SourcePostSO postSO)
    {
        title.text = postSO.title;
        postContent.text = postSO.writeUp;
        authorName.text = "Written By " + postSO.authorName;

        for(int i = 0; i < uiButton.Count; i++)
        {
            uiButton[i].choiceText.text = postSO.options[i];
            uiButton[i].choiceButton.image.color = Color.white;
        }
    }

    private void TimerCountdown(float delta)
    {
        bool popUp = computerPanelUI.popupPanel.gameObject.activeSelf;
        if (isGameOver || popUp) return;

        remainingTime -= delta;
        if (remainingTime <= 0.0f)
        {
            remainingTime = 0.0f;
            EndGame("Time's up!");
            return;
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        int milliSecond = Mathf.FloorToInt((remainingTime * 1000) % 1000);

        countDownTimer.color = (remainingTime < 30f) ? Color.red : Color.white;
        countDownTimer.text = string.Format("{0:00} : {1:00} : {2:000}", minutes, seconds, milliSecond);
    }

    private void EndGame(string message)
    {
        isGameOver = true;
        HidePanel();
    }

    private void HidePanel()
    {
        postPanel.SetActive(false);
        answersPanel.SetActive(false);
    }

    private void ShowPanel()
    {
        postPanel.SetActive(true);
        answersPanel.SetActive(true);
    }

    private void HintButton()
    {
        // Your hint button implementation
    }

    private void SubmitButton()
    {
        computerPanelUI.DisablePanels();
    }
}