using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button menuBtn;

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
        Hide();
    }

    // private void OnEnable()
    // {
    //     GameManager.instance.OnGamePause += GameManager_OnGamePaused;
    // }

    // private void OnDisable()
    // {
    //     if (GameManager.instance != null)
    //         GameManager.instance.OnGamePause -= GameManager_OnGamePaused;
    // }
    
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

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
