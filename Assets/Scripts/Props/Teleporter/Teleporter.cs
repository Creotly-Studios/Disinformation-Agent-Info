using UnityEngine;

public class Teleporter : MonoBehaviour, IInteractable
{
    private Player_v2 _player;

    public GameObject identifier;
    [SerializeField] private string interactText;

    [Header("UI")] 
    [SerializeField] private GameObject teleporterUiPanel;
    
    void Start()
    {
        teleporterUiPanel.SetActive(false);
    }
    
    public void Interact(Player_v2 player)
    {
        _player = player;
        teleporterUiPanel.SetActive(true);
        DeactivatePlayer();
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
