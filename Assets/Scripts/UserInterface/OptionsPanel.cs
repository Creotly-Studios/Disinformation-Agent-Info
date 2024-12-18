using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    [Header("SFX Player UI")]
    [SerializeField] private Button sfxButton;
    [SerializeField] private TextMeshProUGUI sfxText;
    
    [Header("Music Manager UI")]
    [SerializeField] private Button musicButton;
    [SerializeField] private TextMeshProUGUI musicText;
    
    void Start()
    {
        UpdateText();
        sfxButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.ChangeVolume();
            SFXPlayer.Instance.PlayClickSound();
            UpdateText();
        });
        
        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            SFXPlayer.Instance.PlayClickSound();
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
        sfxText.text = $"SFX: {Mathf.Ceil(SFXPlayer.Instance.GetVolume() * 10)}";
        musicText.text = $"Music: {Mathf.Ceil(MusicManager.Instance.GetVolume() * 10)}";
    }
    
}
