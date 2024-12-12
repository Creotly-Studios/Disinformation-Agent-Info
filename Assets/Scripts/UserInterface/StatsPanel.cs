using System;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{
    void Start()
    {
        MainMenu.instance.OnPanelChanged += MainMenu_OnPanelChanged;
        Hide();
    }

    private void MainMenu_OnPanelChanged(object sender, EventArgs e)
    {
        if (MainMenu.instance.GetCurrentPanel() == MainMenu.CurrentPanel.Stats)
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

}
