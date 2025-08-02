using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GamePanels : MonoBehaviour
{
    //Status
    protected bool hasInitialized;

    private Teleporter teleporter;
    private GameController gameController;

    protected SocialMediaComputer sm_Computer;
    protected ComputerPanel_UI computerPanelUI;

    [field: Header("Content Arrays")]
    [field: SerializeField] public PostSO[] ContentArray { get; private set; }

    [field: Header("General Properties")]
    [field: SerializeField] public float MaxTime { get; private set; }
    [SerializeField] protected List<MiniGameOptionButton> uiButtons = new();
    [field: SerializeField] public ObjectiveType PanelObjectiveType { get; protected set; }

    [Header("UI Properties")]
    [SerializeField] protected Button exitButton;
    [SerializeField] protected Button hintButton;
    [SerializeField] protected GameObject postPanel;
    [SerializeField] protected GameObject answersPanel;
    [SerializeField] protected TextMeshProUGUI scoreCount;
    [SerializeField] protected TextMeshProUGUI hintButtonText;
    [SerializeField] protected TextMeshProUGUI countDownTimer;

    private void Awake()
    {
        teleporter = FindFirstObjectByType<Teleporter>();
        sm_Computer = GetComponentInParent<SocialMediaComputer>();
        computerPanelUI = GetComponentInParent<ComputerPanel_UI>();

        gameController = new(this, computerPanelUI, sm_Computer);
    }

    protected virtual void OnEnable()
    {
        ResetGame();
    }

    protected virtual void OnDisable()
    {
        UnInitializePanel();
    }

    private void ResetGame()
    {
        InitalizePanel();
        HandleButtonInitialization(true);
        DisplayPanel(true);
    }

    protected void InitalizePanel()
    {
        hasInitialized = true;
        gameController.ResetGameLogic(scoreCount);
        exitButton.onClick.AddListener(gameController.ExitGame);
        hintButton.onClick.AddListener(gameController.ProvideHint);
    }

    protected void UnInitializePanel()
    {
        hasInitialized = false;

        HandleButtonInitialization(false);
        exitButton.onClick.RemoveAllListeners();
        hintButton.onClick.RemoveAllListeners();
    }

    #region Panel Functions

    public void DisplayPanel(bool status)
    {
        postPanel.SetActive(status);
        answersPanel.SetActive(status);
    }

    public virtual void InitializePostContents(PostSO post)
    {
        for (int i = 0; i < post.PostCheckerOptions.Count; i++)
        {
            uiButtons[i].Initialize(post.PostCheckerOptions[i], PanelObjectiveType);
        }
    }

    #endregion

    public void GamePanel_Update()
    {
        if (gameObject.activeSelf != true || gameController.IsGameOver)
        {
            return;
        }

        bool popupActive = (computerPanelUI.popupPanel.gameObject.activeSelf == true) || (computerPanelUI.objectiveCompletePanel.activeSelf == true);
        EnableButtons(popupActive != true);
        if (popupActive)
        {
            return;
        }
        gameController.HandleMiniGame_Update(Time.deltaTime, hintButtonText);
    }

    protected virtual void EnableButtons(bool status)
    {
        foreach(var btn in uiButtons)
        {
            btn.optionButton.interactable = status;
        }
    }

    public void UpdateCountdownUI(float remainingTime)
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        int milliSecond = Mathf.FloorToInt((remainingTime * 1000) % 1000);
        countDownTimer.color = (remainingTime < 30f) ? Color.red : Color.white;
        countDownTimer.text = string.Format("{0:00} : {1:00} : {2:000}", minutes, seconds, milliSecond);
    }

    #region Button Initalization

    public void CompletedObjective()
    {
        teleporter.identifier.SetActive(true);
        StartCoroutine(computerPanelUI.DisplayObjectiveCompletedPopup());
    }

    public void AllowButtonInteraction(bool status)
    {
        uiButtons.ForEach(x => x.optionButton.interactable = status);
    }

    private void InitializeButton(MiniGameOptionButton button)
    {
        gameController.InitializeButton(uiButtons, button, countDownTimer);
    }

    protected virtual void HandleButtonInitialization (bool status)
    {
        var currentPost = gameController.CurrentPost;
        for (int i = 0; i < currentPost.PostCheckerOptions.Count; i++)
        {
            MiniGameOptionButton uiButton = uiButtons[i];
            
            Button button = uiButton.optionButton;
            OptionBase option = currentPost.PostCheckerOptions[i];
            if (status)
            {
                button.onClick.AddListener(() => InitializeButton(uiButton));
                continue;
            }
            button.onClick.RemoveListener(() => InitializeButton(uiButton));
        }
    }
    #endregion
}
