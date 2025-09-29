using TMPro;
using UnityEngine;

public class DebugText : MonoBehaviour
{
    private int count;
    public static DebugText Instance;
    [SerializeField] private TextMeshProUGUI debugText;

    private void Start()
    {
        Instance = this;
    }

    public void AssignText(string message)
    {
        count++;
        debugText.text += $", message count:{count}: message: {message}";
    }
}
