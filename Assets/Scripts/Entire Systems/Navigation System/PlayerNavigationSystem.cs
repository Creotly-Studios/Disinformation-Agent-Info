using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class PlayerNavigationSystem : MonoBehaviour
{
    private Player_v2 player;
    private Transform cameraObject;
    public bool canResetFilterNavList;

    private List<QuestObjectiveNavIdentifier> identifierList = new();
    private List<QuestObjectiveNavIdentifier> filteredIdentifiers = new();

    [Header("UI Fields")]
    [SerializeField] private RectTransform directionImage;
    [SerializeField] private TextMeshProUGUI distanceToObject;

    private void Awake()
    {
        player = GetComponent<Player_v2>();
        canResetFilterNavList = true;
    }

    private void Start()
    {
        cameraObject = Camera.main.transform;
    }

    public void Update()
    {
        if(canResetFilterNavList)
        {
            QuestSO quest = QuestManager.Instance.activeQuest;
            if(quest != null) { HandleIdentifierFilter(quest.FindNextObjective()); }
            if(filteredIdentifiers.Count != 0)
            {
                canResetFilterNavList = false;
            }
            return;
        }

        HandleNavigation();
        identifierList.RemoveAll(x => x.IsCompleted);
        filteredIdentifiers.RemoveAll(x => x.IsCompleted);
    }

    private void HandleNavigation()
    {
        QuestObjectiveNavIdentifier mainIdentifier = ClosestIdentifier();
        if (mainIdentifier != null) { DirectPlayerTo(mainIdentifier); }
    }

    public void HandleIdentifierFilter(QuestObjectives currentObjective)
    {
        if(currentObjective == null)
        {
            return;
        }

        filteredIdentifiers.Clear();
        for(int i = 0; i < identifierList.Count; i++)
        {
            QuestObjectiveNavIdentifier identifier = identifierList[i];
            if(filteredIdentifiers.Contains(identifier) || identifier.ObjType != currentObjective.objectiveType)
            {
                continue;
            }
            filteredIdentifiers.Add(identifier);
        }
    }

    private QuestObjectiveNavIdentifier ClosestIdentifier()
    {
        float minDistance = float.MaxValue;
        QuestObjectiveNavIdentifier closest = null;

        for(int i = 0; i < filteredIdentifiers.Count; i++)
        {
            QuestObjectiveNavIdentifier identifier = filteredIdentifiers[i];
            float distance = Vector3.SqrMagnitude(identifier.GetPosition() - player.transform.position);

            if(distance < minDistance)
            {
                minDistance = distance;
                closest = identifier;
            }
        }
        return closest;
    }

    private void DirectPlayerTo(QuestObjectiveNavIdentifier identifier)
    {
        Vector3 fwd = transform.forward;
        Vector3 targetDir = identifier.GetPosition() - player.transform.position;
        int distance = Mathf.FloorToInt(targetDir.magnitude);

        fwd.y = targetDir.y = 0.0f;
        Vector2 fwd2D = new Vector2(fwd.x, fwd.z).normalized;
        Vector2 targetDir2D = new Vector2(targetDir.x, targetDir.z).normalized;

        float angle = Vector2.SignedAngle(fwd2D, targetDir2D);
        
        directionImage.localEulerAngles = new Vector3(0, 0, -angle);
        distanceToObject.text = $"{distance}M";
        identifier.EnableIdentifierObjStatus();
    }

    public void RegisterIdentifier(QuestObjectiveNavIdentifier identifier)
    {
        if(identifierList.Contains(identifier) || identifier.IsCompleted)
        {
            return;
        }
        identifierList.Add(identifier);
    }
}
