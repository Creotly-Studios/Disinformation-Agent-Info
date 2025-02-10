using UnityEngine;

[CreateAssetMenu(fileName = "BingoPostSO", menuName = "Scriptable Objects/BingoPostSO")]
public class BingoPostSO : ScriptableObject
{
    [Header("Information")]
    public string authorName;
    [TextArea] public string article;
    [TextArea] public string answerExplanation;

    [Header("Options")]
    public string answer;
    public bool hasChecked;
    public string[] options;
}
