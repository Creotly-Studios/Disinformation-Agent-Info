using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Interact()
    {
        Debug.Log("Hello there, I'm " + gameObject.name);
    }
}
