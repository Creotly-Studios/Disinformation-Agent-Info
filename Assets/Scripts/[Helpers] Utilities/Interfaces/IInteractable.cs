using UnityEngine;

public interface IInteractable
{
    public void Interact(Player_v2 player);
    public string GetInteractText();
}

public interface IQuestObject
{
    public QuestObjectiveNavIdentifier GetNavIdentifier();
    public void HandleDetection(PlayerNavigationSystem navigation);
}
