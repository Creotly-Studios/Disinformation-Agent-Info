using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class PlayerNavigationSystem : MonoBehaviour
{
    private Player_v2 player;
    private readonly List<QuestObjectiveNavIdentifier> identifierList = new();
    private readonly List<QuestObjectiveNavIdentifier> filteredIdentifiers = new();

    [Header("UI Fields")]
    [SerializeField] private RectTransform directionImage;
    [SerializeField] private TextMeshProUGUI distanceToObject;

    private void Awake() => player = GetComponent<Player_v2>();

    private void OnEnable()
    {
        EventBus.Quest.OnActiveQuestChanged += OnActiveQuestChanged;
        EventBus.Quest.OnNavigationRefreshNeeded += OnNavigationRefreshNeeded;
    }

    private void OnDisable()
    {
        EventBus.Quest.OnActiveQuestChanged -= OnActiveQuestChanged;
        EventBus.Quest.OnNavigationRefreshNeeded -= OnNavigationRefreshNeeded;
    }

    private void OnActiveQuestChanged(bool _, QuestSO quest)
    {
        if (quest == null)
        {
            return;
        }
        HandleIdentifierFilter(quest.FindNextObjective());
    }

    private void OnNavigationRefreshNeeded(QuestObjective _)
    {
        QuestSO quest = QuestManager.Instance.ActiveQuest;
        if (quest != null) HandleIdentifierFilter(quest.FindNextObjective());
    }

    // ── Update ────────────────────────────────────────────────────────────────

    private void Update()
    {
        HandleNavigation();
        identifierList.RemoveAll(x => x.IsCompleted);
        filteredIdentifiers.RemoveAll(x => x.IsCompleted);
    }

    // ── Navigation ────────────────────────────────────────────────────────────

    private void HandleNavigation()
    {
        QuestObjectiveNavIdentifier target = ClosestIdentifier();
        if (target != null) DirectPlayerTo(target);
    }

    public void HandleIdentifierFilter(QuestObjective currentObjective)
    {
        if (currentObjective == null) return;
        filteredIdentifiers.Clear();
        foreach (QuestObjectiveNavIdentifier id in identifierList)
        {
            if (!filteredIdentifiers.Contains(id) && id.ObjType == currentObjective.objectiveType)
                filteredIdentifiers.Add(id);
        }
    }

    private QuestObjectiveNavIdentifier ClosestIdentifier()
    {
        float minDist = float.MaxValue;
        QuestObjectiveNavIdentifier closest = null;
        foreach (QuestObjectiveNavIdentifier id in filteredIdentifiers)
        {
            float dist = Vector3.SqrMagnitude(id.GetPosition() - player.transform.position);
            if (dist < minDist) { minDist = dist; closest = id; }
        }
        return closest;
    }

    private void DirectPlayerTo(QuestObjectiveNavIdentifier identifier)
    {
        Vector3 fwd = transform.forward;
        Vector3 targetDir = identifier.GetPosition() - player.transform.position;
        int distance = Mathf.FloorToInt(targetDir.magnitude);

        fwd.y = targetDir.y = 0f;
        Vector2 fwd2D = new Vector2(fwd.x, fwd.z).normalized;
        Vector2 target2D = new Vector2(targetDir.x, targetDir.z).normalized;

        directionImage.localEulerAngles = new Vector3(0f, 0f, -Vector2.SignedAngle(fwd2D, target2D));
        distanceToObject.text = $"{distance}M";
        identifier.EnableIdentifierObjStatus();
    }

    public void RegisterIdentifier(QuestObjectiveNavIdentifier identifier)
    {
        if (!identifierList.Contains(identifier) && !identifier.IsCompleted)
            identifierList.Add(identifier);
    }
}