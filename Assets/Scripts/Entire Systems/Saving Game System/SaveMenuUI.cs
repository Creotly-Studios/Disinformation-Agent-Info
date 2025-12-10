using TMPro;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SaveMenuUI : MonoBehaviour
{
    private bool hasInitialized;
    private string saveDirectory;
    private int pickedSaveDataIndex;
    private SaveManagerSystem saveManagerSystem;
    private List<SaveSlotUI> slotUIList = new();

    [Header("Panels")]
    [SerializeField] private GameObject savePanel;
    [SerializeField] private GameObject loadPanel;

    [Header("Tools")]
    [SerializeField] private SaveSlotUI slotPrefab;
    [SerializeField] private NoticePopup popupPanel;
    [SerializeField] private Transform slotSpawnParent;

    [Header("Buttons")]
    [SerializeField] private Button loadButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button newSaveButton;
    [SerializeField] private Button ovewriteButton;
    [SerializeField] private Button completeSaveButton;

    [Header("Save Properties")]
    [SerializeField] private TextMeshProUGUI fileName;
    [SerializeField] private TextMeshProUGUI fileSize;
    [SerializeField] private TextMeshProUGUI modifiedDate;
    [SerializeField] private TMP_InputField fileNameInputField;

    private void Awake()
    {
        popupPanel.gameObject.SetActive(false);
        saveManagerSystem = GetComponentInParent<SaveManagerSystem>();
        saveDirectory = Path.Combine(Application.persistentDataPath, "Saved Instances");
    }

    private void OnEnable()
    {
        if(hasInitialized)
        {
            return;
        }
        InitializeButtons(true);

        InitializeSlotUI();
        hasInitialized = true;
        savePanel.SetActive(false);
    }

    private void OnDisable()
    {
        if (hasInitialized != true)
        {
            return;
        }
        hasInitialized = false;
        InitializeButtons(false);
    }

    private void InitializeSlotUI()
    {
        if(Directory.Exists(saveDirectory) != true)
        {
            return;
        }

        int difference = saveManagerSystem.SavedDataList.Count - slotUIList.Count;
        for (int i = 0; i < difference; i++)
        {
            SaveSlotUI newSlot = Instantiate(slotPrefab, slotSpawnParent);
            newSlot.InitializeSavedData(i, saveManagerSystem.SavedDataList[i], this);
            slotUIList.Add(newSlot);
        }
    }

    private void HandleQuit()
    {
        gameObject.SetActive(false);
    }

    public void InitializeButtons(bool status)
    {
        if(status)
        {
            loadButton.onClick.AddListener(HandleLoad);
            exitButton.onClick.AddListener(HandleQuit);
            deleteButton.onClick.AddListener(HandleDelete);

            completeSaveButton.onClick.AddListener(HandleSave);
            newSaveButton.onClick.AddListener(() => ShowSavePanel(true));
            ovewriteButton.onClick.AddListener(() => HandleOverwrite("Are You Sure You Want To Overwrite This File"));
            return;
        }
        loadButton.onClick.RemoveListener(HandleLoad);
        exitButton.onClick.RemoveListener(HandleQuit);
        deleteButton.onClick.RemoveListener(HandleDelete);

        completeSaveButton.onClick.RemoveListener(HandleSave);
        newSaveButton.onClick.RemoveListener(() => ShowSavePanel(true));
        ovewriteButton.onClick.RemoveListener(() => HandleOverwrite("Are You Sure You Want To Overwrite This File"));
    }

    public void SetPickedData(int index)
    {
        ShowSavePanel(false);
        pickedSaveDataIndex = index;
        SavedData savedData = saveManagerSystem.SavedDataList[pickedSaveDataIndex];
        deleteButton.gameObject.SetActive(savedData.isAutoSaveFile != true);

        fileSize.text = $"Modified Date: {10}KB"; //Would Change
        fileName.text = $"File Name: {savedData.fileName}";
        modifiedDate.text = $"Modified Date: {savedData.modifiedDate}";
    }

    private void HandleLoad()
    {
        LoadingSaveManager.LoadGame(saveManagerSystem.SavedDataList[pickedSaveDataIndex]);
    }

    public void DisablePanel()
    {
        savePanel.SetActive(false);
        loadPanel.SetActive(false);
        HandleQuit();
    }

    private void HandleSave()
    {
        string rawName = fileNameInputField.text ?? "";
        string fileName = rawName.Trim();

        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogWarning("[HandleSave] Empty file name.");
            return;
        }
        string candidatePath = Path.Combine(saveDirectory, fileName);

        bool exactFileExists = File.Exists(candidatePath);
        bool exactDirExists = Directory.Exists(candidatePath);
        bool filenameWithoutExtMatch = saveManagerSystem.SavedDataList.Exists(x => x.fileName == rawName);
        bool nameWithExtensionExists = Directory.GetFiles(saveDirectory, fileName + ".*", SearchOption.TopDirectoryOnly).Length > 0;
        if (exactFileExists || exactDirExists || nameWithExtensionExists || filenameWithoutExtMatch)
        {
            HandleOverwrite("File Already Exists, Do You Want To Overwrite ?");
            return;
        }
        SavedData savedData = SaveManagerSystem.Instance.CreateNewSaveData(fileName, false);
        SaveManagerSystem.Instance.SaveGame(savedData);
        InitializeSlotUI();
    }


    private void HandleOverwrite(string message)
    {
        popupPanel.HandleSimplePopup(message, OverwriteSavedData, () => popupPanel.gameObject.SetActive(false));
    }

    private void OverwriteSavedData()
    {
        SaveManagerSystem.Instance.SaveGame(saveManagerSystem.SavedDataList[pickedSaveDataIndex]);
        InitializeSlotUI();
        SetPickedData(pickedSaveDataIndex);

        popupPanel.gameObject.SetActive(false);
    }

    private void ShowSavePanel(bool status)
    {
        savePanel.SetActive(status);
        loadPanel.SetActive(!status);
    }

    private void HandleDelete()
    {
        SavedData pickedSavedData = saveManagerSystem.SavedDataList[pickedSaveDataIndex];
        if (pickedSavedData.isAutoSaveFile)
        {
            return;
        }

        SaveSlotUI deletedSlot = slotUIList.Find(x => x.savedData == pickedSavedData);
        SaveManagerSystem.Instance.DeleteSave(pickedSavedData);

        if(deletedSlot != null)
        {
            slotUIList.Remove(deletedSlot);
            Destroy(deletedSlot.gameObject);
            Debug.Log($"{pickedSavedData.fileName} has been deleted succesfully");
        }
    }
}
