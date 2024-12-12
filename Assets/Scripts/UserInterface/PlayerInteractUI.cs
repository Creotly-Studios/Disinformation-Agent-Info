using TMPro;
using UnityEngine;

public class PlayerInteractUI : MonoBehaviour
{
    Player player;

    [SerializeField] private GameObject interactUI;
    [SerializeField] private TextMeshProUGUI interactObjText; 
        
    private PlayerInteraction _playerInteraction;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerInteraction = player.PlayerInteraction;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInteraction.GetInteractableObject() != null)
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
        interactObjText.text = "";
        interactUI.SetActive(false);
    }

    void Show()
    {
        interactObjText.text = _playerInteraction.GetInteractableObject().GetComponent<IInteractable>().GetInteractText();
        interactUI.SetActive(true);
    }
    
    
}
