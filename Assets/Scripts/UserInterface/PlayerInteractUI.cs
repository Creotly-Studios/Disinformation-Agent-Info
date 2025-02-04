using UnityEngine;
public class PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private GameObject interactUI;
    
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
