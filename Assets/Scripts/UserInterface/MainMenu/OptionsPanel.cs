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

    [Header("Reset Progress UI")]
    [SerializeField] private Button resetButton;

    [Header("Credits UI")]
    [SerializeField] private Button creditsButton;
    [SerializeField] private int creditsSceneIndex = 8;

    [Header("Quality Settings UI")]
    [SerializeField] private TMP_Dropdown qualitySettingsDropDown;

    private MainMenu _mainMenu;

    [Space][SerializeField] private Button closePanelButton;
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

        resetButton.onClick.AddListener(() =>
        {
            Debug.Log("Reseting all progress");
        });

        creditsButton.onClick.AddListener(() =>
        {
            LevelLoader.LoadLevel(creditsSceneIndex);
        });

        qualitySettingsDropDown.onValueChanged.AddListener(SetQualityTo);

        MainMenu.instance.OnPanelChanged += MainMenu_OnPanelChanged;
        closePanelButton.onClick.AddListener(() =>
        {
            Camera.main.GetComponent<MainMenuCamera>().ResetPosition();
            _mainMenu.SetCurrentPanelToNone();
        });
        SetCanvasOpacity(1);
        Hide();
    }

    private void MainMenu_OnPanelChanged(object sender, MainMenu e)
    {
        _mainMenu = e;
        if (_mainMenu.GetCurrentPanel() == MainMenu.CurrentPanel.OptionsPanel)
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
        Camera.main.GetComponent<MainMenuCamera>().MoveToOptionsMenu();
        UpdateText();
        gameObject.SetActive(true);
    }

    void UpdateText()
    {
        sfxText.text = $"SFX: {Mathf.Ceil(SFXPlayer.Instance.GetVolume() * 10)}";
        musicText.text = $"Music: {Mathf.Ceil(MusicManager.Instance.GetVolume() * 10)}";
    }

    void SetCanvasOpacity(int value)
    {
        if (GetComponent<CanvasGroup>() != null)
        {
            GetComponent<CanvasGroup>().alpha = value;
        }
    }

    void SetQualityTo(int _)
    {
        QualitySettings.SetQualityLevel(_);
    }

}