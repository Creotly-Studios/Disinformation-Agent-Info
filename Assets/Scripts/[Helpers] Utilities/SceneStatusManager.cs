using UnityEngine;

public class SceneStatusManager : MonoBehaviour
{
    [SerializeField] private bool canAutoSave;

    private void OnEnable()
    {
        EventBus.Save.OnSetSceneAutoSave?.Invoke(canAutoSave);
    }

    private void OnDisable()
    {
        EventBus.Save.OnSetSceneAutoSave?.Invoke(false);
    }
}