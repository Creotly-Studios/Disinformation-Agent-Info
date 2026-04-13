using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class GamePanels : MonoBehaviour
{
    protected bool hasInitialized;

    private Teleporter teleporter;
    private GameController gameController;
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

    private readonly List<UnityAction> cachedButtonActions = new();

    private void Awake()
    {
        teleporter = FindFirstObjectByType<Teleporter>();
        computerPanelUI = GetComponentInParent<ComputerPanel_UI>();
        gameController = new GameController(this, computerPanelUI);
    }

    protected virtual void OnEnable() => ResetGame();
    protected virtual void OnDisable() => UninitializePanel();

    private void ResetGame()
    {
        InitializePanel();
        HandleButtonInitialization(true);
        DisplayPanel(true);
    }

    protected void InitializePanel()
    {
        hasInitialized = true;
        gameController.ResetGameLogic(scoreCount);
        exitButton.onClick.AddListener(gameController.ExitGame);
        hintButton.onClick.AddListener(gameController.ProvideHint);
    }

    protected void UninitializePanel()
    {
        hasInitialized = false;
        HandleButtonInitialization(false);
        exitButton.onClick.RemoveAllListeners();
        hintButton.onClick.RemoveAllListeners();
    }

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

    // ── Game Loop (driven by ComputerPanel_UI.Update) ─────────────────────────

    public void GamePanel_Update()
    {
        if (!gameObject.activeSelf || gameController.IsGameOver)
        {
            return;
        }
        bool popupActive = computerPanelUI.IsPopupActive || computerPanelUI.ObjectiveCompletePanel.activeSelf;

        EnableButtons(!popupActive);
        if (popupActive)
        {
            return;
        }
        gameController.HandleMiniGame_Update(Time.deltaTime, hintButtonText);
    }

    protected virtual void EnableButtons(bool status)
    {
        foreach (var btn in uiButtons)
        {
            btn.optionButton.interactable = status;
        }
    }

    public void UpdateCountdownUI(float remainingTime)
    {
        int min = Mathf.FloorToInt(remainingTime / 60);
        int sec = Mathf.FloorToInt(remainingTime % 60);
        int ms = Mathf.FloorToInt((remainingTime * 1000) % 1000);
        countDownTimer.color = remainingTime < 30f ? Color.red : Color.white;
        countDownTimer.text = $"{min:00} : {sec:00} : {ms:000}";
    }

    public void CompletedObjective()
    {
        teleporter.identifier.SetActive(true);
        StartCoroutine(computerPanelUI.DisplayObjectiveCompletedPopup());
    }

    public void AllowButtonInteraction(bool status) => uiButtons.ForEach(x => x.optionButton.interactable = status);
    private void OnButtonClicked(MiniGameOptionButton button) => gameController.InitializeButton(uiButtons, button, scoreCount);

    protected virtual void HandleButtonInitialization(bool add)
    {
        PostSO currentPost = gameController.CurrentPost;
        if (currentPost == null)
        {
            return;
        }

        if (add)
        {
            cachedButtonActions.Clear();
            for (int i = 0; i < currentPost.PostCheckerOptions.Count; i++)
            {
                MiniGameOptionButton captured = uiButtons[i];
                void action() => OnButtonClicked(captured);
                cachedButtonActions.Add(action);
                captured.optionButton.onClick.AddListener(action);
            }
        }
        else
        {
            for (int i = 0; i < uiButtons.Count && i < cachedButtonActions.Count; i++)
            {
                uiButtons[i].optionButton.onClick.RemoveListener(cachedButtonActions[i]);
            }
            cachedButtonActions.Clear();
        }
    }
}
