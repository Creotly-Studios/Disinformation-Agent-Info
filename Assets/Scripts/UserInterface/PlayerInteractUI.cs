using TMPro;
using UnityEngine;

public class PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private GameObject interactUI;
    [SerializeField] private TextMeshProUGUI interactText;
    
    void Start()
    {
        Player_v2.Instance.OnInteractObjectFind += Player_PlayerHasInteractableObject;
        Hide();
    }

    private void Player_PlayerHasInteractableObject(object sender, GameObject e)
    {
        if (e != null)
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
