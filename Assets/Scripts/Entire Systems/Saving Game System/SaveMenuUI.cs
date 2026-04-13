using TMPro;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class SaveMenuUI : MonoBehaviour
{
    private bool hasInitialized;
    private int pickedSaveDataIndex;
    private SaveManagerSystem saveManagerSystem;
    private readonly List<SaveSlotUI> slotUIList = new();

    private UnityAction onNewSave;
    private UnityAction onOverwrite;

    [Header("Panels")]
    [SerializeField] private GameObject savePanel;
    [SerializeField] private GameObject loadPanel;

    [Header("Tools")]
    [SerializeField] private SaveSlotUI slotPrefab;
    [SerializeField] private Transform slotSpawnParent;

    [Header("Notification")]
    [SerializeField] private NoticePopup savePopup;

    [Header("Buttons")]
    [SerializeField] private Button loadButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button newSaveButton;
    [SerializeField] private Button overwriteButton;
    [SerializeField] private Button completeSaveButton;

    [Header("Save Properties")]
    [SerializeField] private TextMeshProUGUI fileNameLabel;
    [SerializeField] private TextMeshProUGUI fileSizeLabel;
    [SerializeField] private TextMeshProUGUI modifiedDateLabel;
    [SerializeField] private TMP_InputField fileNameInputField;

    private void Awake()
    {
        saveManagerSystem = GetComponentInParent<SaveManagerSystem>();
        onNewSave = () => ShowSavePanel(true);
        onOverwrite = () => ShowOverwriteConfirm("Are You Sure You Want To Overwrite This File?");
        savePopup.SubscribeEvents();
    }

    private void OnDestroy() => savePopup.UnSubscribeEvents();

    private void OnEnable()
    {
        if (hasInitialized)
        {
            return;
        }
        SetupButtonListeners(true);
        RefreshSlotUI();
        savePanel.SetActive(false);
        hasInitialized = true;
    }

    private void OnDisable()
    {
        if (!hasInitialized)
        {
            return;
        }
        SetupButtonListeners(false);
        hasInitialized = false;
    }

    // ── Button Wiring ─────────────────────────────────────────────────────────

    public void SetupButtonListeners(bool enable)
    {
        if (enable)
        {
            loadButton.onClick.AddListener(HandleLoad);
            exitButton.onClick.AddListener(HandleQuit);
            deleteButton.onClick.AddListener(HandleDelete);
            completeSaveButton.onClick.AddListener(HandleSave);
            newSaveButton.onClick.AddListener(onNewSave);
            overwriteButton.onClick.AddListener(onOverwrite);
            return;
        }
        loadButton.onClick.RemoveListener(HandleLoad);
        exitButton.onClick.RemoveListener(HandleQuit);
        deleteButton.onClick.RemoveListener(HandleDelete);
        completeSaveButton.onClick.RemoveListener(HandleSave);
        newSaveButton.onClick.RemoveListener(onNewSave);
        overwriteButton.onClick.RemoveListener(onOverwrite);
    }

    private void RefreshSlotUI()
    {
        List<SavedData> allData = saveManagerSystem.SavedDataList;
        int startIndex = slotUIList.Count;
        for (int i = startIndex; i < allData.Count; i++)
        {
            SaveSlotUI slot = Instantiate(slotPrefab, slotSpawnParent);
            slot.InitializeSavedData(i, allData[i], this);
            slotUIList.Add(slot);
        }
    }

    public void SetPickedData(int index)
    {
        ShowSavePanel(false);
        pickedSaveDataIndex = index;

        SavedData data = saveManagerSystem.SavedDataList[index];
        deleteButton.gameObject.SetActive(!data.isAutoSaveFile);
        fileNameLabel.text = $"File Name: {data.fileName}";
        modifiedDateLabel.text = $"Modified: {data.modifiedDate}";
        fileSizeLabel.text = "File Size: --";
    }

    private void HandleLoad() => LoadingSaveManager.LoadGame(saveManagerSystem.SavedDataList[pickedSaveDataIndex]);

    private void HandleSave()
    {
        string name = (fileNameInputField.text ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("[SaveMenuUI] File name cannot be empty.");
            return;
        }
        bool nameExists = saveManagerSystem.SavedDataList.Exists(x => x.fileName == name);
        bool fileExists = Directory.GetFiles(saveManagerSystem.SaveDirectory,
            name + ".*", SearchOption.TopDirectoryOnly).Length > 0;

        if (nameExists || fileExists)
        {
            int match = saveManagerSystem.SavedDataList.FindIndex(x => x.fileName == name);
            if (match >= 0) pickedSaveDataIndex = match;
            ShowOverwriteConfirm("File Already Exists — Overwrite?");
            return;
        }
        SavedData saved = saveManagerSystem.CreateNewSaveData(name, isAutoSave: false);
        saveManagerSystem.SaveGame(saved);
        RefreshSlotUI();
    }

    private void ShowOverwriteConfirm(string message)
    {
        EventBus.Notification.OnShow?.Invoke(savePopup, NotificationRequest.Confirm(message,accept: OverwriteSavedData,
            reject: () => EventBus.Notification.OnDismiss?.Invoke(savePopup)));
    }

    private void OverwriteSavedData()
    {
        saveManagerSystem.SaveGame(saveManagerSystem.SavedDataList[pickedSaveDataIndex]);
        RefreshSlotUI();
        SetPickedData(pickedSaveDataIndex);
        EventBus.Notification.OnDismiss?.Invoke(savePopup);
    }

    private void HandleDelete()
    {
        SavedData picked = saveManagerSystem.SavedDataList[pickedSaveDataIndex];
        if (picked.isAutoSaveFile)
        {
            return;
        }
        SaveSlotUI slot = slotUIList.Find(x => x.savedData == picked);
        saveManagerSystem.DeleteSave(picked);

        if (slot == null)
        {
            return;
        }
        slotUIList.Remove(slot);
        Destroy(slot.gameObject);
    }

    private void HandleQuit() => gameObject.SetActive(false);

    private void ShowSavePanel(bool show)
    {
        savePanel.SetActive(show);
        loadPanel.SetActive(!show);
    }

    public void DisablePanel()
    {
        savePanel.SetActive(false);
        loadPanel.SetActive(false);
        EventBus.Gameplay.OnGamePausedDisplay?.Invoke(false);
        HandleQuit();
    }
}
