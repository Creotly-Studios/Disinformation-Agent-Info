using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LevelCompleteButton : MonoBehaviour
{
    [SerializeField] private bool buttonActive;
    [SerializeField] private int agencySceneIndex;

    [Header("Events")]
    [SerializeField] private UnityEvent _buttonPressed;

    private void OnTriggerEnter(Collider other)
    {
        if (!buttonActive)
        {
            buttonActive = true;
            _buttonPressed?.Invoke();
        }
    }
}