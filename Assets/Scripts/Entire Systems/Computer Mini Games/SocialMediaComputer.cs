using UnityEngine;
using UnityEngine.Events;

public class SocialMediaComputer : MonoBehaviour, IInteractable
{
    private Player_v2 _player;
    
    ComputerPanel_UI sM_Manager;

    public bool isShowingSocial;

    [SerializeField] private string interactText;
    [SerializeField] private GameObject socialM_Canvas;
    public QuestObjectiveNavIdentifier identifier { get; private set; }
    

    public UnityEvent OnInterated;

    private void Awake()
    {
        sM_Manager = GetComponent<ComputerPanel_UI>();
        socialM_Canvas.GetComponent<CanvasGroup>().alpha = 1;
        identifier = GetComponent<QuestObjectiveNavIdentifier>();
    }

    void Start()
    {
        HideSocial();
    }

    public void Interact(Player_v2 player)
    {
        _player = player;
        if (!isShowingSocial)
        {
            ShowSocial();
        }
        DeactivatePlayer();
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
    
    public void ActivatePlayer()
    { 
        if (_player != null) _player.SetActiveState();
    }

    public void DeactivatePlayer()
    { 
        if (_player != null) _player.SetInactiveState();
    }
}
