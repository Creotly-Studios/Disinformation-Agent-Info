using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FactPopUpUI : MonoBehaviour
{
    [SerializeField] GameObject popupPanel; // Assign the Panel in Inspector
    [SerializeField] TextMeshProUGUI factText; // Assign the TextMeshPro UI element
    [SerializeField] float displayTime = 3f; // Time the fact stays on screen

    [SerializeField] string[] facts = new string[]
    {
        "Fake news spreads six times faster than real news.\nLesson: Always verify information before sharing it.",
        "Misinformation often appeals to emotions rather than facts.\nLesson: If a post makes you feel extreme emotions, fact-check before reacting.",
        "Bots and fake accounts generate a large percentage of online disinformation.\nLesson: Be cautious of posts from unknown or suspicious sources.",
        "Deepfake technology can create realistic but fake videos of real people.\nLesson: Not everything you see in a video is real—check trusted sources.",
        "Confirmation bias makes people believe misinformation that supports their views.\nLesson: Stay open-minded and seek different perspectives before believing a claim.",
        "Satire and parody can sometimes be mistaken for real news.\nLesson: Check if a news source is meant to be a joke before sharing.",
        "Photos and videos can be edited or taken out of context to mislead people.\nLesson: Reverse-search images or look for full context before trusting visuals.",
        "Fact-checking websites can help verify if a news story is real or fake.\nLesson: Use sites like Snopes, FactCheck.org, or local fact-checkers to verify claims.",
        "Fake news is often designed to trigger strong emotional reactions like fear or anger.\nLesson: Be skeptical of news that tries to make you panic or get angry.",
        "A misinformation campaign can be used to influence elections or public opinion.\nLesson: Question viral political claims, especially during elections.",
        "Clickbait headlines are designed to grab attention, but they often mislead readers.\nLesson: Read beyond the headline before believing or sharing.",
        "AI-generated text can create realistic fake articles that spread disinformation.\nLesson: Check if a website or author is credible before trusting an article.",
        "Not everything that trends on social media is true—viral doesn’t mean verified.\nLesson: Popularity does not equal truth—verify before spreading.",
        "Cognitive overload from too much information can make people believe fake news.\nLesson: Take breaks from news and think critically before believing everything you read.",
        "A well-placed hoax can fool millions before it’s debunked.\nLesson: Just because a claim is widely believed doesn’t mean it’s true—always double-check."
    };

    
    string randomFact;


    [Space]
    [SerializeField] private UnityEvent onClosePopup;

    private void Start()
    {
        popupPanel.SetActive(false);
    }

    public void ShowRandomFact()
    {
        randomFact = facts[Random.Range(0, facts.Length)];
        StartCoroutine(DisplayFact());
    }

    private IEnumerator DisplayFact()
    {
        factText.text = randomFact;
        popupPanel.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        onClosePopup?.Invoke();
        popupPanel.SetActive(false);
    }
}
