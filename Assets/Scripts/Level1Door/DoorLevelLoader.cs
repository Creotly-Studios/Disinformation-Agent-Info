using UnityEngine;

public class DoorLevelLoader : MonoBehaviour, IInteractable
{
  	[SerializeField] private string theInteractionText = "Exit Door";
    public string interactText { get; set; }
    
    [Space]
	private int sceneLoadIndex = 0;

    public void Interact()
    {
        LevelLoader.LoadLevel(sceneLoadIndex);
    }

    public string GetInteractText()
    {
    	return interactText;
    }
}
