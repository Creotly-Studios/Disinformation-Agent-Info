using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    [SerializeField] private Button startNewGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button playEndlessButton; // New button for endless mode
    [SerializeField] private Button statsButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [Space]
    [SerializeField] private Button redirectButton;

    public enum CurrentPanel
    {
        None, Options, Stats
    }
    [Space] public CurrentPanel currentPanel;
    
    public event EventHandler<MainMenu> OnPanelChanged;
    [Space] [SerializeField] private CanvasGroup sidePanelsHolder;

    [Header("Debug checking for the UI in the menu ")]
    [SerializeField] private bool hasGameData = true;
    [SerializeField] private bool hasCompletedGame =true;

    private void Awake()
    {
        instance = this;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("SFX", 1);
        PlayerPrefs.SetFloat("Music", 0.5f);
    }

    void Start()
    {
        CheckGameData(); // Call the function to check game data
        SetCurrentPanel(CurrentPanel.None);
        startNewGameButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.PlayClickSound();
            SetCurrentPanel(CurrentPanel.StartPanel);
            Debug.Log("start game");
        });
        playEndlessButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.PlayClickSound();
            Debug.Log("Play Endless mode!");
        });
        statsButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.PlayClickSound();
            OnStatsClick();
        });
        optionsButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.PlayClickSound();
            SetCurrentPanel(CurrentPanel.OptionsPanel);
            Debug.Log("open options menu");
        });
        redirectButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.PlayClickSound();
            OpenDisinformationURL();
        });
        creditsButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.PlayClickSound();
            LevelLoader.LoadLevel(creditLevelIndex);
        });
        quitButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.PlayClickSound();
            Application.Quit();
        });
        sidePanelsHolder.alpha = 1;
    }

    public void CheckGameData()
    {
        // check if there is existing game data
        //bool hasGameData = SaveSystem.HasSaveData(); // also do so here pls
        continueGameButton.gameObject.SetActive(hasGameData);

        // check if the player has completed the game
        //bool hasCompletedGame = SaveSystem.HasCompletedGame(); //implement thsi bool return in the save system pls
        playEndlessButton.gameObject.SetActive(hasCompletedGame);
    }

    public void SetCurrentPanel(CurrentPanel cp)
    {
        currentPanel = cp;
        OnPanelChanged?.Invoke(this, EventArgs.Empty);
    }
    public CurrentPanel GetCurrentPanel()
    {
        return currentPanel;
    }

    void OpenDisinformationURL()
    {
        Application.OpenURL("https://gamejam-agentinfo.vercel.app");
    }

    void OnOptionsClick()
    {
        switch (currentPanel)
        {
            case CurrentPanel.None:
                SetCurrentPanel(CurrentPanel.Options);
                break;
            case CurrentPanel.Options:
                SetCurrentPanel(CurrentPanel.None);
                break;
            case CurrentPanel.Stats:
                SetCurrentPanel(CurrentPanel.Options);
                break;
        }
    }

    void OnStatsClick()
    {
        switch (currentPanel)
        {
            case CurrentPanel.None:
                SetCurrentPanel(CurrentPanel.Stats);
                break;
            case CurrentPanel.Options:
                SetCurrentPanel(CurrentPanel.Stats);
                break;
            case CurrentPanel.Stats:
                SetCurrentPanel(CurrentPanel.None);
                break;
        }
    }
}
