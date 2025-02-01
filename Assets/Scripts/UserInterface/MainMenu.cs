using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    [Header("Main Menu Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button playEndlessButton; // New button for endless mode
    [SerializeField] private Button optionsButton;
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
        None, StartPanel, OptionsPanel
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
        SetCurrentPanel(CurrentPanel.None);
        startButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.PlayClickSound();
            Debug.Log("start game");
        });
        playEndlessButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.PlayClickSound();
            Debug.Log("Play Endless mode!");
        });
        optionsButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.PlayClickSound();
            
            Debug.Log("open options menu");
        });
        redirectButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.PlayClickSound();
            OpenDisinformationURL();
        });
        quitButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.PlayClickSound();
            Application.Quit();
        });
        sidePanelsHolder.alpha = 1;
    }
    
    public void SetCurrentPanel(CurrentPanel cp)
    {
        currentPanel = cp;
        OnPanelChanged?.Invoke(this, this);
    }
    
    public CurrentPanel GetCurrentPanel()
    {
        return currentPanel;
    }

    void OpenDisinformationURL()
    {
        Application.OpenURL("https://gamejam-agentinfo.vercel.app");
    }

    public void SetCurrentPanelToNone()
    {
        currentPanel = CurrentPanel.None;
        OnPanelChanged?.Invoke(this, this);
    }
}
