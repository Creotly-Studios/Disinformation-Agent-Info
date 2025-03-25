using UnityEngine;
using UnityEngine.UI;

public class StartGamePanel : MonoBehaviour
{
    
    [Header("Buttons")]
    [SerializeField] private Button loadGame;
    [SerializeField] private Button startNewGame;

    private MainMenu _mainMenu;

    [Space] [SerializeField] private Button backToMainMenu;
    
    [Header("Debug for loading level")]
    [SerializeField] private int agencyOfficeLevelIndex; //level1 tutorial index lol
    [SerializeField] private int currentLevelIndex; //store last level player played
    
    void Start()
    {
        loadGame.onClick.AddListener(() =>
        {
            SaveManagerSystem.Instance.DisplayMenuPanel();
        });
        
        startNewGame.onClick.AddListener(() =>
        {
            //continue with save file and play
            LevelLoader.LoadLevel(agencyOfficeLevelIndex);
        });
        
        MainMenu.instance.OnPanelChanged += MainMenu_OnPanelChanged;
        backToMainMenu.onClick.AddListener(() =>
        {
            Camera.main.GetComponent<MainMenuCamera>().ResetPosition();
            _mainMenu.SetCurrentPanelToNone();
        });
        
        SetCanvasOpacity(1);
        Hide();
    }

    private void MainMenu_OnPanelChanged(object sender, MainMenu e)
    {
        _mainMenu = e;
        if (_mainMenu.GetCurrentPanel() == MainMenu.CurrentPanel.StartPanel)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    void Show()
    {
        Camera.main.GetComponent<MainMenuCamera>().MoveToStartMenu();
        gameObject.SetActive(true);
    }
    
    void SetCanvasOpacity(int value)
    {
        if (GetComponent<CanvasGroup>() != null)
        {
            GetComponent<CanvasGroup>().alpha = value;
        }
    }
    
}
