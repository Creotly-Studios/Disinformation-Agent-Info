using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BiasBingoPanel : MonoBehaviour
{
    //Status
    private bool hasSet;
    private int currentScore;
    private bool hasInitialized;
    private float remainingTime;
    private string selectedAnswer = "";
    private WaitForSeconds waitForSeconds;

    //Current Data
    private BingoPostSO currentPost;
    private ComputerPanel_UI computerPanelUI;
    private List<BingoPostSO> dynamicContentList = new();

    [Header("Properties")]
    [SerializeField] private float maxTime;
    [SerializeField] private BingoPostSO[] contentArray;

    [Header("Idenifer Buttons")]
    [SerializeField] private List<DialogueUIChoice> uiButton = new();

    [Header("Post Properties")]
    [SerializeField] private TextMeshProUGUI authorName;
    [SerializeField] private TextMeshProUGUI postContent;

    [Header("UI Prpoperties")]
    [SerializeField] private Button hintButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI scoreCount;
    [SerializeField] private TextMeshProUGUI countDownTimer;

    [Space]
    [SerializeField] private GameObject postPanel;
    [SerializeField] private GameObject answersPanel;

    private void Awake()
    {
        waitForSeconds = new WaitForSeconds(1.5f);
        computerPanelUI = GetComponentInParent<ComputerPanel_UI>();
    }

    private void OnEnable()
    {
        if (hasInitialized)
        {
            return;
        }

        currentScore = 0;
        remainingTime = maxTime;
        scoreCount.text = currentScore.ToString();

        InitalizePosts();
        SelectPostSO();

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

    private void InitalizePosts()
    {
        dynamicContentList = contentArray.ToList();
        for (int i = 0; i < dynamicContentList.Count; i++)
        {
            dynamicContentList[i] = Instantiate(dynamicContentList[i]);
        }
    }

    public void BiasBingPanel_Update()
    {
        if (gameObject.activeSelf != true)
        {
            return;
        }

        TimerCountdown(Time.deltaTime);
        if (selectedAnswer.Equals(""))
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
        if (currentPost == null || currentPost.hasChecked)
        {
            SelectPostSO();
            uiButton.ForEach(x => x.choiceButton.interactable = true);
        }
    }

    private void InitializeButton(int i)
    {
        // Validate index
        if (i < 0 || i >= uiButton.Count)
        {
            Debug.LogError($"Invalid button index: {i}. List count: {uiButton.Count}");
            return;
        }

        // Validate UI Button and choiceText
        var button = uiButton[i];
        if (button == null)
        {
            Debug.LogError($"uiButton[{i}] is null");
            return;
        }

        if (button.choiceText == null)
        {
            Debug.LogError($"choiceText is null for button at index {i}");
            return;
        }

        // Process button click
        selectedAnswer = button.choiceText.text;
        button.choiceButton.interactable = false;

        if (currentPost == null)
        {
            Debug.LogError("Current post is null");
            return;
        }

        // Evaluate answer
        DialogueUIChoice pickedAnswer = uiButton.Find(x => x.choiceText.text == selectedAnswer);
        DialogueUIChoice correctAnswer = uiButton.Find(x => x.choiceText.text == currentPost.answer);
        if (hasSet)
        {
            return;
        }

        hasSet = true;
        if (selectedAnswer.Equals(currentPost.answer))
        {
            currentScore++;
            scoreCount.text = currentScore.ToString();
            correctAnswer.choiceButton.image.color = Color.green;

            QuestSO quest = QuestManager.Instance.activeQuest;
            if (quest != null && quest.currentObjective.objectiveType == ObjectiveType.BiasBingo)
            {
                quest.IncreaseQuestObjectiveProgressLevels(quest.currentObjective);
            }
        }
        else
        {
            pickedAnswer.choiceButton.image.color = Color.red;
            correctAnswer.choiceButton.image.color = Color.green;
        }
    }


    private BingoPostSO GetPostSO()
    {
        int random = Random.Range(0, dynamicContentList.Count);
        return dynamicContentList[random];
    }

    private void SelectPostSO()
    {
        if (dynamicContentList == null || dynamicContentList.Count <= 0)
        {
            Debug.Log("All posts have been answered.");
            HidePanel();
            return;
        }

        currentPost = GetPostSO();
        InitializePostContents(currentPost);
        dynamicContentList.Remove(currentPost);
    }

    private void InitializePostContents(BingoPostSO postSO)
    {
        postContent.text = postSO.article;
        authorName.text = "Written By : " + postSO.authorName;

        for (int i = 0; i < uiButton.Count; i++)
        {
            uiButton[i].choiceText.text = postSO.options[i];
            uiButton[i].choiceButton.image.color = Color.white;
        }
    }

    private void TimerCountdown(float delta)
    {
        remainingTime -= delta;
        if (remainingTime <= 0.0f)
        {
            remainingTime = 0.0f;
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        int milliSecond = Mathf.FloorToInt((remainingTime * 1000) % 1000);

        countDownTimer.color = (remainingTime < 30f) ? Color.red : Color.white;
        countDownTimer.text = string.Format("{0:00} : {1:00} : {2: 000}", minutes, seconds, milliSecond);
    }

    private void HintButton()
    {

    }

    private void SubmitButton()
    {
        computerPanelUI.DisablePanels();
    }

    void HidePanel()
    {
        postPanel.SetActive(false);
        answersPanel.SetActive(false);
    }

    void ShowPanel()
    {
        postPanel.SetActive(true);
        answersPanel.SetActive(true);
    }
}
