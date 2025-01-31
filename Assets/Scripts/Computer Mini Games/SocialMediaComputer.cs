using UnityEngine;
using UnityEngine.Events;

public class SocialMediaComputer : MonoBehaviour, IInteractable
{
    ComputerPanel_UI sM_Manager;

    [SerializeField] private string theInteractionText = "";
    public string interactText { get; set; }

    [SerializeField] private GameObject socialM_Canvas;
    public bool isShowingSocial;

    public UnityEvent OnInterated;

    void Start()
    {
        socialM_Canvas.GetComponent<CanvasGroup>().alpha = 1;
        sM_Manager = GetComponent<ComputerPanel_UI>();
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
        OnInterated?.Invoke();
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
