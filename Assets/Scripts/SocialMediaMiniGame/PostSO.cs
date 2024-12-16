using UnityEngine;

[CreateAssetMenu(fileName = "PostSO", menuName = "Scriptable Objects/PostSO")]
public class PostSO : ScriptableObject
{
    [TextArea(2, 10)]public string postAuthor;
    public PostType postType;
    [Range(1, 99)] public float postIntegrity;

    [TextArea] public string postText;
    public Sprite postImage;

    [Space] [TextArea]
    public string funFact_Hint;
}

public enum PostType
{
    TextOnly,
    TextWithImage
}
