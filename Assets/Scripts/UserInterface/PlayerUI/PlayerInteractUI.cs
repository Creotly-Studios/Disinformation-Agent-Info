using TMPro;
using UnityEngine;

public class PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private GameObject interactUI;
    [SerializeField] private TextMeshProUGUI interactText;
    private Player_v2 player;

    void Start()
    {
        if (Player_v2.Instance != null)
        {
            player = Player_v2.Instance;
            player.OnInteractObjectFind += Player_PlayerHasInteractableObject;
        }
        Hide();
    }

    private void Player_PlayerHasInteractableObject(object sender, GameObject e)
    {
        if (e != null && player.StateMachine.CurrentState != player.InactiveState)
        {
            if (e.TryGetComponent(out IInteractable interactable))
            {
                Show();
                interactText.text = interactable.GetInteractText();
            }
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

    void OnDestroy()
    {
        if (player != null)
        {
            player.OnInteractObjectFind -= Player_PlayerHasInteractableObject;
        }
    }
}
