using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class LoadingSaveManager
{
    public static void LoadGame(SavedData dataToLoad)
    {
        SavedData data = SaveManagerSystem.Instance.LoadGame(dataToLoad);

        if(data == null)
        {
            Debug.LogWarning("No File Found");
        }
        SaveManagerSystem.Instance.SaveMenuUI.DisablePanel();
        LevelLoader.LoadLevel(dataToLoad.sceneIndex, () => ApplyLoadedData(data));
    }

    private static IEnumerator ResetListeners()
    {
        SaveManagerSystem.Instance.SaveMenuUI.InitializeButtons(false);

        yield return new WaitForSeconds(0.5f);
        SaveManagerSystem.Instance.SaveMenuUI.InitializeButtons(true);
    }

    private static void ApplyLoadedData(SavedData dataToLoad)
    {
        Player_v2 player = Player_v2.Instance;
        if(player == null)
        {
            Debug.LogError("No Player In Scene");
            return;
        }
        LoadSavedAssets(dataToLoad.saveableAssets);

        player.StartCoroutine(ResetListeners());
        QuestManager.Instance.RestoreQuestProgress(dataToLoad.questDataList);
        Debug.Log($"{dataToLoad.fileName} Loaded Succesfully");
    }

    private static void LoadSavedAssets(List<ObjectSaveData> savedDatas)
    {
        ISaveable[] Saveables = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveable>().ToArray();
        foreach (ISaveable saveable in Saveables)
        {
            ObjectSaveData saveData = savedDatas.Find(x => x.name == saveable.GetSaveData().name)
                ?? throw new Exception("No Save Data Available");
            saveable.ReloadDataFromSavedFile(saveData);
        }
        GameObject.FindFirstObjectByType<SceneStatusManager>().ReloadEnemies();
    }
}
