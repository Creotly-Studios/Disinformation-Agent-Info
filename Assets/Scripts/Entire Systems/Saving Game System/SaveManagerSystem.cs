using System;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveManagerSystem : MonoBehaviour
{
    public static SaveManagerSystem Instance { get; private set; }

    private SceneStatusManager sceneStatusManager;

    private bool canAutoSave;
    private bool hasInitialized;
    private string saveDirectory;
    private bool shouldCompletedAutoSave;
    public List<ISaveable> saveables = new();
    
    [SerializeField] private SavedData autoSavedFile;
    private const string AUTO_SAVE_SLOT = "AutoSave.agentInfo";
    [field: SerializeField] public SaveMenuUI SaveMenuUI { get; private set;}
    [field: SerializeField] public List<SavedData> SavedDataList { get; private set; } = new();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(hasInitialized != true)
        {
            InitializeSaveSystem();
        }
        if(shouldCompletedAutoSave && Player_v2.Instance != null)
        {
            UpdateSavedFile(autoSavedFile);
        }
    }

    private void InitializeSaveSystem()
    {
        saveDirectory = Path.Combine(Application.persistentDataPath, "Saved Instances");
        string[] files = Directory.GetFiles(saveDirectory, "*.agentInfo").OrderByDescending(File.GetLastWriteTimeUtc).ToArray();
        for (int i = 0; i < files.Length; i++)
        {
            byte[] rawData = SecureSaveUtility.LoadFromFile(files[i]);
            SavedData loadedData = SaveSerializer.Deserialize(rawData);
            SavedDataList.Add(loadedData);
        }

        autoSavedFile = SavedDataList.Find(x => x.fileName == "Auto Save");
        if(autoSavedFile == null)
        {
            autoSavedFile = CreateNewSaveData("Auto Save", true);
            SaveGame(autoSavedFile);
        }
        hasInitialized = true;
    }

    public void DisplayMenuPanel()
    {
        SaveMenuUI.gameObject.SetActive(true);
    }

    public void SaveGame(SavedData newData)
    {
        if(newData == null) { return; }
        string newDataPath = $"{newData.fileName}.agentInfo";
        string savePath = GetSavePath(newData.isAutoSaveFile ? AUTO_SAVE_SLOT : newDataPath);

        UpdateSavedFile(newData);
        byte[] serializedData = SaveSerializer.SerializeData(newData);

        SecureSaveUtility.SaveToFile(savePath, serializedData);
    }

    public void DeleteSave(SavedData savedData)
    {
        if(savedData == null || savedData.isAutoSaveFile)
        {
            return;
        }

        string filePath = Path.Combine(saveDirectory, savedData.fileName + ".agentInfo");
        if(!File.Exists(filePath))
        {
            return;
        }
        File.Delete(filePath);
        SavedDataList.Remove(savedData);
    }

    public SavedData LoadGame(SavedData dataToLoad)
    {
        if (dataToLoad == null)
        { 
            return null;
        }
        string newDataPath = $"{dataToLoad.fileName}.agentInfo";
        string savePath = GetSavePath(dataToLoad.isAutoSaveFile ? AUTO_SAVE_SLOT : newDataPath);

        if (File.Exists(savePath) != true)
        {
            throw new FileNotFoundException($"Save File Not Found At {savePath}");
        }
        byte[] loadedData = SecureSaveUtility.LoadFromFile(savePath);

        saveables.Clear();
        return SaveSerializer.Deserialize(loadedData);
    }

    private string GetSavePath(string fileName)
    {
        return Path.Combine(saveDirectory, fileName);
    }

    public SavedData CreateNewSaveData(string filename, bool isAutoSave = false)
    {
        SavedData newData = new(filename, isAutoSave);
        UpdateSavedFile(newData);
        SavedDataList.Add(newData);
        return newData;
    }

    private void UpdateSavedFile(SavedData saveData)
    {
        Player_v2 player = Player_v2.Instance;
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        saveData.sceneIndex = sceneIndex;
        saveData.modifiedDate = dateTime;

        if (player == null)
        {
            shouldCompletedAutoSave = true;
            return;
        }
        QuestManager questManager = QuestManager.Instance;
        if(questManager == null)
        {
            return;
        }

        for (int i = 0; i < questManager.availableQuests.Count; i++)
        {
            QuestSO quest = questManager.availableQuests[i];
            SerializableQuestData exist = saveData.questDataList.Find(x => x.questName == quest.questTitle);
            if(exist == null)
            {
                saveData.questDataList.Add(new(quest));
            }
            saveData.questDataList[i].UpdateQuestData(quest);
        }

        saveData.saveableAssets.Clear();
        saveables.RemoveAll(x => x == null);
        foreach(var asset in saveables)
        {
            asset.UpdateSavedData();
            saveData.saveableAssets.Add(asset.GetSaveData());
        }
        saveData.killedEnemiesIndex.Clear();
        saveData.killedEnemiesIndex.AddRange(sceneStatusManager.KilledEnemies);
        shouldCompletedAutoSave = false;
    }

    public void AutoSave()
    {
        if(canAutoSave != true)
        {
            return;
        }

        QuestManager questManager = QuestManager.Instance;
        int currentLevel = questManager.CurrentLevel;

        if (currentLevel < autoSavedFile.currentLevel)
        {
            return;
        }

        if(currentLevel == autoSavedFile.currentLevel)
        {
            SerializableQuestData compare = autoSavedFile.GetQuestData(currentLevel);
            if (questManager.activeQuest.CompletedObjectives < compare.completedObjectives)
            {
                return;
            }
        }

        if(File.Exists(GetSavePath(AUTO_SAVE_SLOT)) != true)
        {
            autoSavedFile = CreateNewSaveData("Auto Save", true);
        }
        SaveGame(autoSavedFile);
    }

    public void SetAutoSaveBool(bool status, SceneStatusManager sceneStatusManager)
    {
        canAutoSave = status;
        this.sceneStatusManager = sceneStatusManager;
    }

    private void OnApplicationQuit()
    {
        AutoSave();
    }
}
