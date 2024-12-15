using UnityEngine;

public interface IInteractable
{
    string interactText { get; set; }
    public void Interact();
    public string GetInteractText();
}
