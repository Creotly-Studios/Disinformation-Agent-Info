using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button menuBtn;
    [SerializeField] private Button replayBtn;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnStateChange += GameManager_OnStateChange;
        menuBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.ResetGame();
            LevelLoader.LoadLevel(0);
        });
        replayBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.ResetGame();
            LevelLoader.LoadLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        });
        Hide();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChange -= GameManager_OnStateChange;
    }

    void GameManager_OnStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver() && GameManager.Instance.IsPlayerDead())
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
        if (gameObject != null) // Add this check
        {
            gameObject.SetActive(false);
        }
    }
}
