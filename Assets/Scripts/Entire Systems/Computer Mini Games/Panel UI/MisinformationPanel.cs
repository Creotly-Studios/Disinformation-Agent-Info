using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MisinformationPanel : MonoBehaviour
{
    //Status
    private bool hasSet;
    private int currentScore;
    private float remainingTime;
    private bool hasInitialized;
    private bool isGameOver;

    //Parameters
    private PostSO currentPost;
    private PostFactType selectedPostType;
    private WaitForSeconds waitForSeconds;
    private ComputerPanel_UI computerPanelUI;
    private List<PostSO> dynamicContentList = new List<PostSO>();

    [Header("Properties")]
    [SerializeField] private float maxTime;
    [SerializeField] private PostSO[] contentArray;

    [Header("Idenifer Buttons")]
    [SerializeField] private Button info_Btn;
    [SerializeField] private Button malInfo_Btn;

    [Header("Post Properties")]
    [SerializeField] private Image authorImage;
    [SerializeField] private TextMeshProUGUI userName;
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
        waitForSeconds = new WaitForSeconds(0.5f);
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
        isGameOver = false;
        scoreCount.text = currentScore.ToString();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        InitalizePosts();
        SetCurrentPost();

        hintButton.onClick.AddListener(() => HintButton());
        exitButton.onClick.AddListener(() => SubmitButton());

        info_Btn.onClick.AddListener(() => InitalizeButton(info_Btn, PostFactType.Information));
        malInfo_Btn.onClick.AddListener(() => InitalizeButton(malInfo_Btn, PostFactType.MalignedInformation));

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

        info_Btn.onClick.RemoveListener(() => InitalizeButton(info_Btn, PostFactType.Information));
        malInfo_Btn.onClick.RemoveListener(() => InitalizeButton(malInfo_Btn, PostFactType.MalignedInformation));

        hasInitialized = false;
    }

    public void Misinformation_Update()
    {
        if (gameObject.activeSelf != true || isGameOver)
        {
            return;
        }

        bool popupActive = (computerPanelUI.popupPanel.gameObject.activeSelf == true);
        EnableButtons(popupActive != true);
        if (popupActive)
        {
            return;
        }

        TimerCountdown(Time.deltaTime);
        if (selectedPostType == PostFactType.None)
        {
            return;
        }
        StartCoroutine(ResetCurrentPost());
    }

    private void EnableButtons(bool status)
    {
        info_Btn.interactable = status;
        malInfo_Btn.interactable = status;
    }

    private IEnumerator ResetCurrentPost()
    {
        hasSet = false;
        selectedPostType = PostFactType.None;
        yield return waitForSeconds;

        currentPost.hasChecked = true;
        if (currentPost == null || currentPost.hasChecked)
        {
            SetCurrentPost();
        }
    }

    private void SetCurrentPost()
    {
        if (dynamicContentList.Count == 0)
        {
            EndGame("Congratulations! You've completed all posts!");
            return;
        }

        info_Btn.interactable = true;
        malInfo_Btn.interactable = true;

        currentPost = GetPost();
        InitializePostContents(currentPost);
        dynamicContentList.Remove(currentPost);
    }

    private PostSO GetPost()
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

    private void InitializePostContents(PostSO post)
    {
        authorName.text = post.postAuthor;
        postContent.text = post.postContent;
        userName.text = post.authorUsername;
        authorImage.sprite = post.authorImage;
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

    private void InitalizeButton(Button button, PostFactType factType)
    {
        if (isGameOver) return;

        selectedPostType = factType;
        button.interactable = false;

        if (hasSet == true)
        {
            return;
        }

        hasSet = true;
        if (currentPost.postFactType == selectedPostType)
        {
            currentScore++;
            scoreCount.text = currentScore.ToString();

            QuestObjectives objective = QuestManager.Instance.FindQuestObjective(ObjectiveType.MisInfoGames);
            if (objective != null && objective.isDone != true)
            {
                QuestSO quest = QuestManager.Instance.activeQuest;
                quest.IncreaseQuestObjectiveProgressLevels(objective);
            }
            computerPanelUI.popupPanel.DisplayPopUpWindow(currentPost.answerExplanation, NoticeType.Correct);
            return;
        }
        computerPanelUI.popupPanel.DisplayPopUpWindow(currentPost.answerExplanation, NoticeType.Wrong);
    }
}