using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GameManager.instance.OnGamePause += GameManager_OnGamePaused;
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        // pauseBtn.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        // pauseBtn.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
