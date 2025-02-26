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

    public enum CurrentPanel
    {
        None, StartPanel, OptionsPanel
    }
    [Space] public CurrentPanel currentPanel;

    public event EventHandler<MainMenu> OnPanelChanged;
    [Space][SerializeField] private CanvasGroup sidePanelsHolder;

    [Header("Debug checking for the UI in the menu ")]
    [SerializeField] private bool hasGameData = true;
    [SerializeField] private bool hasCompletedGame = true;

    [SerializeField] AudioClip mainMenuMusic;

    private void Awake()
    {
        instance = this;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("SFX", 1);
        PlayerPrefs.SetFloat("Music", 0.5f);
    }

    void Start()
    {
        AudioManager.Instance.PlayMusicWithXFade(mainMenuMusic);
        SetCurrentPanel(CurrentPanel.None);
        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (var btn in allButtons)
        {
            btn.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.soundEffects.buttonClick);
            });
        }

        startButton.onClick.AddListener(() =>
        {
            SetCurrentPanel(CurrentPanel.StartPanel);
            Debug.Log("start game");
        });
        playEndlessButton.onClick.AddListener(() =>
        {
            Debug.Log("Play Endless mode!");
        });
        optionsButton.onClick.AddListener(() =>
        {
            SetCurrentPanel(CurrentPanel.OptionsPanel);
            Debug.Log("open options menu");
        });
        redirectButton.onClick.AddListener(() =>
        {
            OpenDisinformationURL();
        });
        quitButton.onClick.AddListener(() =>
        {
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