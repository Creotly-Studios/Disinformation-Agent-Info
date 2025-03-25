using UnityEngine;

public class QuestObjectiveNavIdentifier : MonoBehaviour
{
    public bool IsCompleted { get; private set; }

    [SerializeField] private GameObject identifier;
    [field: SerializeField] public ObjectiveType ObjType { get; private set; }

    private void Start()
    {
        IsCompleted = false;
        Player_v2.Instance.PlayerNav.RegisterIdentifier(this);
    }

    public void SetObjectiveType(ObjectiveType objectiveType)
    {
        ObjType = objectiveType;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void EnableIdentifierObjStatus()
    {
        identifier.SetActive(!IsCompleted);
    }

    public void MarkCompleted()
    {
        IsCompleted = true;
        EnableIdentifierObjStatus();
    }
}
