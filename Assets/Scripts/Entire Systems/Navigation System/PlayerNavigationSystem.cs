using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class PlayerNavigationSystem : MonoBehaviour
{
    private Player_v2 player;
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
        Vector3 directionToPlayer = (player.transform.position - identifier.GetPosition());
        int distance = Mathf.RoundToInt(directionToPlayer.magnitude);

        distanceToObject.text = $"{distance}M";
        identifier.EnableIdentifierObjStatus();

        //Rotate image UI to target direction;
        directionToPlayer.Normalize();
        Vector2 direction2D = new Vector2(directionToPlayer.x, directionToPlayer.z);
        float angle = Mathf.Atan2(direction2D.x, direction2D.y) * Mathf.Rad2Deg;

        Quaternion rotationAxis = Quaternion.Euler(0, 0, angle);
        directionImage.rotation = HandleRotation(rotationAxis,directionToPlayer);
    }

    private Quaternion HandleRotation(Quaternion rotationAxis, Vector3 direction)
    {
        float dotProduct = Vector3.Dot(player.transform.forward, direction);
        if(dotProduct < 0)
        {
            return Quaternion.Euler(0, 0, 180f) * rotationAxis;
        }
        return rotationAxis;
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
