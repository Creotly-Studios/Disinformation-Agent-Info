using UnityEngine;
using UnityEngine.UI;

public class StartGamePanel : MonoBehaviour
{
    
    [Header("Buttons")]
    [SerializeField] private Button startNewGame;
    [SerializeField] private Button continueGame;

    private MainMenu _mainMenu;

    [Space] [SerializeField] private Button backToMainMenu;
    
    [Header("Debug for loading level")]
    [SerializeField] private int firstLevelIndex; //level1 tutorial index lol
    [SerializeField] private int currentLevelIndex; //store last level player played
    
    void Start()
    {
        startNewGame.onClick.AddListener(() =>
        {
            //ovvveride/clear save file and play
            LevelLoader.LoadLevel(firstLevelIndex);
        });
        
        continueGame.onClick.AddListener(() =>
        {
            //continue with save file and play
        });
        
        MainMenu.instance.OnPanelChanged += MainMenu_OnPanelChanged;
        backToMainMenu.onClick.AddListener(() =>
        {
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
