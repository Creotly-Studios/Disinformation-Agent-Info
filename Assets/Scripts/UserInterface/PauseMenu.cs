using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button menuBtn;
    
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button loadSaveBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.OnGamePause += GameManager_OnGamePaused;

        menuBtn.onClick.AddListener(() =>
        {
                LevelLoader.LoadLevel(0);
        });
        resumeBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePause();
        });
        replayBtn.onClick.AddListener(() =>
        {
            LevelLoader.LoadLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        });
        loadSaveBtn.onClick.AddListener(() =>
        {
            SaveManagerSystem.Instance.DisplayMenuPanel();
        });

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
        gameObject?.SetActive(true);
        Player_v2.Instance.DisplayPauseButton(false);
    }

    public void Hide()
    {
        gameObject?.SetActive(false);
        Player_v2.Instance.DisplayPauseButton(true);
    }
}
