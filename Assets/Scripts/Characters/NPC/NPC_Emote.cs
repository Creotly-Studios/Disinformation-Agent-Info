using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NPC_Emote : MonoBehaviour
{
    public Image displayImg; // Reference to the UI Image component

    [System.Serializable]
    public struct Emotion
    {
        public Emotions emotion;
        public Sprite emotionSprite;
    }

    // Dictionary to store emotion-sprites mappings
    public Dictionary<Emotions, Sprite> emotionSprites = new Dictionary<Emotions, Sprite>();

    // List of emotions to populate in the Inspector
    public List<Emotion> emotions;

    private Emotions _currentEmotion;
    public Emotions CurrentEmotion
    {
        get => _currentEmotion;
        set
        {
            if (_currentEmotion != value)
            {
                _currentEmotion = value;
                UpdateEmotionDisplay();
            }
        }
    }

    private void Awake()
    {
        // Populate the dictionary from the list
        foreach (var emotion in emotions)
        {
            if (!emotionSprites.ContainsKey(emotion.emotion))
            {
                emotionSprites.Add(emotion.emotion, emotion.emotionSprite);
            }
        }
    }

    private void UpdateEmotionDisplay()
    {
        if (displayImg != null && emotionSprites.ContainsKey(CurrentEmotion))
        {
            displayImg.sprite = emotionSprites[CurrentEmotion];
        }
    }

    // Public method to set the current emotion
    public void SetCurrentEmotion(Emotions newEmotion)
    {
        CurrentEmotion = newEmotion;
    }
}


public enum Emotions
{
    Exclaiming, Happy, Sad, Angry, Confused, Alert, Hungry, Dizzy, Heartbroken, Singing, Laughing, Star
}