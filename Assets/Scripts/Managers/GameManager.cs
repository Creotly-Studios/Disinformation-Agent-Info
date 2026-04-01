using System;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private InputSystem_Actions inputSystemActions;
    private bool isGamePaused;

    [Header("Notification")]
    [SerializeField] private NoticePopup paymentPopup;

    public event EventHandler OnGamePause;
    public event EventHandler OnStateChange;

    public int PlayerCoinAmount { get; private set; }
    public GameState GameState { get; private set; }
    public GameOverState GameOverState { get; private set; }

    public bool IsGamePaused() => isGamePaused;
    public bool IsGameOver() => GameState == GameState.GameOver;
    public bool IsPlayerDead() => GameOverState == GameOverState.PlayerDie;
    public bool IsMissionComplete() => GameOverState == GameOverState.MissionComplete;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        Time.timeScale = 1;
        GameState = GameState.Playing;
    }

    private void Start() => SetCoinAmount(10);
    private void OnDestroy() => OnGamePause = null;

    private void OnEnable()
    {
        if (inputSystemActions == null)
        {
            inputSystemActions = new InputSystem_Actions();
            inputSystemActions.Player.Pause.performed += OnPausePerformed;
        }
        inputSystemActions.Enable();
    }

    private void OnDisable()
    {
        if (inputSystemActions == null) return;
        inputSystemActions.Player.Pause.performed -= OnPausePerformed;
        inputSystemActions.Dispose();
        inputSystemActions = null;
    }

    private void OnPausePerformed(UnityEngine.InputSystem.InputAction.CallbackContext _)
        => TogglePause();

    // ── Payment ───────────────────────────────────────────────────────────────

    public void HandleCoinPaymentPopup(int amount, string body, Action onPaid) =>
        EventBus.Notification.OnShow?.Invoke(
            paymentPopup, NotificationRequest.Payment(amount, body, onPaid));

    // ── Pause ─────────────────────────────────────────────────────────────────

    public void TogglePause()
    {
        if (IsGameOver()) return;
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0 : 1;
        OnGamePause?.Invoke(this, EventArgs.Empty);
    }

    public void UnPause()
    {
        if (!isGamePaused) return;
        isGamePaused = false;
        Time.timeScale = 1;
    }

    // ── Game State ────────────────────────────────────────────────────────────

    public void SetGameState(GameState state)
    {
        GameState = state;
        OnStateChange?.Invoke(this, EventArgs.Empty);
    }

    public void PlayerDie()
    {
        GameOverState = GameOverState.PlayerDie;
        SetGameState(GameState.GameOver);
    }

    public void MissionComplete()
    {
        GameOverState = GameOverState.MissionComplete;
        SetGameState(GameState.GameOver);
        StartCoroutine(ToAgencyScene());
    }

    private IEnumerator ToAgencyScene()
    {
        ResetGame();
        yield return new WaitForSeconds(5f);
        LevelLoader.LoadLevel(2);
    }

    public void ResetGame()
    {
        GameState = GameState.Playing;
        GameOverState = GameOverState.None;
        isGamePaused = false;
        Time.timeScale = 1;
    }

    // ── Coins ─────────────────────────────────────────────────────────────────

    public void PlayerCoinAdd() => PlayerCoinAmount++;
    public void SetCoinAmount(int value) => PlayerCoinAmount = value;
}
