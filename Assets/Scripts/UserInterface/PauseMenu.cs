using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button menuBtn;

    [Header("Primary Buttons")]
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button loadSaveBtn;

    [Header("Options Parameters")]
    [SerializeField] private Button optionsBtn;
    [SerializeField] private PauseMenuOptions optionsMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.OnGamePause += GameManager_OnGamePaused;

        menuBtn.onClick.AddListener(() =>
        {
                LevelLoader.LoadLevel(0);
                GameManager.Instance.ResetGame();
        });
        replayBtn.onClick.AddListener(() =>
        {
            LevelLoader.LoadLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            GameManager.Instance.ResetGame();
        });

        resumeBtn.onClick.AddListener(() => { GameManager.Instance.TogglePause(); });
        optionsBtn.onClick.AddListener(() => { optionsMenu.gameObject.SetActive(true); });
        loadSaveBtn.onClick.AddListener(() =>{SaveManagerSystem.Instance.DisplayMenuPanel();});
        Hide();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGamePause -= GameManager_OnGamePaused;
    }

    void GameManager_OnGamePaused(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGamePaused())
        {
            Show();
        }
        else Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(false);
    }
}
