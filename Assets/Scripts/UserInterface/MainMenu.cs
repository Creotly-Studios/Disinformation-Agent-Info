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
    [Space]
    [SerializeField] private int creditLevelIndex;

    [Space]
    [SerializeField] private int firstLevelIndex; //level1 tutorial index lol
    [SerializeField] private int currentLevelIndex; //store last level player played
    public enum CurrentPanel
    {
        None, Options, Stats
    }
    [Space] public CurrentPanel currentPanel;
    public event EventHandler OnPanelChanged;
    [Space] [SerializeField] private CanvasGroup sidePanelsHolder;

    [Header("Debug checking for the UI in the menu ")]
    [SerializeField] private bool hasGameData = true;
    [SerializeField] private bool hasCompletedGame =true;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CheckGameData(); // Call the function to check game data

        SetCurrentPanel(CurrentPanel.None);
        startNewGameButton.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(firstLevelIndex);
        });
        continueGameButton.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentLevelIndex);
        });
        playEndlessButton.onClick.AddListener(() =>
        {
            // Load endless mode scene or logic
            Debug.Log("Play Endless mode!");
        });
        statsButton.onClick.AddListener(() =>
        {
            OnStatsClick();
        });
        optionsButton.onClick.AddListener(() =>
        {
            OnOptionsClick();
        });
        redirectButton.onClick.AddListener(() =>
        {
            OpenDisinformationURL();
        });
        creditsButton.onClick.AddListener(() =>
        {
            LevelLoader.LoadLevel(creditLevelIndex);
        });
        quitButton.onClick.AddListener(() =>
        {
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
        Application.OpenURL("https://web-agentinfo.vercel.app/");
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
