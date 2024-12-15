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
        sfxButton.onClick.AddListener(() =>
        {
            SFXPlayer.Instance.ChangeVolume();
            UpdateText();
        });
        
        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateText();
        });
        
        UpdateText();
    }

    void UpdateText()
    {
        sfxText.text = $"SFX: {Mathf.Ceil(SFXPlayer.Instance.GetVolume() * 10)}";
        musicText.text = $"Music: {Mathf.Ceil(MusicManager.Instance.GetVolume() * 10)}";
    }
}
