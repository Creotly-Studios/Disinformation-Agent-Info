using UnityEngine;
using DG.Tweening;

public class MainMenuCamera : MonoBehaviour
{
    private Vector3 initialPos;
    public Vector3 startMenuPos;  // Assign the Start Menu position in Inspector
    public Vector3 optionsMenuPos; // Assign the Options Menu position in Inspector
    public float transitionTime = 1f; // Duration of movement

    private void Start()
    {
        initialPos = transform.position;
    }

    public void MoveToStartMenu()
    {
        transform.DOMove(startMenuPos, transitionTime);
    }

    public void MoveToOptionsMenu()
    {
        transform.DOMove(optionsMenuPos, transitionTime);
    }

    public void ResetPosition()
    {
        transform.DOMove(initialPos, transitionTime);
    }
}
