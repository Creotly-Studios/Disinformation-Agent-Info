using UnityEngine;
using UnityEngine.Events;

public class Coin : MonoBehaviour, ISaveable
{
    private bool hasCollected;
    private ObjectSaveData saveData;
    [SerializeField] private UnityEvent pickUpEvent;

    private void Start()
    {
        hasCollected = false;
        saveData = new ObjectSaveData
        {
            name = gameObject.name
        };
        EventBus.Save.OnRegisterSaveableAsset?.Invoke(this);
    }

    public ObjectSaveData GetSaveData()
    {
        return saveData;
    }

    public void ReloadDataFromSavedFile(ObjectSaveData saveData)
    {
        hasCollected = saveData.SwitchStatus;
        gameObject.SetActive(!hasCollected);
    }

    public void UpdateSavedData()
    {
        saveData.UpdateSaveData(transform.position, transform.rotation, hasCollected);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player_v2>().CallPlayerCoinPickup();
            pickUpEvent?.Invoke();
            hasCollected = true;
            gameObject.SetActive(!hasCollected);
        }
    }

}
