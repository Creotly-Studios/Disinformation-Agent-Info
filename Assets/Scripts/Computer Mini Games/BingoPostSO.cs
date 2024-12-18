using UnityEngine;

[CreateAssetMenu(fileName = "BingoPostSO", menuName = "Scriptable Objects/BingoPostSO")]
public class BingoPostSO : ScriptableObject
{
    [Header("Information")]
    public string authorName;
    [TextArea] public string article;

    [Header("Options")]
    public string answer;
    public string[] options;
}
