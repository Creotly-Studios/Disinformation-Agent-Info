using UnityEngine;

[CreateAssetMenu(fileName = "SourcePostSO", menuName = "Scriptable Objects/SourcePostSO")]
public class SourcePostSO : ScriptableObject
{
    [Header("Information")]
    public string title;
    public string authorName;
    [TextArea] public string writeUp;

    [Header("Answers")]
    public string[] options;
    [field: SerializeField] public string correctAnswer { get; private set; }
}
