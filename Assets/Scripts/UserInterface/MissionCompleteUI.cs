using UnityEngine;
using UnityEngine.UI;

public class MissionCompleteUI : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.OnStateChange += GameManager_OnStateChange;
        Hide();
    }


    void GameManager_OnStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver() && GameManager.Instance.IsMissionComplete())
        {
            Show();
        }
        else Hide();
    }

    private void Show()
    {
        gameObject?.SetActive(true);
    }

    public void Hide()
    {
        gameObject?.SetActive(false);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChange -= GameManager_OnStateChange;
    }
}
