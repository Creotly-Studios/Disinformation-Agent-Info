using UnityEngine;
using UnityEngine.Events;

public class TalkToNPCGoals : MonoBehaviour
{
    public int maxCount = 2;
    public int currentCount = 0;

    public UnityEvent eventFinishTalk;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DialogueManager.Instance.OnDialogueEnd.AddListener(AddAndCheckTalk);
    }

    void AddAndCheckTalk()
    {
        currentCount++;
        if (currentCount >= maxCount)
        {
            eventFinishTalk?.Invoke();
        }
    }
}
