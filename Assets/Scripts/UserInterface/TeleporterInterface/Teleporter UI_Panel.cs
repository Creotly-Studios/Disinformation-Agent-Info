using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TeleporterUI_Panel : MonoBehaviour
{
    [Header("UI")] 
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button closePanelButton;
    [Space] [SerializeField] private GameObject teleporterUiPanel;

    [Header("Teleporter Logic")] public LevelData levelData;
    
    
    [Space]
    [SerializeField] private UnityEvent onCancelTeleport;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        teleporterUiPanel.SetActive(false);
        submitButton.onClick.AddListener(() =>
        {
            OnSubmitButtonClick();
        });
        closePanelButton.onClick.AddListener(() =>
        {
            teleporterUiPanel.SetActive(false);
            onCancelTeleport?.Invoke();
        });
    }
    
    private void OnSubmitButtonClick()
    {
        foreach (var ld in levelData.levelsData)
        {
            if (codeInputField.text == ld.levelLoadCode.ToString())
            {
                teleporterUiPanel.SetActive(false);
                LevelLoader.LoadLevel(ld.levelIndex);
            }
            else
            {
                Debug.Log("Wrong Code, Try Again!");
            }
        }
        codeInputField.text = "";
    }
}
