using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private string theInteractionText = "";
    public string interactText { get; set; }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactText = theInteractionText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Interact()
    {
        Debug.Log("Hello there, I'm " + gameObject.name);
    }

    public string GetInteractText()
    {
        return interactText;
    }
}
