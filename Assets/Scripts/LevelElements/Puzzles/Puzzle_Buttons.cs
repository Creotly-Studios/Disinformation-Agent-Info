using UnityEngine;
using UnityEngine.Events;

public class Puzzle_Buttons : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent _buttonPressed;
    [SerializeField] private UnityEvent _buttonReleased;
    
    [Header("Indication")]
    [SerializeField] private Material onMat;
    [SerializeField] private Material offMat;
    [SerializeField] private MeshRenderer indicatorMesh;

    private void OnTriggerEnter(Collider other) 
    {
        indicatorMesh.material = onMat;
        _buttonPressed?.Invoke();
    }

    private void OnTriggerExit(Collider other) 
    {
        indicatorMesh.material = offMat;
        _buttonReleased?.Invoke();
    }
}
