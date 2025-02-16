using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState GameState { get; private set; }
    public GameOverState GameOverState {get; private set;}
    public event EventHandler OnStateChange;

    public event EventHandler OnGamePause;

    private bool isGamePaused = false;

    DialogueManager dialogueManager;

    private void Awake()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        if (instance != null)
        {
            return;
        }
        instance = this;

        dialogueManager = DialogueManager.Instance;
    }

    void Start()
    {
        InputManager.instance.InputSystemActions.Player.Pause.performed += ctx => TogglePause();
    }

    void Update()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.Quest_Update();
        }
    }

    public void SetGameState(GameState _)
    {
        GameState = _;
        OnStateChange?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        OnGamePause = null;
    }

    private void OnDisable()
    {
        if (InputManager.instance != null)
            InputManager.instance.InputSystemActions.Player.Pause.performed -= ctx => TogglePause();
    }

    public void TogglePause()
    {
        if (!IsGameOver())
        {
            Pause();
        }
    }


    public bool IsGamePaused()
    {
        return isGamePaused;
    }
    public bool IsGameOver()
    {
        return GameState == GameState.GameOver;
    }

    public bool IsMissionComplete()
    {
        return GameOverState == GameOverState.MissionComplete;
    }
    public bool IsPlayerDead()
    {
        return GameOverState == GameOverState.PlayerDie;
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
    }

    void Pause()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0;
            OnGamePause?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1;
            OnGamePause?.Invoke(this, EventArgs.Empty);
        }
    }
}

public enum GameState { Playing, GameOver }
public enum GameOverState
{
    None, PlayerDie, MissionComplete
}
