using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SavedData
{
    [Header("SavedData Paramters")]
    public string fileName;
    public string modifiedDate;
    public bool isAutoSaveFile;

    [Header("Player Data")]
    public int coinAmount;
    public int healthCount;
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    [Header("Quest Data")]
    public int sceneIndex;
    public List<SerializableQuestData> questDataList = new();

    //Un-Modifiable Properties for Cleaner Text
    public float[] playerPos { get; private set; }
    public float[] playerRot { get; private set; }

    public SavedData(string name, bool isAutoSave)
    {
        fileName = name;
        isAutoSaveFile = isAutoSave;
    }

    public SavedData(int scene, string name, string date, bool isAutoSave)
    {
        fileName = name;
        modifiedDate = date;

        sceneIndex = scene;
        isAutoSaveFile = isAutoSave;
    }

    public void SetPlayerTransformValues()
    {
        playerPos = new float[] {playerPosition.x, playerPosition.y, playerPosition.z};
        playerRot = new float[] {playerRotation.x, playerRotation.y, playerRotation.z, playerRotation.w};
    }
}

[System.Serializable]
public class SerializableQuestData
{
    public string questName;
    public List<int> objectiveProgressvalue = new();

    public SerializableQuestData(QuestSO quest)
    {
        questName = quest.questTitle;
        for(int i = 0; i < quest.questObjectives.Count; i++)
        {
            int progressvalues = quest.questObjectives[i].progressValue;
            objectiveProgressvalue.Add(progressvalues);
        }
    }

    public SerializableQuestData(string questName, List<int> objectiveProgressvalue)
    {
        this.questName = questName;
        this.objectiveProgressvalue = objectiveProgressvalue;
    }

    public void RestoreQuestValues(QuestSO quest)
    {
        for(int i = 0; i < objectiveProgressvalue.Count; i++)
        {
            quest.questObjectives[i].LoadProgressValue(objectiveProgressvalue[i]);
        }
        quest.CheckIfQuestIsComplete();
    }
}
