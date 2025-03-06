using UnityEngine;
using System.Collections;

public class SwitchPlatforms : MonoBehaviour
{
    [SerializeField] GameObject platform_one;
    [SerializeField] GameObject platform_two;
    [SerializeField] float switchInterval = 2.0f; // Time in seconds between switches
    [SerializeField] float warningDuration = 0.5f; // Time in seconds for the warning indication

    private float timer;
    private bool isWarning;
    private Renderer platformOneRenderer;
    private Renderer platformTwoRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        platform_one.SetActive(true);
        platform_two.SetActive(false);
        timer = switchInterval; // Initialize the timer

        // Get the renderers of the platforms
        platformOneRenderer = platform_one.GetComponent<Renderer>();
        platformTwoRenderer = platform_two.GetComponent<Renderer>();

        isWarning = false;
    }

    // Update is called once per frame
    void Update()
    {
        TogglePlatformsPeriodically();
    }

    void TogglePlatformsPeriodically()
    {
        // Update the timer
        timer -= Time.deltaTime;

        // Check if it's time to show the warning
        if (timer <= warningDuration && !isWarning)
        {
            StartWarning();
        }

        // Check if the timer has reached zero
        if (timer <= 0)
        {
            // Toggle the platforms
            platform_one.SetActive(!platform_one.activeSelf);
            platform_two.SetActive(!platform_two.activeSelf);

            // Reset the timer and warning state
            timer = switchInterval;
            isWarning = false;

            // Ensure platforms are visible after switching
            platformOneRenderer.enabled = true;
            platformTwoRenderer.enabled = true;
        }
    }

    void StartWarning()
    {
        isWarning = true;
        StartCoroutine(BlinkPlatforms());
    }

    IEnumerator BlinkPlatforms()
    {
        float blinkInterval = 0.1f; // Time between blinks
        int blinkCount = Mathf.FloorToInt(warningDuration / blinkInterval);

        for (int i = 0; i < blinkCount; i++)
        {
            // Toggle visibility of the platforms
            platformOneRenderer.enabled = !platformOneRenderer.enabled;
            platformTwoRenderer.enabled = !platformTwoRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        // Ensure platforms are visible before switching
        platformOneRenderer.enabled = true;
        platformTwoRenderer.enabled = true;
    }
}