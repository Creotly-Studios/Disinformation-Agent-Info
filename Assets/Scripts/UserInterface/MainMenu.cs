using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    
    [SerializeField] private Button playButton;
    [SerializeField] private Button statsButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [Space]
    [SerializeField] private Button redirectButton;
    [Space]
    [SerializeField] private int creditLevelIndex;
    [SerializeField] private int levelSelectionIndex;
    public enum CurrentPanel
    {
        None, Options, Stats
    }

    [Space] public CurrentPanel currentPanel;
    public event EventHandler OnPanelChanged;

    [Space] [SerializeField] private CanvasGroup sidePanelsHolder;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetCurrentPanel(CurrentPanel.None);
        playButton.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelSelectionIndex);
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
            //open user browser and redirect to the site url
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
    
    void Update()
    {
        
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


