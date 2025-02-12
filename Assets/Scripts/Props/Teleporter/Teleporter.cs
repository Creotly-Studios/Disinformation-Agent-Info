using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Teleporter : MonoBehaviour, IInteractable
{
    private Player_v2 _player;
    
    [SerializeField] private string interactText;
    [SerializeField] private float sceneLoadDelay = 2f;

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
        if (_player != null) _player.StateMachine.ChangeState(_player.IdleState);
    }

    public void DeactivatePlayer()
    { 
        if (_player != null) _player.StateMachine.ChangeState(_player.InactiveState);
    }
    
}
