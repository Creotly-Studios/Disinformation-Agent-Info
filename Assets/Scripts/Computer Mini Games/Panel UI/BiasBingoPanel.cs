using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BiasBingoPanel : MonoBehaviour
{
    private class PostClass
    {
        public bool hasChecked;
        public BingoPostSO postSO;

        public void Initialize(BingoPostSO postSO)
        {
            hasChecked = false;
            this.postSO = postSO;
        }
    }

    //Status
    private bool hasSet;
    private int currentScore;
    private bool hasInitialized;
    private float remainingTime;
    private string selectedAnswer = "";
    private WaitForSeconds waitForSeconds;

    //Current Data
    private PostClass currentPostClass = new();

    //Parameters
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
        SelectPostSO();
        currentScore = 0;
        remainingTime = maxTime;

        scoreCount.text = currentScore.ToString();
        dynamicContentList = contentArray.ToList();

        hintButton.onClick.AddListener(() => HintButton());
        exitButton.onClick.AddListener(() => SubmitButton());

        uiButton[0].choiceButton.onClick.AddListener(() => InitializeButton(0));
        uiButton[1].choiceButton.onClick.AddListener(() => InitializeButton(1));
        uiButton[2].choiceButton.onClick.AddListener(() => InitializeButton(2));
        uiButton[3].choiceButton.onClick.AddListener(() => InitializeButton(3));

        hasInitialized = true;
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

    private void Update()
    {
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

        currentPostClass.hasChecked = true;
        if (currentPostClass.postSO == null || currentPostClass.hasChecked)
        {
            SelectPostSO();
            uiButton.ForEach(x => x.choiceButton.interactable = true);
        }
    }

    private void InitializeButton(int i)
    {
        // Validate input first
        if (i < 0 || i >= uiButton.Count)
        {
            Debug.LogError("Invalid button index");
            return;
        }

        // Store selected answer and disable button
        selectedAnswer = uiButton[i].choiceText.text;
        uiButton[i].choiceButton.interactable = false;

        // Validate currentPostClass and its postSO
        if (currentPostClass == null || currentPostClass.postSO == null)
        {
            Debug.LogError("Current post or post SO is null");
            return;
        }

        // Find answers
        DialogueUIChoice pickedAnswer = uiButton.Find(x => x.choiceText.text == selectedAnswer);
        DialogueUIChoice correctAnswer = uiButton.Find(x => x.choiceText.text == currentPostClass.postSO.answer);

        if (hasSet)
        {
            return;
        }

        hasSet = true;
        
        // Validate found answers
        if (pickedAnswer == null || correctAnswer == null)
        {
            Debug.LogError("Could not find picked or correct answer buttons");
            return;
        }

        if (selectedAnswer.Equals(currentPostClass.postSO.answer))
        {
            currentScore++;
            scoreCount.text = currentScore.ToString();
            correctAnswer.choiceButton.image.color = Color.green;
            return;
        }
        
        pickedAnswer.choiceButton.image.color = Color.red;
        correctAnswer.choiceButton.image.color = Color.green;
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
            Debug.LogWarning("No more posts available");
            return;
        }

        BingoPostSO currentPost = GetPostSO();
        if (currentPost == null)
        {
            Debug.LogError("Failed to get valid post SO");
            return;
        }

        InitializePostContents(currentPost);
        dynamicContentList.Remove(currentPost);
        
        if (currentPostClass == null)
        {
            currentPostClass = new PostClass();
        }
        currentPostClass.Initialize(currentPost);
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
}
