using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class Puzzle_Switches : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText = "Switch";
    public bool Switch {get; private set;}

    [Header("Eventsa")]
    [SerializeField] private UnityEvent switchInteract;
    [SerializeField] private UnityEvent _switchOn;
    [SerializeField] private UnityEvent _switchOff;
    
    [Header("Indication")]
    [SerializeField] private Material onMat;
    [SerializeField] private Material offMat;
    [SerializeField] private MeshRenderer indicatorMesh;
    [Space]
    [SerializeField] private Transform switchHandle;
    [SerializeField] private Vector3 handleRotationTrue = new Vector3(0, 0, 50);


    private void Start() {
        Switch = false;
        indicatorMesh.material = offMat;
        SetSwitchHandleRotation(false);
    }

    public string GetInteractText()
    {
        return interactText;
    }

    public void Interact(Player_v2 player)
    {
        if (Switch == true)
        {
            _switchOff?.Invoke();
             indicatorMesh.material = offMat;
            SetSwitchHandleRotation(false);
            Switch = false;
        } else if (Switch == false)
        {
            _switchOn?.Invoke();
             indicatorMesh.material = onMat;
             SetSwitchHandleRotation(true);
            Switch = true;
        }
        
        switchInteract?.Invoke();
    }

    void SetSwitchHandleRotation(bool _switch_B)
    {
        if (_switch_B == true)
        { 
            switchHandle.eulerAngles = handleRotationTrue;  
        } else switchHandle.eulerAngles = handleRotationTrue * -1;  
    }








}
