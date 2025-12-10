using UnityEngine;

public class Props : MonoBehaviour, ISaveable
{
    private ObjectSaveData saveData;

    private void Start()
    {
        saveData = new()
        {
            name = name
        };
        SaveManagerSystem.Instance.saveables.Add(this);
    }

    public ObjectSaveData GetSaveData()
    {
        return saveData;
    }

    public void ReloadDataFromSavedFile(ObjectSaveData saveData)
    {
        transform.SetPositionAndRotation(saveData.ObjectPosition, saveData.ObjectRotation);
    }

    public void UpdateSavedData()
    {
        saveData.UpdateSaveData(transform.position, transform.rotation, false);
    }
}
