using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public event EventHandler OnStateChange;
    public event EventHandler OnGamePause;
    
    private bool isGamePaused = false;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    private void OnDestroy()
    {
        OnGamePause = null;
    }

    private void Start()
    {
        InputManager.instance.InputSystemActions.Player.Pause.performed += ctx => TogglePause();
    }

    private void OnDisable()
    {
        if (InputManager.instance != null)
            InputManager.instance.InputSystemActions.Player.Pause.performed -= ctx => TogglePause();
    }

    public void TogglePause()
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


    public bool IsGamePaused()
    {
        return isGamePaused;
    }
    
    public bool IsGameOver()
    {
        return true;
    }
}
