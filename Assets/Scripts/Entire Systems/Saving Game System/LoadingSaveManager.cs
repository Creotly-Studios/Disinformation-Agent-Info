using UnityEngine;
using System.Collections;

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
        player.StartCoroutine(ResetListeners());
        player.transform.SetPositionAndRotation(dataToLoad.playerPosition, dataToLoad.playerRotation);

        GameManager.Instance.SetCoinAmount(dataToLoad.coinAmount);
        player.PlayerStatistics.SetCurrentHealth(dataToLoad.healthCount);
        QuestManager.Instance.RestoreQuestProgress(dataToLoad.questDataList);
        Debug.Log($"{dataToLoad.fileName} Loaded Succesfully");
    }
}
