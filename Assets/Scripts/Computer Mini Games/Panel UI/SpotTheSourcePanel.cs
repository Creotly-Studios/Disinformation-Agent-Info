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
        if(hasInitialized)
        {
            return;
        }
        currentScore = 0;
        remainingTime = maxTime;

        InitalizePosts();
        SelectPostSO();
        scoreCount.text = currentScore.ToString();

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

    public void SpotSource_Update()
    {
        if (gameObject.activeSelf != true)
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
            return;
        }
        if(pickedAnswer != null ) { pickedAnswer.choiceButton.image.color = Color.red; }
        if(correctAnswer != null ) { correctAnswer.choiceButton.image.color = Color.green; }
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
