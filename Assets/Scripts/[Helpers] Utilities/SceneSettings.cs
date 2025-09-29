using UnityEngine;

public class SceneSettings : MonoBehaviour
{
    private bool hasSet;
    [Header("Scene Parameters")]
    [SerializeField] private bool canAutoSave;

    private void OnEnable()
    {
        if(hasSet)
        {
            return;
        }

        SaveManagerSystem saveManager = SaveManagerSystem.Instance;
        if(saveManager != null)
        {
            saveManager.SetAutoSaveBool(canAutoSave);
        }
        hasSet = true;
    }

    private void OnDisable()
    {
        if(hasSet != true)
        {
            return;
        }
        hasSet = false;
    }
}