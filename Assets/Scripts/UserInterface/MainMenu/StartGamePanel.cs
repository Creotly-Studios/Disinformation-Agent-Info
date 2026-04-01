using UnityEngine;
using UnityEngine.UI;

public class StartGamePanel : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button loadGame;
    [SerializeField] private Button startNewGame;
    [SerializeField] private Button backToMainMenu;

    [Header("Level Indices")]
    [SerializeField] private int agencyOfficeLevelIndex;
    [SerializeField] private int currentLevelIndex;

    private MainMenu _mainMenu;

    private void Start()
    {
        loadGame.onClick.AddListener(() => EventBus.Save.OnDisplaySaveMenu?.Invoke());
        startNewGame.onClick.AddListener(() => LevelLoader.LoadLevel(agencyOfficeLevelIndex));

        backToMainMenu.onClick.AddListener(() =>
        {
            Camera.main.GetComponent<MainMenuCamera>().ResetPosition();
            _mainMenu.SetCurrentPanelToNone();
        });
        MainMenu.instance.OnPanelChanged += OnPanelChanged;

        SetCanvasOpacity(1);
        Hide();
    }

    private void OnPanelChanged(object sender, MainMenu e)
    {
        _mainMenu = e;
        if (_mainMenu.GetCurrentPanel() == MainMenu.CurrentPanel.StartPanel) Show();
        else Hide();
    }

    private void Show()
    {
        Camera.main.GetComponent<MainMenuCamera>().MoveToStartMenu();
        gameObject.SetActive(true);
    }

    private void Hide() => gameObject.SetActive(false);

    private void SetCanvasOpacity(int value)
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = value;
    }
}