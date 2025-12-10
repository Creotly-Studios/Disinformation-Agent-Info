using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    private int index;
    private SaveMenuUI menuUI;
    public SavedData savedData { get; private set; }

    [Header("Slot UI Parameters")]
    [SerializeField] private Button slotUIButton;
    [SerializeField] private TextMeshProUGUI fileNameText;
    [SerializeField] private TextMeshProUGUI lastModifiedDate;

    public void InitializeSavedData(int index, SavedData data, SaveMenuUI menu)
    {
        menuUI = menu;
        savedData = data;
        this.index = index;
        fileNameText.text = savedData.fileName;

        lastModifiedDate.text = $"Last Modified Date: {savedData.modifiedDate}";
        slotUIButton.onClick.AddListener(() => HandleLoad());
    }

    public void HandleLoad()
    {
        menuUI.SetPickedData(index);
    }
}
