using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GamePanels : MonoBehaviour
{
    //Status
    protected bool hasSet;
    protected bool isGameOver;
    protected bool hasInitialized;

    protected int currentScore;
    protected float remainingTime;
    protected string correctAnswer;
    protected string reasonForAnswer;
    public string selectedAnswer { get; protected set; } = "";

    protected WaitForSeconds waitForSeconds;
    protected SocialMediaComputer sm_Computer;
    protected ComputerPanel_UI computerPanelUI;

    [Header("Properties")]
    [SerializeField] protected float maxTime;
    [SerializeField] protected ObjectiveType objectiveType;
    [SerializeField] protected List<DialogueUIChoice> uiButton = new();

    [Header("UI Properties")]
    [SerializeField] protected Button exitButton;
    [SerializeField] protected GameObject postPanel;
    [SerializeField] protected GameObject answersPanel;
    [SerializeField] protected GameObject gameOverPanel;
    [SerializeField] protected TextMeshProUGUI scoreCount;
    [SerializeField] protected TextMeshProUGUI gameOverText;
    [SerializeField] protected TextMeshProUGUI countDownTimer;

    protected void Awake()
    {
        waitForSeconds = new WaitForSeconds(0.5f);
        sm_Computer = GetComponentInParent<SocialMediaComputer>();
        computerPanelUI = GetComponentInParent<ComputerPanel_UI>();
    }

    protected void InitalizePanel()
    {
        currentScore = 0;
        isGameOver = false;
        hasInitialized = true;

        remainingTime = maxTime;
        scoreCount.text = currentScore.ToString();
        if (gameOverPanel != null) { gameOverPanel.SetActive(false); }

        InitalizePosts();
        SelectPostSO();
        exitButton.onClick.AddListener(() => SubmitButton(objectiveType));
    }

    protected void UnInitializePanel(ObjectiveType objType)
    {
        hasInitialized = false;
        exitButton.onClick.RemoveListener(() => SubmitButton(objectiveType));
        HandleButtonInitialization(false, objType);
    }

    #region Panel Functions

    protected void EndGame(string message)
    {
        isGameOver = true;
        HidePanel();
    }

    protected void SubmitButton(ObjectiveType objType)
    {
        QuestObjectives objective = QuestManager.Instance.FindQuestObjective(objType);
        if (objective != null && objective.isDone != true)
        {
            objective.progressValue = 0;
        }
        computerPanelUI.DisablePanels();
    }

    protected void HidePanel()
    {
        postPanel.SetActive(false);
        answersPanel.SetActive(false);
    }

    protected void ShowPanel()
    {
        postPanel.SetActive(true);
        answersPanel.SetActive(true);
    }

    #endregion

    #region SO Functions

    protected virtual void InitalizePosts()
    {
        
    }

    protected virtual void SelectPostSO()
    {
        
    }

    #endregion

    public void GamePanel_Update(PostFactType selectedPostType, string selectedAnswer)
    {
        if (gameObject.activeSelf != true || isGameOver)
        {
            return;
        }

        bool popupActive = (computerPanelUI.popupPanel.gameObject.activeSelf == true) || (computerPanelUI.objectiveCompletePanel.activeSelf == true);
        EnableButtons(popupActive != true);
        if (popupActive)
        {
            return;
        }

        TimerCountdown(Time.deltaTime);
        bool isMisinfoPanel = this is MisinformationPanel;

        if (isMisinfoPanel)
        {
            if(selectedPostType != PostFactType.None)
            {
                StartCoroutine(ResetCurrentPost());
            }
            return;
        }
        if (!selectedAnswer.Equals("")) { StartCoroutine(ResetCurrentPost()); }
    }

    protected virtual void EnableButtons(bool status)
    {
        foreach (DialogueUIChoice btn in uiButton)
        {
            btn.choiceButton.interactable = status;
        }
    }

    protected virtual IEnumerator ResetCurrentPost()
    {
        yield return null;
    }

    protected void TimerCountdown(float delta)
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

    protected void WrongAnswer(DialogueUIChoice pickedAnswer, DialogueUIChoice correctAnswer)
    {
        if (pickedAnswer != null) { pickedAnswer.choiceButton.image.color = Color.red; }
        if (correctAnswer != null) { correctAnswer.choiceButton.image.color = Color.green; }
    }

    protected virtual void CorrectAnswer(string answerExplanation, ObjectiveType objectiveType)
    {
        currentScore++;
        scoreCount.text = currentScore.ToString();

        QuestObjectives objective = QuestManager.Instance.FindQuestObjective(objectiveType);
        if (objective != null && objective.isDone != true)
        {
            QuestSO quest = QuestManager.Instance.activeQuest;
            quest.IncreaseQuestObjectiveProgressLevels(objective, sm_Computer.identifier);
        }
        if (objective.isDone) { StartCoroutine(computerPanelUI.DisplayObjectiveCompletedPopup()); }
        computerPanelUI.popupPanel.DisplayPopUpWindow(reasonForAnswer, NoticeType.Correct);
    }

    #region Button Initalization

    protected virtual void HandleButtonInitialization (bool status, ObjectiveType objType)
    {
        if(status)
        {
            uiButton[0].choiceButton.onClick.AddListener(() => InitializeButton(0, objType));
            uiButton[1].choiceButton.onClick.AddListener(() => InitializeButton(1, objType));
            uiButton[2].choiceButton.onClick.AddListener(() => InitializeButton(2, objType));
            uiButton[3].choiceButton.onClick.AddListener(() => InitializeButton(3, objType));
            return;
        }
        uiButton[0].choiceButton.onClick.RemoveListener(() => InitializeButton(0, objType));
        uiButton[1].choiceButton.onClick.RemoveListener(() => InitializeButton(1, objType));
        uiButton[2].choiceButton.onClick.RemoveListener(() => InitializeButton(2, objType));
        uiButton[3].choiceButton.onClick.RemoveListener(() => InitializeButton(3, objType));
    }

    protected virtual void InitializeButton(int i, ObjectiveType objType)
    {
        if (isGameOver) return;

        selectedAnswer = uiButton[i].choiceText.text;
        uiButton[i].choiceButton.interactable = false;

        DialogueUIChoice pickedAnswerUI = uiButton.Find(x => x.choiceText.text == selectedAnswer);
        DialogueUIChoice correctAnswerUI = uiButton.Find(x => x.choiceText.text == correctAnswer);
        EvaluateAnswer(pickedAnswerUI, correctAnswerUI, objType);
    }

    protected void EvaluateAnswer(DialogueUIChoice picked, DialogueUIChoice correct, ObjectiveType objType)
    {
        if (hasSet == true)
        {
            return;
        }

        hasSet = true;
        if (selectedAnswer.Equals(correctAnswer))
        {
            CorrectAnswer(reasonForAnswer, objType);
            correct.choiceButton.image.color = Color.green;
            return;
        }
        WrongAnswer(picked, correct);
        computerPanelUI.popupPanel.DisplayPopUpWindow(reasonForAnswer, NoticeType.Wrong);
    }
    

    #endregion
}
