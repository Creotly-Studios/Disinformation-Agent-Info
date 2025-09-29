using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUp : MonoBehaviour
{
    public delegate void I_PopUp(string text, Color windowColor, Color textColor);
    public static I_PopUp initPopUp;
    public delegate void D_PopUp();
    public static D_PopUp dismissPopUp;
    
    public Image window;
    public TextMeshProUGUI windowText;
    private Vector2 startScale;
    
    public static void ShowPopUp(string text, Color w_color, Color t_color)
    {
        initPopUp?.Invoke(text, w_color, t_color);
    }
    
    public static void DismissPopUp()
    {
        dismissPopUp?.Invoke();
    }
    
    private void Awake()
    {
        startScale = transform.localScale;
    }
    
    private void Start()
    {
        transform.localScale = Vector2.zero;
    }
    
    private void OnEnable()
    {
        initPopUp += Show;
        dismissPopUp += Dismiss;
    }
    
    private void OnDisable()
    {
        initPopUp -= Show;
        dismissPopUp -= Dismiss;
    }
    
    private void Update()
    {
        if (!timed) return;
        if(Time.time > (time + 1.5f))
        {
            timed = false;
            Dismiss();
        }
    }
    
    private float time;
    private bool timed;
    private Coroutine fadeCoroutine;
    private Coroutine scaleCoroutine;
    
    private void Show(string text, Color w_color, Color t_color)
    {
        // Stop any running animations
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        
        timed = true;
        windowText.text = text;
        window.color = w_color;
        
        // Reset and start animations
        transform.localScale = new Vector2(startScale.x, 0);
        windowText.color = new Color(t_color.r, t_color.g, t_color.b, 0);
        
        scaleCoroutine = StartCoroutine(ScaleAnimation(true));
        fadeCoroutine = StartCoroutine(FadeTextAnimation(true, t_color));
        
        time = Time.time;
    }
    
    private void Dismiss()
    {
        // Stop any running animations
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        
        fadeCoroutine = StartCoroutine(FadeTextAnimation(false, windowText.color));
        scaleCoroutine = StartCoroutine(ScaleAnimation(false));
    }
    
    private IEnumerator ScaleAnimation(bool showing)
    {
        float elapsed = 0f;
        float duration = 0.1f;
        
        Vector2 startSize = showing ? new Vector2(startScale.x, 0) : startScale;
        Vector2 endSize = showing ? startScale : new Vector2(startScale.x, 0);
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            transform.localScale = Vector2.Lerp(startSize, endSize, progress);
            yield return null;
        }
        
        transform.localScale = endSize;
    }
    
    private IEnumerator FadeTextAnimation(bool fadeIn, Color targetColor)
    {
        float elapsed = 0f;
        float duration = 0.1f;
        
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        
        Color startColor = new Color(targetColor.r, targetColor.g, targetColor.b, startAlpha);
        Color endColor = new Color(targetColor.r, targetColor.g, targetColor.b, endAlpha);
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            windowText.color = Color.Lerp(startColor, endColor, progress);
            yield return null;
        }
        
        windowText.color = endColor;
    }
}