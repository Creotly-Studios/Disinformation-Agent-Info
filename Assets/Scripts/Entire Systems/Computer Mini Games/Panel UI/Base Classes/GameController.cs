using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameController
{
    private bool hasAnswered;    // True after a button is pressed; gates post-reset.
    private bool isResetting;    // Guard: prevents spawning multiple reset coroutines.
    private int hintCount;
    private int currentScore;
    private float remainingTime;

    public bool IsGameOver { get; private set; }

    private readonly WaitForSeconds postResetDelay;
    private List<PostSO> dynamicContentList;

    private readonly GamePanels gamePanel;
    private readonly ComputerPanel_UI computerPanel;

    private OptionBase selectedOption;
    public PostSO CurrentPost { get; private set; }

    [SerializeField] private ObjectiveType objectiveType;
    [SerializeField] private QuestObjective questObjective;

    // Convenience: all notifications from this controller target the computer's popup.
    private NoticePopup Popup => computerPanel.Popup;

    public GameController(GamePanels gp, ComputerPanel_UI cp)
    {
        gamePanel = gp;
        computerPanel = cp;
        postResetDelay = new WaitForSeconds(1.5f);
        objectiveType = gamePanel.PanelObjectiveType;
    }

    // ── Update (called from GamePanel_Update every frame) ─────────────────────

    public void HandleMiniGame_Update(float delta, TextMeshProUGUI hintText)
    {
        TimerCountdown(delta);
        hintText.text = $"Hints: {hintCount:00}";
    }

    // ── Game Logic ────────────────────────────────────────────────────────────

    private void GameOver(string reason)
    {
        IsGameOver = true;
        gamePanel.DisplayPanel(false);
        EventBus.Notification.OnShow?.Invoke(
            Popup, NotificationRequest.MiniGameOver(reason, () => ResetGameLogic(null), ExitGame));
    }

    private void TimerCountdown(float delta)
    {
        if (IsGameOver || computerPanel.IsPopupActive) return;
        remainingTime -= delta;
        if (remainingTime <= 0f) { remainingTime = 0f; GameOver("Time's up!"); return; }
        gamePanel.UpdateCountdownUI(remainingTime);
    }

    // ── Answer Evaluation ─────────────────────────────────────────────────────

    // Called by each game panel button via GamePanels.InitializeButton.
    public void InitializeButton(List<MiniGameOptionButton> uiButtons,
        MiniGameOptionButton button, TextMeshProUGUI counterText)
    {
        if (IsGameOver || hasAnswered) return;

        Image correct = null;
        selectedOption = button.Option;
        button.optionButton.interactable = false;
        bool isCorrect = selectedOption.IsCorrectAnswer;

        if (!isCorrect)
        {
            var crrtBtn = uiButtons.Find(x => x.IsCorrect());
            correct = crrtBtn.optionButton.image;
        }

        EvaluateAnswer(isCorrect, correct, button.optionButton.image, counterText);

        NoticeType resultType = isCorrect ? NoticeType.Correct : NoticeType.Wrong;
        EventBus.Notification.OnShow?.Invoke(
            Popup, NotificationRequest.QuizResult(resultType, selectedOption.Explanation));

        // Trigger post-reset exactly once per answered question.
        if (!isResetting)
            gamePanel.StartCoroutine(ResetAfterDelay());
    }

    public void EvaluateAnswer(bool isCorrect, Image correct, Image picked, TextMeshProUGUI counterText)
    {
        hasAnswered = true;
        if (isCorrect)
        {
            currentScore++;
            counterText.text = currentScore.ToString();
            EventBus.Quest.OnQuestObjectiveCompleted?.Invoke(true, false, objectiveType, null);
            if (questObjective.isDone) gamePanel.CompletedObjective();
            if (objectiveType != ObjectiveType.MiniGame_MalignInfluence)
                picked.color = Color.green;
        }
        else
        {
            if (objectiveType != ObjectiveType.MiniGame_MalignInfluence)
            {
                if (picked != null) picked.color = Color.red;
                if (correct != null) correct.color = Color.green;
            }
        }
    }

    // ── Post Logic ────────────────────────────────────────────────────────────

    // Bug fix: was called every frame from HandleMiniGame_Update, spawning
    // ~60 coroutines/second. Now triggered once per answer via InitializeButton.
    private IEnumerator ResetAfterDelay()
    {
        isResetting = true;
        yield return postResetDelay;

        if (selectedOption != null)
        {
            CurrentPost.hasChecked = true;
            GetCurrentPost();
            gamePanel.AllowButtonInteraction(true);
            selectedOption = null;
        }

        hasAnswered = false;
        isResetting = false;
    }

    private void GetCurrentPost()
    {
        if (dynamicContentList.Count == 0)
        {
            GameOver("Congratulations! You've completed all posts!");
            return;
        }
        int index = Random.Range(0, dynamicContentList.Count);
        CurrentPost = dynamicContentList[index];
        gamePanel.InitializePostContents(CurrentPost);
        dynamicContentList.RemoveAt(index);
    }

    // ── Button Actions ────────────────────────────────────────────────────────

    public void ExitGame()
    {
        if (questObjective != null && !questObjective.isDone)
            questObjective.progressValue = 0;
        dynamicContentList?.Clear();
        computerPanel.DisablePanels();
    }

    public void ProvideHint()
    {
        if (hintCount <= 0)
        {
            EventBus.Notification.OnShow?.Invoke(
                Popup, NotificationRequest.Payment(3, "Pay 3 Coins for a Hint", DisplayHint));
            return;
        }
        hintCount--;
        DisplayHint();
    }

    private void DisplayHint() =>
        EventBus.Notification.OnShow?.Invoke(Popup, NotificationRequest.Hint(CurrentPost.funFact_Hint));

    // ── Initialisation ────────────────────────────────────────────────────────

    public void ResetGameLogic(TextMeshProUGUI counterText)
    {
        QuestSO quest = QuestManager.Instance.ActiveQuest;
        questObjective = quest.FindQuestObjective(objectiveType);

        currentScore = 0;
        IsGameOver = false;
        hasAnswered = false;
        isResetting = false;

        if (counterText != null) counterText.text = "00";

        remainingTime = gamePanel.MaxTime;
        hintCount = Mathf.Max(0, questObjective.targetValue - 2);

        dynamicContentList = new List<PostSO>(gamePanel.ContentArray);
        for (int i = 0; i < dynamicContentList.Count; i++)
        {
            dynamicContentList[i] = ScriptableObject.Instantiate(dynamicContentList[i]);
            dynamicContentList[i].Initialize();
        }
        GetCurrentPost();
    }
}
