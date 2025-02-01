using UnityEngine;

public interface IInteractable
{
    string interactText { get; set; }
    // public void Interact();
    public void Interact(Player_v2 player);
    public string GetInteractText();
}
