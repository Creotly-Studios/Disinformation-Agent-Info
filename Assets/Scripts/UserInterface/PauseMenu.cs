using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button menuBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GameManager.instance.OnGamePause += GameManager_OnGamePaused;
        menuBtn.onClick.AddListener(() =>
            {
                LevelLoader.LoadLevel(0);
            });
        Hide();
    }

    private void OnEnable()
    {
        GameManager.instance.OnGamePause += GameManager_OnGamePaused;
    }

    private void OnDisable()
    {
        if (GameManager.instance != null)
            GameManager.instance.OnGamePause -= GameManager_OnGamePaused;
    }
    
    void GameManager_OnGamePaused(object sender, System.EventArgs e)
    {
        if (GameManager.instance.IsGamePaused())
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
