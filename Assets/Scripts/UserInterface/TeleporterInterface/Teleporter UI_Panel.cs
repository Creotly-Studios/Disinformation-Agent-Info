using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TeleporterUI_Panel : MonoBehaviour
{
    [Header("UI")] 
    [SerializeField] private Button submitButton;
    [SerializeField] private Button closePanelButton;
    [SerializeField] private TMP_InputField codeInputField;
    [Space] [SerializeField] private GameObject teleporterUiPanel;

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
        QuestManager questManager = QuestManager.Instance;
        QuestSO quest = questManager.activeQuest;

        if (quest == null)
        {
            return;
        }
        
        QuestSO teleportQuest = questManager.availableQuests.Find(x => x.questCode.ToString() == codeInputField.text);

        if(teleportQuest != null)
        {
            codeInputField.text = "";
            teleporterUiPanel.SetActive(false);
            LevelLoader.LoadLevel(teleportQuest.questLevelIndex);
            return;
        }
        //Show Wrong Prompt
        codeInputField.text = "";
        Debug.Log("Wrong Code, Try Again!");
    }
}
