using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PauseMenuOptions : MonoBehaviour
{
    [Header("SFX Player UI")]
    [SerializeField] private Button sfxButton;
    [SerializeField] private TextMeshProUGUI sfxText;

    [Header("Music Manager UI")]
    [SerializeField] private Button musicButton;
    [SerializeField] private TextMeshProUGUI musicText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateText();
        
        sfxButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.ChangeSFXVolume();
            UpdateText();
        });

        musicButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.ChangeMusicVolume();
            UpdateText();
        });

    }

    void UpdateText()
    {
        sfxText.text = $"SFX: {Mathf.Ceil(AudioManager.Instance.GetSFXVolume() * 10)}";
        musicText.text = $"Music: {Mathf.Ceil(AudioManager.Instance.GetMusicVolume() * 10)}";
    }
}
