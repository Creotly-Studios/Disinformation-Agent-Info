using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManagerSystem : MonoBehaviour
{
    public static SaveManagerSystem Instance { get; private set; }

    private bool canAutoSave;
    private string saveDirectory;
    private bool shouldCompletedAutoSave;
    
    private SavedData autoSavedFile;
    private const string AUTO_SAVE_SLOT = "AutoSave.agentInfo";
    [field: SerializeField] public SaveMenuUI SaveMenuUI { get; private set;}
    
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

    private void Start()
    {
        InitializeSaveSystem();
    }

    private void Update()
    {
        if(shouldCompletedAutoSave && Player_v2.Instance != null)
        {
            UpdateSavedFile(autoSavedFile);
        }
    }

    private void InitializeSaveSystem()
    {
        saveDirectory = Path.Combine(Application.persistentDataPath, "Saved Instances");
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        string autoSavePath = GetSavePath(AUTO_SAVE_SLOT);
        if(!File.Exists(autoSavePath))
        {
            autoSavedFile = CreateNewSaveData("Auto Save", true);
            SaveGame(autoSavedFile);
            return;
        }
        autoSavedFile = FindSaveData(autoSavePath);
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
    }

    public SavedData LoadGame(SavedData dataToLoad)
    {
        if (dataToLoad == null) { return null; }
        string newDataPath = $"{dataToLoad.fileName}.agentInfo";
        string savePath = GetSavePath(dataToLoad.isAutoSaveFile ? AUTO_SAVE_SLOT : newDataPath);

        if (File.Exists(savePath) != true)
        {
            throw new FileNotFoundException($"Save File Not Found At {savePath}");
        }
        byte[] loadedData = SecureSaveUtility.LoadFromFile(savePath);
        return SaveSerializer.Deserialize(loadedData);
    }

    private string GetSavePath(string fileName)
    {
        return Path.Combine(saveDirectory, fileName);
    }

    public SavedData FindSaveData(string file)
    {
        byte[] rawData = SecureSaveUtility.LoadFromFile(file);
        SavedData loadedData = SaveSerializer.Deserialize(rawData);
        return loadedData;
    }

    public SavedData CreateNewSaveData(string filename, bool isAutoSave = false)
    {
        SavedData newData = new(filename, isAutoSave);
        UpdateSavedFile(newData);
        return newData;
    }

    private void UpdateSavedFile(SavedData newData)
    {
        Player_v2 player = Player_v2.Instance;
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        newData.sceneIndex = sceneIndex;
        newData.modifiedDate = dateTime;

        if (player == null)
        {
            shouldCompletedAutoSave = true;
            return;
        }
        newData.playerPosition = player.transform.position;
        newData.playerRotation = player.transform.rotation;
        newData.coinAmount = GameManager.Instance.PlayerCoinAmount;
        newData.healthCount = player.PlayerStatistics.CurrentHealth;

        QuestManager questManager = QuestManager.Instance;
        if(questManager == null)
        {
            return;
        }

        for (int i = 0; i < questManager.availableQuests.Count; i++)
        {
            QuestSO quest = questManager.availableQuests[i];
            SerializableQuestData exist = newData.questDataList.Find(x => x.questName == quest.questTitle);
            if(exist == null)
            {
                newData.questDataList.Add(new(quest));
            }
            newData.questDataList[i].UpdateQuestData(quest);
        }
        newData.SetPlayerTransformValues();
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
        SaveGame(autoSavedFile);
    }

    public void SetAutoSaveBool(bool status)
    {
        canAutoSave = status;
    }

    private void OnApplicationQuit()
    {
        if(File.Exists(GetSavePath(AUTO_SAVE_SLOT)) != true)
        {
            return;
        }
        AutoSave();
    }
}
