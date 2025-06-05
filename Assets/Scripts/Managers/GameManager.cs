using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private InputSystem_Actions InputSystemActions;
    public GameState GameState { get; private set; }
    public GameOverState GameOverState {get; private set;}
    public event EventHandler OnStateChange;

    public event EventHandler OnGamePause;

    private bool isGamePaused = false;
    private bool canPause = true;

    [Header("-- Variables")]
    public int PlayerCoinAmount {get; private set;}
    public int PlayerRank {get; private set;} //1 min - 5 max... 

    private void Awake()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        GameState = GameState.Playing;
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        if(InputSystemActions == null)
        {
            InputSystemActions = new InputSystem_Actions();
            InputSystemActions.Player.Pause.performed += ctx => TogglePause();
        }
        InputSystemActions.Enable();
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
        if (InputSystemActions != null)
        {
            InputSystemActions.Player.Pause.performed -= ctx => TogglePause();
            InputSystemActions.Dispose();
        }
    }

    public void TogglePause()
    {
        if(canPause)
        {
            if (!IsGameOver())
            {
                Pause();
            }
        }
    }
    public void Unpause()
    {
        if (isGamePaused)
        {
            Time.timeScale = 1;
            isGamePaused = false;
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
        LoadAgencyScene();
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

    //stuff and data
    public void PlayerCoinAdd()
    {
        PlayerCoinAmount++;
    }

    public void SetCoinAmount(int value)
    {
        PlayerCoinAmount = value;
    }

    public int PlayerCoins()
    {
        return PlayerCoinAmount;
    }

    public void SetCanPause(bool _)
    {
        canPause = _;
    }
    public bool CheckIfCanPause() {return canPause;}

    void LoadAgencyScene()
    {
        StartCoroutine(ToAgencyScene());
    }

    IEnumerator ToAgencyScene()
    {
        ResetGame();
        yield return new WaitForSeconds(5);
        LevelLoader.LoadLevel(2);
    }

    public void ResetGame()
    {
        GameOverState = GameOverState.None;
        GameState = GameState.Playing;
        Time.timeScale = 1;
        isGamePaused = false;
    }
}

public enum GameState { Playing, GameOver }
public enum GameOverState
{
    None, PlayerDie, MissionComplete
}
