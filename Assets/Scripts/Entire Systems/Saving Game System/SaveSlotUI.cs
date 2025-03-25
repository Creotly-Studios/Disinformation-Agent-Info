using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    private SaveMenuUI menuUI;
    public SavedData savedData { get; private set; }
    public string saveFilePath { get; private set; }

    [Header("Slot UI Parameters")]
    [SerializeField] private Button slotUIButton;
    [SerializeField] private TextMeshProUGUI fileNameText;
    [SerializeField] private TextMeshProUGUI lastModifiedDate;

    public void InitializeSavedData(SavedData data, string filePath, SaveMenuUI menu)
    {
        menuUI = menu;
        savedData = data;

        saveFilePath = filePath;
        fileNameText.text = savedData.fileName;

        lastModifiedDate.text = $"Last Modified Date: {savedData.modifiedDate}";
        slotUIButton.onClick.AddListener(() => HandleLoad());
    }

    public void HandleLoad()
    {
        menuUI.SetPickedData(savedData);
    }
}
