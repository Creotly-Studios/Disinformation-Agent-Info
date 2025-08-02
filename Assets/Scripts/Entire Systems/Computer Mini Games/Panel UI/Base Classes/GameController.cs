using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameController
{
    private int hintCount;
    private int currentScore;

    private bool hasSet;
    private float remainingTime;
    public bool IsGameOver { get; private set; }

    private WaitForSeconds waitForSeconds;
    private List<PostSO> dynamicContentList;

    //Game Panel Inherited Panels
    private readonly GamePanels gamePanel;
    private readonly ComputerPanel_UI computerPanel;
    private readonly SocialMediaComputer sm_Computer;

    private OptionBase selectedOption;
    public PostSO CurrentPost { get; private set; }

    [Header("Quest Parameters")]
    [SerializeField] private ObjectiveType objectiveType;
    [SerializeField] private QuestObjectives questObjective;

    public GameController(GamePanels gp, ComputerPanel_UI cp, SocialMediaComputer smc)
    {
        gamePanel = gp;
        sm_Computer = smc;
        computerPanel = cp;
        waitForSeconds = new WaitForSeconds(1.5f);
        objectiveType = gamePanel.PanelObjectiveType;
    }

    public void HandleMiniGame_Update(float delta, TextMeshProUGUI hintText)
    {
        TimerCountdown(delta);
        hintText.text = $"Hint: {hintCount:00}";
        gamePanel.StartCoroutine(ResetCurrentPost());
    }

    //Game Logic
    private void GameOver(string text)
    {
        IsGameOver = true;

        gamePanel.DisplayPanel(false);
        computerPanel.popupPanel.HandleMini_GameOver(text, () => ResetGameLogic(null), ExitGame);
    }

    protected void TimerCountdown(float delta)
    {
        bool popUp = computerPanel.popupPanel.gameObject.activeSelf;
        if (IsGameOver || popUp) return;

        remainingTime -= delta;
        if (remainingTime <= 0.0f)
        {
            remainingTime = 0.0f;
            GameOver("Time's up!");
            return;
        }
        gamePanel.UpdateCountdownUI(remainingTime);
    }

    private void WrongAnswer(Image pickedAnswer, Image correctAnswer)
    {
        if (objectiveType == ObjectiveType.MiniGame_MalignInfluence)
        {
            return;
        }
        if (pickedAnswer != null) { pickedAnswer.color = Color.red; }
        if (correctAnswer != null) { correctAnswer.color = Color.green; }
    }

    private void CorrectAnswer(Image buttonImage, TextMeshProUGUI counterText)
    {
        currentScore++;
        counterText.text = currentScore.ToString();

        QuestSO quest = QuestManager.Instance.activeQuest;
        if (quest != null)
        {
            QuestObjectives objective = quest.FindQuestObjective(objectiveType);
            if (objective != null && objective.isDone != true)
            {
                quest.IncreaseQuestObjectiveProgressLevels(objective, sm_Computer.identifier);
            }
            if (objective.isDone)
            {
                gamePanel.CompletedObjective();
            }
        }
        if (objectiveType != ObjectiveType.MiniGame_MalignInfluence) buttonImage.color = Color.green;
    }

    public void InitializeButton(List<MiniGameOptionButton> uiButtons, MiniGameOptionButton button, TextMeshProUGUI counterText)
    {
        if (IsGameOver)
        {
            return;
        }

        Image correct = null;
        selectedOption = button.Option;
        button.optionButton.interactable = false;
        bool isCorrect = selectedOption.IsCorrectAnswer;

        if (isCorrect != true)
        {
            var crrtBtn = uiButtons.Find(x => x.IsCorrect());
            correct = (crrtBtn == null) ? null : crrtBtn.optionButton.image;
        }
        EvaluateAnswer(isCorrect, correct, button.optionButton.image, counterText);
        ResultNotificationPopup(isCorrect, selectedOption.Explanation);
    }

    private void ResultNotificationPopup(bool isCorrect, string explanation)
    {
        NoticeType noticeType = (isCorrect) ? NoticeType.Correct : NoticeType.Wrong;
        computerPanel.popupPanel.DisplayPopUpWindow(explanation, noticeType);
    }

    public void EvaluateAnswer(bool isCorrect, Image correct, Image picked, TextMeshProUGUI counterText)
    {
        if (hasSet == true)
        {
            return;
        }

        hasSet = true;

        if (isCorrect)
        {
            CorrectAnswer(picked, counterText);
            return;
        }
        WrongAnswer(picked, correct);
    }

    //Post Logic
    private IEnumerator ResetCurrentPost()
    {
        hasSet = false;

        yield return waitForSeconds;
        SetCurrentPost_Complete();
    }

    private void GetCurrentPost()
    {
        int count = dynamicContentList.Count;
        if (count <= 0)
        {
            Debug.Log("All posts have been answered.");
            GameOver("Congratulations! You've completed all posts!");
            return;
        }
        int random = Random.Range(0, count);
        CurrentPost = dynamicContentList[random];

        gamePanel.InitializePostContents(CurrentPost);
        dynamicContentList.Remove(CurrentPost);
    }

    private void SetCurrentPost_Complete()
    {
        if(selectedOption == null)
        {
            return;
        }

        CurrentPost.hasChecked = true;
        if (CurrentPost == null || CurrentPost.hasChecked)
        {
            GetCurrentPost();
            gamePanel.AllowButtonInteraction(true);
        }
        selectedOption = null;
    }

    //Button Functionalities
    public void ExitGame()
    {
        if(questObjective != null && questObjective.isDone != true)
        {
            questObjective.progressValue = 0;
        }
        dynamicContentList.Clear();
        computerPanel.DisablePanels();
    }

    public void ProvideHint()
    {
        if (hintCount <= 0)
        {
            hintCount = 0;
        }
        hintCount--;
        computerPanel.popupPanel.DisplayPopUpWindow(CurrentPost.funFact_Hint, NoticeType.Hint);
    }

    public void ResetGameLogic(TextMeshProUGUI counterText)
    {
        QuestSO quest = QuestManager.Instance.activeQuest;
        questObjective = quest.FindQuestObjective(objectiveType);

        currentScore = 0;
        IsGameOver = false;
        if(counterText != null)
        {
            counterText.text = currentScore.ToString("00");
        }
        
        remainingTime = gamePanel.MaxTime;
        hintCount = questObjective.targetValue - 2;

        int length = gamePanel.ContentArray.Length;
        dynamicContentList = new(gamePanel.ContentArray);
        for (int i = 0; i < length; i++)
        {
            dynamicContentList[i] = ScriptableObject.Instantiate(dynamicContentList[i]);
            dynamicContentList[i].Initialize();
        }
        GetCurrentPost();
    }
}
