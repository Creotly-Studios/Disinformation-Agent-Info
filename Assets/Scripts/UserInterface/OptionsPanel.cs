using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    [SerializeField] private Button sfxButton;
    [SerializeField] private TextMeshProUGUI sfxText;
    

    void Start()
    {
        sfxButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.ChangeVolume();
            UpdateText();
        });
        
        MainMenu.instance.OnPanelChanged += MainMenu_OnPanelChanged;
        Hide();
    }

    private void MainMenu_OnPanelChanged(object sender, EventArgs e)
    {
        if (MainMenu.instance.GetCurrentPanel() == MainMenu.CurrentPanel.Options)
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
        UpdateText();
        gameObject.SetActive(true);
    }

    void UpdateText()
    {
        sfxText.text = $" SFX: {SFXPlayer.Instance.GetVolume() * 10}";
    }
    
}
