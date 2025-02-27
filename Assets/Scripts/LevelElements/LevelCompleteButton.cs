using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LevelCompleteButton : MonoBehaviour
{
    [SerializeField] private bool buttonActive;
    [SerializeField] private int agencySceneIndex;
    [SerializeField] private float loadDelay = 2f;

    [Header("Events")]
    [SerializeField] private UnityEvent _buttonPressed;

    private void OnTriggerEnter(Collider other)
    {
        if (!buttonActive)
        {
            buttonActive = true;
            StartCoroutine(CompleteLevel());
        }
    }

    private IEnumerator CompleteLevel()
    {
        _buttonPressed?.Invoke();
        GameManager.Instance.MissionComplete();
        
        yield return new WaitForSeconds(loadDelay);
        
        LevelLoader.LoadLevel(agencySceneIndex);
    }
}