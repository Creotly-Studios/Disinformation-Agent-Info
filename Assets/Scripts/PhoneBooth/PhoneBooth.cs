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

    public bool activatedByDialogue;

    void Start()
    {
        SetActiveByDialogue();
    }

    public void Interact()
    {
        if (canBeUsed)
        {
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
        SetUsage(true);
    }

    public void SetActiveByDialogue()
    {
        if (DialogueManager.Instance != null && activatedByDialogue)
        {
            DialogueManager.Instance.OnDialogueEnd.AddListener(ActivateBooth);
        }
    }

}
