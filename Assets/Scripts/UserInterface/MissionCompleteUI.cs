using UnityEngine;
using UnityEngine.UI;

public class MissionCompleteUI : MonoBehaviour
{
    void Start()
    {
        GameManager.instance.OnStateChange += GameManager_OnStateChange;

        Hide();
    }

    void GameManager_OnStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.instance.IsGameOver() && GameManager.instance.IsMissionComplete())
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
