using UnityEngine;

public class DoorLevelLoader : MonoBehaviour, IInteractable
{
  	
    public string interactText = "Exit Door";
    
    [Space]
	[SerializeField] private int sceneLoadIndex = 0;

    public void Interact(Player_v2 player)
    {
        PlayDoorSound();
        LevelLoader.LoadLevel(sceneLoadIndex);
    }

    public string GetInteractText()
    {
    	return interactText;
    }

    public void PlayDoorSound()
    {

    }
}
