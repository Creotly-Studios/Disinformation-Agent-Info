using UnityEngine;
using UnityEngine.Events;

public class UnityEventHandler : MonoBehaviour
{
    public UnityEvent unityEvent;
   
    public void InvokeEvent()
    {
        unityEvent?.Invoke();
    }
}
