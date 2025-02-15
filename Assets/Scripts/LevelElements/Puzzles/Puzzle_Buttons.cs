using UnityEngine;
using UnityEngine.Events;

public class Puzzle_Buttons : MonoBehaviour
{
    [SerializeField] private string interactText = "Button";
    [SerializeField] private bool buttonActive;

    [Header("Events")]
    [SerializeField] private UnityEvent _buttonPressed;
    [SerializeField] private UnityEvent _buttonReleased;
    
    [Header("Indication")]
    [SerializeField] private Material onMat;
    [SerializeField] private Material offMat;
    [SerializeField] private MeshRenderer indicatorMesh;

    private void OnTriggerEnter(Collider other) {
        indicatorMesh.material = onMat;
        buttonActive = true;
        _buttonPressed?.Invoke();
    }

    private void OnTriggerStay(Collider other) {
        buttonActive = true;
    }

    private void OnTriggerExit(Collider other) {
        indicatorMesh.material = offMat;
        buttonActive = false;
        _buttonReleased?.Invoke();
    }
}
