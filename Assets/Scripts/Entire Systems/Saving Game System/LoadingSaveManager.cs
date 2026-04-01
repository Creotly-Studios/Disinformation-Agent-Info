using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class LoadingSaveManager
{
    public static void LoadGame(SavedData dataToLoad)
    {
        SavedData data = SaveManagerSystem.Instance.LoadGame(dataToLoad);
        if (data == null)
        {
            Debug.LogWarning("[LoadingSaveManager] LoadGame returned null — aborting.");
            return;
        }

        SaveManagerSystem.Instance.SaveMenuUI.DisablePanel();
        LevelLoader.LoadLevel(dataToLoad.sceneIndex, () => ApplyLoadedData(data));
    }

    private static void ApplyLoadedData(SavedData dataToLoad)
    {
        Player_v2 player = Player_v2.Instance;
        if (player == null)
        {
            Debug.LogError("[LoadingSaveManager] No Player in scene after load.");
            return;
        }

        LoadSavedAssets(dataToLoad.saveableAssets);
        player.StartCoroutine(ResetListeners());
        QuestManager.Instance.RestoreQuestProgress(dataToLoad.questDataList);
        Debug.Log($"[LoadingSaveManager] '{dataToLoad.fileName}' loaded successfully.");
    }

    private static void LoadSavedAssets(List<ObjectSaveData> savedDatas)
    {
        ISaveable[] saveables = GameObject
            .FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISaveable>()
            .ToArray();

        foreach (ISaveable saveable in saveables)
        {
            string key = saveable.GetSaveData().name;
            ObjectSaveData match = savedDatas.Find(x => x.name == key);

            if (match == null)
            {
                // Bug fix: was ?? throw — any new ISaveable added since the save was
                //          written would hard-crash the entire load. Warn and skip instead.
                Debug.LogWarning($"[LoadingSaveManager] No saved data found for '{key}'. Skipping.");
                continue;
            }

            saveable.ReloadDataFromSavedFile(match);
        }
    }

    // Brief listener reset to avoid double-firing after scene reload.
    private static IEnumerator ResetListeners()
    {
        SaveManagerSystem.Instance.SaveMenuUI.SetupButtonListeners(false);
        yield return new WaitForSeconds(0.5f);
        SaveManagerSystem.Instance.SaveMenuUI.SetupButtonListeners(true);
    }
}