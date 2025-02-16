using UnityEngine;

[ExecuteInEditMode]
public class ResetCanvasOpacityAtStart : MonoBehaviour
{
    [Range(0f, 1f)]
    public float editorAlpha = 0.5f; // Default alpha in the editor

    private CanvasGroup c;

    void Awake()
    {
        c = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (c != null)
        {
            c.alpha = 1f; // Reset to full opacity when game starts
        }
    }

    void Update()
    {
        if (!Application.isPlaying && c != null)
        {
            c.alpha = editorAlpha; // Set alpha in the editor
        }
    }
}
