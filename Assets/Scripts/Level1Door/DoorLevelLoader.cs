using UnityEngine;

public class DoorLevelLoader : MonoBehaviour, IInteractable
{
  	[SerializeField] private string theInteractionText = "Exit Door";
    public string interactText { get; set; }
    
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
        SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.sfxList.interactWithDoor);
    }
}
