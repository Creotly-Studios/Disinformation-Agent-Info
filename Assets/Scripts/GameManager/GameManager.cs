using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public event EventHandler OnStateChange;
    public event EventHandler OnPlayerDie;
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
        Pause();
    }


    public bool IsGamePaused()
    {
        return isGamePaused;
    }
    
    public bool IsGameOver()
    {
        return true;
    }

    public void PlayerDie()
    {
        OnPlayerDie?.Invoke(this, EventArgs.Empty);
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
