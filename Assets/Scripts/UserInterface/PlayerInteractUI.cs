using TMPro;
using UnityEngine;

public class PlayerInteractUI : MonoBehaviour
{
    Player _player;
    private PlayerInteraction _playerInteraction;

    [SerializeField] private GameObject interactUI;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerInteraction = _player.PlayerInteraction;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInteraction.PlayerCanInteract())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    
    void Hide()
    {
        interactUI.SetActive(false);
    }

    void Show()
    {
        interactUI.SetActive(true);
    }
    
    
}
