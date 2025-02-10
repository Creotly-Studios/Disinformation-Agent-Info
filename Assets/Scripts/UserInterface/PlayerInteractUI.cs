using TMPro;
using UnityEngine;

public class PlayerInteractUI : MonoBehaviour
{
    Player _player;
    private PlayerInteraction _playerInteraction;

    [SerializeField] private GameObject interactUI;
    [SerializeField] private TextMeshProUGUI interactText;
    
    void Start()
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
            interactText.text = e.GetComponent<IInteractable>().GetInteractText();
        }
        else
        {
            interactText.text = "";
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
