using UnityEngine;

public class SocialMediaComputer : MonoBehaviour, IInteractable
{
    SM_Manager sM_Manager;

    [SerializeField] private string theInteractionText = "";
    public string interactText { get; set; }

    [SerializeField] private GameObject socialM_Canvas;
    public bool isShowingSocial;

    void Start()
    {
        socialM_Canvas.GetComponent<CanvasGroup>().alpha = 1;
        sM_Manager = GetComponent<SM_Manager>();
        HideSocial();
    }

    public void Interact()
    {
        if (!isShowingSocial)
        {
            ShowSocial();
        }
    }

    void ShowSocial()
    {
        socialM_Canvas.SetActive(true);
        isShowingSocial = true;
    }

    public void HideSocial()
    {
        socialM_Canvas.SetActive(false);
        isShowingSocial = false;
    }

    public string GetInteractText()
    {
    	return interactText;
    }
}
