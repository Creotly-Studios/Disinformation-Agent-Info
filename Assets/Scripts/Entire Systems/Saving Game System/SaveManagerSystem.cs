using System;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveManagerSystem : MonoBehaviour
{
    public static SaveManagerSystem Instance { get; private set; }

    private bool canAutoSave;
    private bool pendingAutoSave;
    private string saveDirectory;
    private SavedData autoSavedFile;

    private readonly List<ISaveable> saveables = new();
    private const string AUTO_SAVE_SLOT = "AutoSave.agentInfo";

    [field: SerializeField] public SaveMenuUI SaveMenuUI { get; private set; }
    [field: SerializeField] public List<SavedData> SavedDataList { get; private set; } = new();

    public string SaveDirectory => saveDirectory;
    private string GetSavePath(string fileName) => Path.Combine(saveDirectory, fileName);

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        saveables.Capacity = 100;
        InitializeSaveSystem();
        SubscribeToEvents();
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy() => UnsubscribeFromEvents();

    private void Update()
    {
        if (pendingAutoSave && Player_v2.Instance != null)
            UpdateSavedFile(autoSavedFile);
    }

    private void OnApplicationQuit() => AutoSave();

    private void SubscribeToEvents()
    {
        EventBus.Save.OnHandleAutoSave += AutoSave;
        EventBus.Save.OnDisplaySaveMenu += DisplayMenuPanel;
        EventBus.Save.OnSetSceneAutoSave += SetAutoSaveEnabled;
        EventBus.Save.OnRegisterSaveableAsset += RegisterSaveableAsset;
    }

    private void UnsubscribeFromEvents()
    {
        EventBus.Save.OnHandleAutoSave -= AutoSave;
        EventBus.Save.OnDisplaySaveMenu -= DisplayMenuPanel;
        EventBus.Save.OnSetSceneAutoSave -= SetAutoSaveEnabled;
        EventBus.Save.OnRegisterSaveableAsset -= RegisterSaveableAsset;
    }

    // ── Initialization ────────────────────────────────────────────────────────

    private void InitializeSaveSystem()
    {
        saveDirectory = Path.Combine(Application.persistentDataPath, "SavedInstances");
        Directory.CreateDirectory(saveDirectory);

        string[] files = Directory.GetFiles(saveDirectory, "*.agentInfo")
            .OrderByDescending(File.GetLastWriteTimeUtc).ToArray();

        foreach (string file in files)
        {
            byte[] rawData = SecureSaveUtility.LoadFromFile(file);
            SavedData loadedData = SaveSerializer.Deserialize(rawData);
            SavedDataList.Add(loadedData);
        }

        autoSavedFile = SavedDataList.Find(x => x.fileName == "Auto Save");
        if (autoSavedFile == null)
        {
            autoSavedFile = CreateNewSaveData("Auto Save", isAutoSave: true);
            SaveGame(autoSavedFile);
        }
    }

    // ── Public Save API ───────────────────────────────────────────────────────

    public SavedData CreateNewSaveData(string filename, bool isAutoSave = false)
    {
        SavedData newData = new(filename, isAutoSave);
        UpdateSavedFile(newData);
        SavedDataList.Add(newData);
        return newData;
    }

    public void SaveGame(SavedData data)
    {
        if (data == null) return;
        string fileName = data.isAutoSaveFile ? AUTO_SAVE_SLOT : $"{data.fileName}.agentInfo";
        UpdateSavedFile(data);
        SecureSaveUtility.SaveToFile(GetSavePath(fileName), SaveSerializer.SerializeData(data));
    }

    public SavedData LoadGame(SavedData dataToLoad)
    {
        if (dataToLoad == null) return null;

        string fileName = dataToLoad.isAutoSaveFile ? AUTO_SAVE_SLOT : $"{dataToLoad.fileName}.agentInfo";
        string savePath = GetSavePath(fileName);

        if (!File.Exists(savePath))
            throw new FileNotFoundException($"Save file not found: {savePath}");

        saveables.Clear();
        return SaveSerializer.Deserialize(SecureSaveUtility.LoadFromFile(savePath));
    }

    public void DeleteSave(SavedData savedData)
    {
        if (savedData == null || savedData.isAutoSaveFile) return;
        string filePath = GetSavePath(savedData.fileName + ".agentInfo");
        if (File.Exists(filePath)) File.Delete(filePath);
        SavedDataList.Remove(savedData);
    }

    public void AutoSave()
    {
        if (!canAutoSave) return;

        QuestManager questManager = QuestManager.Instance;
        int currentLevel = questManager.CurrentLevel;

        if (currentLevel < autoSavedFile.currentLevel) return;

        if (currentLevel == autoSavedFile.currentLevel)
        {
            SerializableQuestData compare = autoSavedFile.GetQuestData(currentLevel);
            // Fix: was questManager.activeQuest (private field) → ActiveQuest property
            if (questManager.ActiveQuest.CompletedObjectives < compare.completedObjectives)
                return;
        }

        if (!File.Exists(GetSavePath(AUTO_SAVE_SLOT)))
            autoSavedFile = CreateNewSaveData("Auto Save", isAutoSave: true);

        SaveGame(autoSavedFile);
    }

    // ── Private Event Handlers ────────────────────────────────────────────────

    private void DisplayMenuPanel() => SaveMenuUI.gameObject.SetActive(true);
    private void SetAutoSaveEnabled(bool status) => canAutoSave = status;

    private void RegisterSaveableAsset(ISaveable saveable)
    {
        if (!saveables.Contains(saveable))
            saveables.Add(saveable);
    }

    // ── Internal ──────────────────────────────────────────────────────────────

    private void UpdateSavedFile(SavedData saveData)
    {
        saveData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        saveData.modifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        Player_v2 player = Player_v2.Instance;
        if (player == null) { pendingAutoSave = true; return; }

        QuestManager questManager = QuestManager.Instance;
        if (questManager == null) return;

        for (int i = 0; i < questManager.AvailableQuests.Count; i++)
        {
            QuestSO quest = questManager.AvailableQuests[i];
            if (saveData.questDataList.Find(x => x.questName == quest.questTitle) == null)
                saveData.questDataList.Add(new SerializableQuestData(quest));
            saveData.questDataList[i].UpdateQuestData(quest);
        }

        saveData.saveableAssets.Clear();
        saveables.RemoveAll(x => x == null);
        foreach (ISaveable asset in saveables)
        {
            asset.UpdateSavedData();
            saveData.saveableAssets.Add(asset.GetSaveData());
        }

        pendingAutoSave = false;
    }
}