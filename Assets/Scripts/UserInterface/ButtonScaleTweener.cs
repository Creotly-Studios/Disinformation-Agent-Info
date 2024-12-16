using UnityEngine;
using UnityEngine.UI;


public class ButtonScaleTweener : MonoBehaviour
{
    private RectTransform targetButton; // The button to scale
    [SerializeField] private float minScale = 0.8f; // Minimum scale
    [SerializeField] public float maxScale = 1.2f; // Maximum scale
    [SerializeField] public float tweenSpeed = 2f; // Speed of the scaling

    private Vector3 initialScale;

    void Start()
    {
        targetButton = GetComponent<RectTransform>();

        // Store the initial scale of the button
        if (targetButton == null)
        {
            Debug.LogError("Target Button is not assigned!");
            return;
        }

        initialScale = targetButton.localScale;
    }

    void Update()
    {
        if (targetButton != null)
        {
            // Calculate the current scale using Mathf.PingPong
            float scale = Mathf.Lerp(minScale, maxScale, Mathf.PingPong(Time.time * tweenSpeed, 1));

            // Apply the scale to the button
            targetButton.localScale = initialScale * scale;
        }
    }
}
