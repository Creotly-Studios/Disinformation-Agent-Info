using UnityEngine;
using UnityEngine.Events;

public class Puzzle_Switches : MonoBehaviour, IInteractable
{
    private PuzzleManager puzzleManager;
    public bool Switch { get; private set; }

    [Header("Events")]
    [SerializeField] private UnityEvent _switchOn;
    [SerializeField] private UnityEvent _switchOff;
    [SerializeField] private UnityEvent switchInteract;

    [Header("Indication")]
    [SerializeField] private Material onMat;
    [SerializeField] private Material offMat;
    [SerializeField] private MeshRenderer indicatorMesh;
    [SerializeField] private QuestObjectiveNavIdentifier navIdentifier;

    [Header("Status")]
    [SerializeField] private Switch_Type switchType;
    [SerializeField] private Transform switchHandle;
    [SerializeField] private string interactText = "Switch";
    [SerializeField] private Vector3 handleRotationTrue = new Vector3(0, 0, 50);


    private void Start()
    {
        Switch = false;
        indicatorMesh.material = offMat;
        SetSwitchHandleRotation(false);
        puzzleManager = GetComponentInParent<PuzzleManager>();
    }

    public string GetInteractText()
    {
        return interactText;
    }

    public void Interact(Player_v2 player)
    {
        if(Switch == true)
        {
            Switch = false;
            UpdateSwitch(offMat, _switchOff);
        }
        else
        {
            Switch = true;
            UpdateSwitch(onMat, _switchOn);
        }
        switchInteract?.Invoke();
    }

    private void UpdateSwitch(Material mat, UnityEvent uEvent)
    {
        uEvent?.Invoke();
        UpdateObjective(Switch);
        indicatorMesh.material = mat;
        SetSwitchHandleRotation(Switch);
    }

    private void SetSwitchHandleRotation(bool _switch_B)
    {
        if (_switch_B == true)
        {
            switchHandle.eulerAngles = handleRotationTrue;
        }
        else switchHandle.eulerAngles = handleRotationTrue * -1;
    }

    private void UpdateObjective(bool status)
    {
        QuestManager questManager = QuestManager.Instance;
        QuestObjectives objective = questManager.activeQuest.FindQuestObjective(ObjectiveType.Puzzle);
        if(objective == null)
        {
            return;
        }

        QuestSO quest = questManager.activeQuest;
        QuestObjectiveNavIdentifier identifier = (puzzleManager != null) ? puzzleManager.identifier : navIdentifier;
        
        if (switchType == Switch_Type.Main)
        {
            if (status != true)
            {
                quest.DecreaseQuestObjectiveProgressLevels(objective, identifier);
                return;
            }
            quest.IncreaseQuestObjectiveProgressLevels(objective, identifier);
        }
        else if (switchType == Switch_Type.Blockers)
        {
            if(status)
            {
                quest.DecreaseQuestObjectiveProgressLevels(objective, identifier);
                return;
            }
            quest.IncreaseQuestObjectiveProgressLevels(objective, identifier);
        }
    }
}
