using TMPro;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SaveMenuUI : MonoBehaviour
{
    private bool hasInitialized;
    private string savedDirectory;

    private SavedData pickedSavedData;
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
    [SerializeField] private Button completeSaveButton;

    [Header("Save Properties")]
    [SerializeField] private TextMeshProUGUI fileName;
    [SerializeField] private TextMeshProUGUI fileSize;
    [SerializeField] private TextMeshProUGUI modifiedDate;
    [SerializeField] private TMP_InputField fileNameInputField;

    private void Awake()
    {
        savedDirectory = Path.Combine(Application.persistentDataPath, "Saved Instances");
    }

    private void OnEnable()
    {
        if(hasInitialized)
        {
            return;
        }

        print(01);
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

        print(-1);
        hasInitialized = false;
        InitializeButtons(false);
    }

    private void InitializeSlotUI()
    {
        //Instantiate UI and Add to Canvas
        if(Directory.Exists(savedDirectory) != true)
        {
            return;
        }

        string[] savedFiles = Directory.GetFiles(savedDirectory, "*.agentInfo").OrderByDescending(File.GetLastWriteTimeUtc).ToArray();

        int difference = savedFiles.Length - slotUIList.Count;
        for (int i = 0; i < difference; i++)
        {
            SaveSlotUI newSlot = Instantiate(slotPrefab, slotSpawnParent);
            slotUIList.Add(newSlot);
        }

        for (int i = 0; i < slotUIList.Count; i++)
        {
            string file = savedFiles[i];
            byte[] rawData = SecureSaveUtility.LoadFromFile(file);

            SavedData loadedData = SaveSerializer.Deserialize(rawData);
            slotUIList[i].InitializeSavedData(loadedData, file, this);
        }
    }

    private void HandleQuit()
    {
        print(5);
        gameObject.SetActive(false);
    }

    public void InitializeButtons(bool status)
    {
        print(6);
        if(status)
        {
            loadButton.onClick.AddListener(HandleLoad);
            exitButton.onClick.AddListener(HandleQuit);
            deleteButton.onClick.AddListener(HandleDelete);

            completeSaveButton.onClick.AddListener(HandleSave);
            newSaveButton.onClick.AddListener(() => ShowSavePanel(true));
            return;
        }
        loadButton.onClick.RemoveListener(HandleLoad);
        exitButton.onClick.RemoveListener(HandleQuit);
        deleteButton.onClick.RemoveListener(HandleDelete);

        completeSaveButton.onClick.RemoveListener(HandleSave);
        newSaveButton.onClick.RemoveListener(() => ShowSavePanel(true));
    }

    public void SetPickedData(SavedData savedData)
    {
        ShowSavePanel(false);
        pickedSavedData = savedData;
        deleteButton.gameObject.SetActive(savedData.isAutoSaveFile != true);

        fileSize.text = $"Modified Date: {10}KB"; //Would Change
        fileName.text = $"File Name: {pickedSavedData.fileName}";
        modifiedDate.text = $"Modified Date: {pickedSavedData.modifiedDate}";
    }

    private void HandleLoad()
    {
        LoadingSaveManager.LoadGame(pickedSavedData);
    }

    public void DisablePanel()
    {
        pickedSavedData = null;
        savePanel.SetActive(false);
        loadPanel.SetActive(false);
        HandleQuit();
    }

    private void HandleSave()
    {
        SaveManagerSystem saveManager = SaveManagerSystem.Instance;

        SavedData savedData = saveManager.CreateNewSaveData(fileNameInputField.text, false);
        saveManager.SaveGame(savedData);

        InitializeSlotUI();
    }

    private void ShowSavePanel(bool status)
    {
        savePanel.SetActive(status);
        loadPanel.SetActive(!status);
    }

    private void HandleDelete()
    {
        if(pickedSavedData.isAutoSaveFile)
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
