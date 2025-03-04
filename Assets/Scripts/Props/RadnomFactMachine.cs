using UnityEngine;

public class RadnomFactMachine : MonoBehaviour, IInteractable
{
        private Player_v2 _player;
    [SerializeField] private string interactText = "Fun Fact Machine";
    [SerializeField] private FactPopUpUI fact;


    public void Interact(Player_v2 player)
    {
        _player = player;
        DeactivatePlayer();
        fact.ShowRandomFact();
    }

    public string GetInteractText()
    {
        return interactText;
    }

    public void ActivatePlayer()
    { 
        if (_player != null) _player.SetActiveState();
    }

    public void DeactivatePlayer()
    {
        if (_player != null) _player.SetInactiveState();
    }
}
