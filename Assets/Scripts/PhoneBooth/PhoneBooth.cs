using System.Collections;
using UnityEngine;

public class PhoneBooth : MonoBehaviour, IInteractable
{
	[SerializeField] private string theInteractionText = "";
    public string interactText { get; set; }
    [Space]
    [SerializeField] private int sceneLoadIndex;
    [SerializeField] private float sceneLoadDelay = 2f;
    [SerializeField] bool canBeUsed;
    // Scene index to load

    [SerializeField] GameObject mesh;
    [SerializeField] bool meshActiveAtStart;

    public bool activatedByDialogue;

    void Start()
    {
        mesh.SetActive(meshActiveAtStart);
        GetComponent<BoxCollider>().enabled = meshActiveAtStart;
        SetActiveByDialogue();
    }

    public void Interact(Player_v2 player)
    {
        if (canBeUsed)
        {
            PlayPhoneBoothSound();
            StartCoroutine(LoadTheLevel());
        }
    }

    private IEnumerator LoadTheLevel()
    {
        // Example: Add a delay or fade effect before loading the level
        Debug.Log("Loading level with index: " + sceneLoadIndex);

        // Optionally, wait for a brief moment or play an animation
        yield return new WaitForSeconds(sceneLoadDelay);
        LevelLoader.LoadLevel(sceneLoadIndex);
    }

    public string GetInteractText()
    {
    	return interactText;
    }

    public void SetUsage(bool _)
    {
        canBeUsed = _;
    }

    public void ActivateBooth()
    {
        mesh.SetActive(true);
        GetComponent<BoxCollider>().enabled = true;
        SetUsage(true);
    }

    public void SetActiveByDialogue()
    {
        if (DialogueManager.Instance != null && activatedByDialogue)
        {
            DialogueManager.Instance.OnDialogueEnd.AddListener(ActivateBooth);
        }
    }

    public void PlayPhoneBoothSound()
    {
        SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.sfxList.interactWithPhoneBooth);
    }

}
