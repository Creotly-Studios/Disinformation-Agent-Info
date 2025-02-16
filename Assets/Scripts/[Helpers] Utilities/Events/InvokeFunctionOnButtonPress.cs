using UnityEngine;
using UnityEngine.Events;

public class InvokeFunctionOnButtonPress : MonoBehaviour
{
    public KeyCode key;
    public UnityEvent _event;

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            _event?.Invoke();
        }        
    }
}
